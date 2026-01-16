export async function ShowDialog(
    dialogElement,
    preventKeyboard,
    callbackInstance,
    callbackName
) {
    if (!dialogElement.open && !!dialogElement.showModal) {
        if (preventKeyboard) {
            dialogElement.show();
        } else {
            dialogElement.showModal();
        }

        dialogElement.addEventListener('close', (ev) => {
            callbackInstance.invokeMethodAsync(callbackName)
        });
    }
}

export async function CloseDialog(dialogElement) {
    if (!dialogElement.open) return;

    dialogElement.close();
}
