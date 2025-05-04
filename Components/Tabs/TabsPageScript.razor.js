import { RegisterTabs } from './Tabs.razor.js';

/**
 * Register tabs on enhanced load
 */
export function onUpdate() {
  Array.from(document.querySelectorAll("#pago-ui-tabs"))
    .forEach(RegisterTabs);
}
