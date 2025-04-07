// Sortable will auto be global available
import * as _ISortable from "https://cdn.jsdelivr.net/npm/sortablejs@latest/Sortable.js";

/** @type {_ISortable } */
const Sortable = globalThis.Sortable;

export function ApplySortable(elementRef, sortableInstance) {
  const sortableList = elementRef.querySelector("ul");

  // Using SSR form handling
  const sortableHandlerTrigger = elementRef.querySelector(
    "form#pango-ui-sortable-handler > input[type=submit]",
  );
  const oldIndexInput = elementRef.querySelector(
    "input#pango-ui-sortable-old-index",
  );
  const newIndexInput = elementRef.querySelector(
    "input#pango-ui-sortable-new-index",
  );

  Sortable.create(sortableList, {
    handle: "#pango-ui-sortable-handle",
    onUpdate: ({ oldIndex, newIndex }) => {
      oldIndexInput.value = oldIndex;
      newIndexInput.value = newIndex;

      if (sortableInstance) {
        sortableInstance.invokeMethodAsync("UpdateIndex", oldIndex, newIndex);
      }

      sortableHandlerTrigger.click();
    },
  });
}
