import { computePosition, flip, shift, autoUpdate } from 'https://cdn.jsdelivr.net/npm/@floating-ui/dom@1.7.4/+esm';

export async function InitializeSelect(
    triggerElement,
    contentElement,
    placement,
    callbackInstance,
    closeCallbackName,
    selectCallbackName
) {
    async function updatePosition() {
        const { x, y } = await computePosition(triggerElement, contentElement, {
            placement,
            middleware: [flip(), shift({ padding: 8 })],
        });

        Object.assign(contentElement.style, {
            left: `${x}px`,
            top: `${y}px`,
        });
    }

    const positionCleaner = autoUpdate(
        triggerElement,
        contentElement,
        updatePosition,
    );

    // Show dialog (use show() instead of showModal() to avoid backdrop and scroll issues)
    if (!contentElement.open && contentElement.show) {
        contentElement.show();
    }

    // Track focused item index
    let focusedIndex = -1;
    const getItems = () => contentElement.querySelectorAll('[role="option"]:not([data-disabled="true"])');

    function updateFocus() {
        const items = getItems();
        items.forEach((item, i) => {
            if (i === focusedIndex) {
                item.setAttribute('data-focused', 'true');
                item.scrollIntoView({ block: 'nearest' });
            } else {
                item.removeAttribute('data-focused');
            }
        });
    }

    // Click outside to close
    function handleClickOutside(e) {
        if (!contentElement.contains(e.target) && !triggerElement.contains(e.target)) {
            contentElement.close();
        }
    }

    // Close event callback
    function handleClose() {
        callbackInstance.invokeMethodAsync(closeCallbackName);
    }

    // Keyboard navigation
    function handleKeydown(e) {
        const items = getItems();

        switch (e.key) {
            case 'ArrowDown':
                e.preventDefault();
                focusedIndex = Math.min(focusedIndex + 1, items.length - 1);
                if (focusedIndex < 0) focusedIndex = 0;
                updateFocus();
                break;
            case 'ArrowUp':
                e.preventDefault();
                focusedIndex = Math.max(focusedIndex - 1, 0);
                updateFocus();
                break;
            case 'Enter':
            case ' ':
                e.preventDefault();
                if (focusedIndex >= 0 && focusedIndex < items.length) {
                    callbackInstance.invokeMethodAsync(selectCallbackName, focusedIndex);
                }
                break;
            case 'Escape':
                e.preventDefault();
                contentElement.close();
                break;
            case 'Home':
                e.preventDefault();
                focusedIndex = 0;
                updateFocus();
                break;
            case 'End':
                e.preventDefault();
                focusedIndex = items.length - 1;
                updateFocus();
                break;
            case 'Tab':
                e.preventDefault();
                contentElement.close();
                break;
        }
    }

    // Type-ahead search
    let searchBuffer = '';
    let searchTimeout;

    function handleTypeAhead(e) {
        if (e.key.length === 1 && !e.ctrlKey && !e.metaKey && !e.altKey) {
            const items = getItems();
            clearTimeout(searchTimeout);
            searchBuffer += e.key.toLowerCase();

            const matchIndex = Array.from(items).findIndex(item =>
                item.textContent.toLowerCase().startsWith(searchBuffer)
            );

            if (matchIndex >= 0) {
                focusedIndex = matchIndex;
                updateFocus();
            }

            searchTimeout = setTimeout(() => {
                searchBuffer = '';
            }, 500);
        }
    }

    document.addEventListener('click', handleClickOutside);
    contentElement.addEventListener('close', handleClose);
    document.addEventListener('keydown', handleKeydown);
    document.addEventListener('keydown', handleTypeAhead);

    // Set initial focus to selected item or first item
    const items = getItems();
    const selectedItem = contentElement.querySelector('[data-selected="true"]');
    if (selectedItem) {
        focusedIndex = Array.from(items).indexOf(selectedItem);
    } else if (items.length > 0) {
        focusedIndex = 0;
    }
    updateFocus();

    return {
        CleanUp: () => {
            if (contentElement.open && contentElement.close) {
                contentElement.close();
            }
            positionCleaner();
            document.removeEventListener('click', handleClickOutside);
            contentElement.removeEventListener('close', handleClose);
            document.removeEventListener('keydown', handleKeydown);
            document.removeEventListener('keydown', handleTypeAhead);
            clearTimeout(searchTimeout);
        }
    };
}
