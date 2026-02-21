import { PDFDocument } from "pdf-lib";

async function extractPages(pdfBytes, pageIndices) {
  const src = await PDFDocument.load(pdfBytes);
  const out = await PDFDocument.create();

  const copied = await out.copyPages(src, pageIndices);
  copied.forEach((p) => out.addPage(p));

  return await out.save(); // Uint8Array
}

// API globale stable pour JSInterop (Blazor)
window.PdfTools = {
  extractPages,
};