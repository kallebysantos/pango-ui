/**
 * Applies dialog on enhanced load
 */
export function onUpdate() {
  document.querySelectorAll("#dialog-wrapper > #dialog-trigger")
    .forEach(dialogTrigger => dialogTrigger.addEventListener('click', (ev) => {
      if (!ev.currentTarget) return;

      const dialogId = dialogTrigger.getAttribute('data-open');
      const preventKeyboardDismiss = dialogTrigger.getAttribute('data-prevent-keyboard-dismiss');

      const dialogElement = document.querySelector(`#dialog-wrapper > dialog#${dialogId}`);
      const closeElements = document.querySelectorAll('#dialog-wrapper button#dialog-close');
      if (!dialogElement) return;

      if (preventKeyboardDismiss) {
        dialogElement.show();
      } else {
        dialogElement.showModal();
      }

      closeElements.forEach(closeTrigger => closeTrigger.addEventListener('click', (ev) => {
        dialogElement.close();
      }));
    }));


  const openDialogStack = Array.from(document.querySelectorAll('#dialog-wrapper[data-state=open]'));

  openDialogStack.forEach((dialogWrapper) => {
    dialogWrapper.querySelector('dialog').addEventListener('close', ev => {
      const nextDialog = openDialogStack.shift();
      if (!nextDialog) return;

      nextDialog.querySelector('#dialog-trigger').click();
    })
  });

  openDialogStack.shift()?.querySelector('#dialog-trigger').click();
}
