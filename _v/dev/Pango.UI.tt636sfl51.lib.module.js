// Reference: https://learn.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability/static-server-rendering

import * as PageScripts from './Components/PageScript/PageScript.razor.js';

export function beforeWebStart() {
    // const customScript = document.createElement('script');
    // customScript.setAttribute('src', 'scripts/beforeStartScripts/index.js');
    // document.head.appendChild(customScript);
}

export function afterWebStarted(blazor) {
    PageScripts.afterWebStarted(blazor)
}
