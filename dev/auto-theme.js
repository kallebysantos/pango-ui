const getThemePreference = () => {
  if (typeof localStorage !== 'undefined' && localStorage.getItem('theme')) {
    return localStorage.getItem('theme');
  }
  return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
};

const isDark = () => document.documentElement.classList.contains('dark');

const setTheme = (theme) => {
  const isDarkTheme = theme === 'dark';
  document.documentElement.classList[isDarkTheme  ? 'add' : 'remove']('dark');
}

(() => {
  const preferedTheme = getThemePreference();
  setTheme(preferedTheme)

  if (typeof localStorage !== 'undefined') {
    const observer = new MutationObserver(() => {
      localStorage.setItem('theme', isDark() ? 'dark' : 'light');
    });
    observer.observe(document.documentElement, {attributes: true, attributeFilter: ['class']});
  }
})();
