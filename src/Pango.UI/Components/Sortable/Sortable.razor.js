// Sortable will auto be global available
import * as _ISortable from "https://cdn.jsdelivr.net/npm/sortablejs@latest/Sortable.js";

/** @type {_ISortable } */
const Sortable = globalThis.Sortable;

export function ApplySortable(elementRef) {
  Sortable.create(elementRef, {
    handle: "#pango-ui-sortable-handle",
  });
}
