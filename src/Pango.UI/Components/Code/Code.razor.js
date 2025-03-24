import { codeToHtml } from 'https://esm.sh/shiki@3.0.0'

export async function ApplySyntaxHighlight(elementRef) {
  elementRef.toggleAttribute("data-loading", true);

  const codeLang = elementRef.getAttribute("data-language");
  const codeStr = elementRef.querySelector("textarea").value;
  const outputContainer = elementRef.querySelector("div#pango-ui-code-container");

  const dark = await codeToHtml(codeStr, {
    lang: codeLang,
    theme: 'github-dark-default'
  });

  const light = await codeToHtml(codeStr, {
    lang: codeLang,
    theme: 'github-light-default'
  });

  outputContainer.innerHTML = dark + light;
  elementRef.toggleAttribute("data-loading", false);
}
