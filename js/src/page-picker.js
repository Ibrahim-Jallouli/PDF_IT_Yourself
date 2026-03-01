import * as pdfjsLib from "pdfjs-dist";

pdfjsLib.GlobalWorkerOptions.workerSrc = new URL(
    "pdfjs-dist/build/pdf.worker.min.mjs",
    import.meta.url
).toString();

function toUint8Array(raw, label = "PDF") {
    if (!raw) throw new Error(`${label}: missing bytes`);
    if (raw instanceof Uint8Array) return raw;
    if (raw instanceof ArrayBuffer) return new Uint8Array(raw);
    if (ArrayBuffer.isView(raw)) return new Uint8Array(raw.buffer, raw.byteOffset, raw.byteLength);
    throw new Error(`${label}: expected Uint8Array/ArrayBuffer`);
}

export async function loadPdf(pdfBytesRaw) {
    const pdfBytes = toUint8Array(pdfBytesRaw, "loadPdf");
    const loadingTask = pdfjsLib.getDocument({ data: pdfBytes });
    const pdf = await loadingTask.promise;
    return pdf; // garde-le en mémoire pour le viewer
}

export async function renderPageToObjectUrl(pdf, pageIndex0, options = {}) {
    const { scale = 0.25, mime = "image/png", quality = 0.92 } = options;

    const pageNumber = pageIndex0 + 1;
    if (pageNumber < 1 || pageNumber > pdf.numPages) {
        throw new Error(`pageIndex0 out of range: ${pageIndex0} (numPages=${pdf.numPages})`);
    }

    const page = await pdf.getPage(pageNumber);
    const viewport = page.getViewport({ scale });

    const canvas = document.createElement("canvas");
    const ctx = canvas.getContext("2d", { alpha: false });

    canvas.width = Math.ceil(viewport.width);
    canvas.height = Math.ceil(viewport.height);

    await page.render({ canvasContext: ctx, viewport }).promise;
    page.cleanup?.();

    const blob = await new Promise((resolve, reject) => {
        canvas.toBlob(
            b => (b ? resolve(b) : reject(new Error("toBlob returned null"))),
            mime,
            quality
        );
    });

    return URL.createObjectURL(blob);
}

export function revokeObjectUrl(url) {
    try { URL.revokeObjectURL(url); } catch { }
}

export async function destroyPdf(pdf) {
    await pdf.cleanup?.();
    await pdf.destroy?.();
}