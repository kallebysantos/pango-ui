import { computePosition, flip, shift, autoUpdate, offset } from 'https://cdn.jsdelivr.net/npm/@floating-ui/dom@1.7.4/+esm';

export async function ComputeDialogPosition(
    anchorElement,
    dialogElement,
    placement,
    callbackInstance,
    callbackName
) {
    const scrollX = window.scrollX;
    const scrollY = window.scrollY;

    async function updatePosition() {
        const {x, y} = await computePosition(anchorElement, dialogElement, {
            placement,
            middleware: [flip(), shift(), offset(5)],
        });

        Object.assign(dialogElement.style, {
            left: `${x}px`,
            top: `${y}px`,
        });
    }

    const positionCleaner = autoUpdate(
        anchorElement,
        dialogElement,
        updatePosition,
    )

    if (!dialogElement.open && !!dialogElement.showModal) {
        dialogElement.showModal();

        window.scrollTo(scrollX, scrollY);

        dialogElement.addEventListener('click', () => {
            dialogElement.close();
        });

        dialogElement.addEventListener('close', () => {
            callbackInstance.invokeMethodAsync(callbackName)
        });
    }

    return {
        CleanUpPosition: () => {
            if (dialogElement.open && !!dialogElement.close) {
                dialogElement.close();
            }

            positionCleaner()
        }
    };
}

export function ScrollToElement(dialogElement, targetValue) {
    const scrollX = window.scrollX;
    const scrollY = window.scrollY;
    const options = dialogElement.querySelectorAll('[role="option"]');
    const targetOption = Array.from(options).find(opt => opt.textContent.trim() === targetValue);

    if (!targetOption) return;

    requestAnimationFrame(() => {
        targetOption.scrollIntoView({
            block: 'nearest',
            behavior: 'contain'
        });
        window.scrollTo(scrollX, scrollY);
    });

}
