import { ApplySortable } from "./Sortable.razor.js";

/**
 * Applies sortable interactivity on enhanced load
 */
export function onUpdate() {
  Array.from(document.querySelectorAll("#pango-ui-sortable"))
    .forEach(ApplySortable);
}
