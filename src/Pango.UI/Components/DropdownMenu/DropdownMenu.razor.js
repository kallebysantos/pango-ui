import {computePosition, flip, shift, autoUpdate} from 'https://cdn.jsdelivr.net/npm/@floating-ui/dom@1.7.4/+esm';

export async function ComputeDialogPosition(
    anchorElement,
    dialogElement,
    callbackInstance,
    callbackName
) {
    async function updatePosition() {
        const {x, y} = await computePosition(anchorElement, dialogElement, {
            placement: 'bottom-start',
            middleware: [flip(), shift()],
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
