window.pdfDrop = {
  wireDropZone: function (dropzoneSelector, inputId) {
    const dz = document.querySelector(dropzoneSelector);
    const input = document.getElementById(inputId);
    if (!dz || !input) return;

    dz.addEventListener("dragover", (e) => {
      e.preventDefault();
    });

    dz.addEventListener("drop", (e) => {
      e.preventDefault();
      if (!e.dataTransfer || !e.dataTransfer.files || e.dataTransfer.files.length === 0) return;
      const dt = new DataTransfer();
      for (const f of e.dataTransfer.files) dt.items.add(f);
      input.files = dt.files;

      input.dispatchEvent(new Event("change", { bubbles: true }));
    });
  }
};
window.pdfDrop = window.pdfDrop || {};

window.pdfDrop.openPicker = function (inputId) {
    const input = document.getElementById(inputId);
    if (input) input.click();
};