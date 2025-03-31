import { codeToHtml } from 'https://esm.sh/shiki@3.0.0'

export async function ApplySyntaxHighlight(elementRef) {
  const isHtmlElement = elementRef instanceof Element;
  if (!elementRef || !isHtmlElement) return;

  elementRef.toggleAttribute("data-loading", true);

  const codeValueWrapper = elementRef.querySelector("textarea");

  const codeLang = elementRef.getAttribute("data-language");
  const outputContainer = elementRef.querySelector("div#pango-ui-code-container");

  outputContainer.innerHTML = await codeToHtml(codeValueWrapper.value, {
    lang: codeLang,
    themes: {
      light: 'catppuccin-latte',
      dark: 'catppuccin-mocha',
    }
  });

  Array.from(outputContainer.querySelectorAll("pre"))
    .forEach(codeBlock => {
      codeValueWrapper.addEventListener('scroll', () => {
        codeBlock.scrollTop = codeValueWrapper.scrollTop;
        codeBlock.scrollLeft = codeValueWrapper.scrollLeft;
      });
    });


  elementRef.toggleAttribute("data-loading", false);
}
