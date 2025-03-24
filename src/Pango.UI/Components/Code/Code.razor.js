import { codeToHtml } from 'https://esm.sh/shiki@3.0.0'

export async function ApplySyntaxHighlight(elementRef) {
  elementRef.toggleAttribute("data-loading", true);

  const codeValueWrapper = elementRef.querySelector("textarea");

  const codeLang = elementRef.getAttribute("data-language");
  const outputContainer = elementRef.querySelector("div#pango-ui-code-container");

  const dark = await codeToHtml(codeValueWrapper.value, {
    lang: codeLang,
    theme: 'github-dark-default'
  });

  const light = await codeToHtml(codeValueWrapper.value, {
    lang: codeLang,
    theme: 'github-light-default'
  });

  outputContainer.innerHTML = dark + light;

  Array.from(outputContainer.querySelectorAll("pre"))
    .forEach(codeBlock => {
      codeValueWrapper.addEventListener('scroll', () => {
        codeBlock.scrollTop = codeValueWrapper.scrollTop;
        codeBlock.scrollLeft = codeValueWrapper.scrollLeft;
      });
    });


  elementRef.toggleAttribute("data-loading", false);
}
