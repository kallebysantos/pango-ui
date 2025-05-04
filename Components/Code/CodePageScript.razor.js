import { ApplySyntaxHighlight } from './Code.razor.js';

/**
 * Applies code syntax highligth on enhanced load
 */
export function onUpdate() {
  Array.from(document.querySelectorAll("#pango-ui-code"))
    .forEach(ApplySyntaxHighlight);
}
