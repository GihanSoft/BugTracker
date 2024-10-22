
!function () {
  const bgColors = [
    "#1abc9c", "#2ecc71", "#3498db", "#9b59b6", "#16a085", "#27ae60", "#2980b9", "#8e44ad",
    "#f1c40f", "#e67e22", "#e74c3c", "#95a5a6", "#f39c12", "#d35400", "#c0392b", "#7f8c8d"
  ]

  const setAvatarBg = () => {
    let index = 0;
    document.querySelectorAll('.avatar').forEach(el => {
      el.style.backgroundColor = bgColors[index];
      el.style.color = '#fff';
      index = (index + 1) % bgColors.length;
    });
  };

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

  const domChanged = () => {
    setAvatarBg();
    addConfirmToFormsIfNeeded();
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

  mdui.loadLocale((locale) => import(`/lib/mdui/${locale}.min.js`));
  mdui.setLocale('fa-ir');
}()
