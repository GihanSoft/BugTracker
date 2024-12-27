
!function () {

  /**
   *
   * @param {SubmitEvent} e
   */
  const onDeleteSubmit = (e) => {
    if (confirm("از حذف مطمئن هستید؟")) {
    }
    else {
      e.preventDefault();
    }
  }

  const addConfirmToFormsIfNeeded = () => {

    document.querySelectorAll("form").forEach(el => {
      el.removeEventListener("submit", onDeleteSubmit);
      if (el.hasAttribute('needs-confirm')) {
        el.addEventListener("submit", onDeleteSubmit);
      }
    });
  }

  const updateColorMode = () => {
    const themeIcon = document.querySelector('#dark-mode-toggle use');
    const rootElement = document.querySelector('html');
    let theme = localStorage.getItem('theme');

    if (theme === null) {
      theme = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches
        ? 'dark'
        : 'light';
    }

    if (rootElement.getAttribute('data-bs-theme') !== theme) {
      rootElement.setAttribute('data-bs-theme', theme);
      if (theme === 'dark') {
        rootElement.classList.add('mdui-theme-dark')
      }
      else {
        rootElement.classList.remove('mdui-theme-dark')
      }
    }

    const iconValue = theme === 'dark'
      ? 'lib/bootstrap-icons/bootstrap-icons.svg#moon-stars-fill'
      : 'lib/bootstrap-icons/bootstrap-icons.svg#sun-fill';
    if (themeIcon.getAttribute('xlink:href') !== iconValue) {
      themeIcon.setAttribute('xlink:href', iconValue);
    }

    return;
  }
  const toggleColorMode = () => {
    const theme = localStorage.getItem('theme');
    const preferedColor = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches
      ? 'dark'
      : 'light';

    if (theme === null) {
      localStorage.setItem('theme', preferedColor === 'dark' ? 'light' : 'dark');
      updateColorMode();
      return;
    }

    if (theme === preferedColor) {
      localStorage.setItem('theme', theme === 'dark' ? 'light' : 'dark');
      updateColorMode();
      return;
    }

    localStorage.removeItem('theme');
    updateColorMode();
  }
  const toggleButton = document.getElementById('dark-mode-toggle');
  toggleButton.addEventListener('click', toggleColorMode);

  const domChanged = (a, b) => {
    addConfirmToFormsIfNeeded();
    updateColorMode();

    document.querySelectorAll('mdui-select[multiple]').forEach(select => {
      const selected = [];
      for (var i = 0; i < select.children.length; i++) {
        var option = select.children[i];
        if (option.selected) {
          selected.push(option.value);
        }
      }

      select.value = selected;
    });
  }

  const observer = new MutationObserver(domChanged);
  observer.observe(document.body, { subtree: true, childList: true, attributes: true });
  domChanged();

  mdui.loadLocale((locale) => import(`/lib/mdui/locales/${locale}.min.js`));
  mdui.setLocale('fa-ir');
}()
