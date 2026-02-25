import { PDFDocument, rgb, degrees, StandardFonts } from "pdf-lib";
import { loadPdf, destroyPdf, renderPageToObjectUrl, revokeObjectUrl } from "./page-picker.js";

/* ------------------------
   Helpers
------------------------- */
function assertPdfBytes(bytes, label = "PDF") {
    if (!bytes || bytes.length === 0) {
        throw new Error(`${label}: empty file.`);
    }
}
function toUint8Array(raw, label = "PDF") {
    if (!raw) return null;
    if (raw instanceof Uint8Array) return raw;
    if (raw instanceof ArrayBuffer) return new Uint8Array(raw);
    if (ArrayBuffer.isView(raw)) return new Uint8Array(raw.buffer, raw.byteOffset, raw.byteLength);
    if (typeof raw === "string") throw new Error(`${label}: received a string. Expected raw bytes (Uint8Array).`);

    return null;
}

async function getPageCount(pdfBytes) {
    assertPdfBytes(pdfBytes, "getPageCount");
    const doc = await PDFDocument.load(pdfBytes);
    return doc.getPageCount();
}

/* ------------------------
   ExtractPages 
------------------------- */

async function extractPages(pdfBytes, pageIndices) {
    assertPdfBytes(pdfBytes, "extractPages");
    const src = await PDFDocument.load(pdfBytes);
    const out = await PDFDocument.create();

    const copied = await out.copyPages(src, pageIndices);
    copied.forEach((p) => out.addPage(p));

    return await out.save(); 
}

/* ------------------------
   MergePdfs 
------------------------- */

async function mergePdfs(pdfsBytesArray) {
    if (!Array.isArray(pdfsBytesArray) || pdfsBytesArray.length < 2) {
        throw new Error("mergePdfs: At least 2 PDFs are required.");
    }
    const merged = await PDFDocument.create();

    for (let i = 0; i < pdfsBytesArray.length; i++) {
        const bytes = toUint8Array(pdfsBytesArray[i], `pdf #${i + 1}`);
        assertPdfBytes(bytes, `pdf #${i + 1}`);

        const doc = await PDFDocument.load(bytes);
        const pages = await merged.copyPages(doc, doc.getPageIndices());
        pages.forEach(p => merged.addPage(p));
    }

    return await merged.save(); 
}

/* ------------------------
   DELETE PAGES
------------------------- */
async function deletePages(pdfBytes, pageIndices) {
    assertPdfBytes(pdfBytes, "deletePages");
    const src = await PDFDocument.load(pdfBytes);
    const out = await PDFDocument.create();

    const total = src.getPageCount();
    const toDelete = new Set(pageIndices || []);

    // Keep pages not in toDelete
    const keepIndices = [];
    for (let i = 0; i < total; i++) {
        if (!toDelete.has(i)) keepIndices.push(i);
    }

    if (keepIndices.length === 0) {
        throw new Error("deletePages: all pages were removed (result would be empty).");
    }

    const copied = await out.copyPages(src, keepIndices);
    copied.forEach((p) => out.addPage(p));

    return await out.save(); 
}

/* ------------------------
   REORDER PAGES
------------------------- */
async function reorderPages(pdfBytes, newOrder) {
    assertPdfBytes(pdfBytes, "reorderPages");
    const src = await PDFDocument.load(pdfBytes);
    const out = await PDFDocument.create();

    const total = src.getPageCount();
    if (!newOrder || newOrder.length !== total) {
        throw new Error(`reorderPages: newOrder must have exactly ${total} items.`);
    }

    const seen = new Set();
    for (const idx of newOrder) {
        if (!Number.isInteger(idx) || idx < 0 || idx >= total) {
            throw new Error(`reorderPages: invalid page index ${idx}.`);
        }
        if (seen.has(idx)) {
            throw new Error(`reorderPages: duplicate page index ${idx}.`);
        }
        seen.add(idx);
    }

    const copied = await out.copyPages(src, newOrder);
    copied.forEach((p) => out.addPage(p));

    return await out.save(); 
}

/* ------------------------
   SPLIT BY RANGES
------------------------- */
async function splitByRanges(pdfBytes, ranges) {
    assertPdfBytes(pdfBytes, "splitByRanges");
    const src = await PDFDocument.load(pdfBytes);
    const total = src.getPageCount();

    if (!ranges || ranges.length === 0) {
        throw new Error("splitByRanges: ranges is empty.");
    }

    const outputs = [];

    for (const r of ranges) {
        if (!r || !Number.isInteger(r.start) || !Number.isInteger(r.end)) {
            throw new Error("splitByRanges: each range must have {start,end} integers.");
        }

        let start = r.start;
        let end = r.end;

        if (start < 0 || end < 0 || start >= total || end >= total) {
            throw new Error(`splitByRanges: range out of bounds (0..${total - 1}).`);
        }

        if (end < start) [start, end] = [end, start];

        const out = await PDFDocument.create();
        const indices = [];
        for (let i = start; i <= end; i++) indices.push(i);

        const copied = await out.copyPages(src, indices);
        copied.forEach((p) => out.addPage(p));

        outputs.push(await out.save()); 
    }

    return outputs; 
}

/* ------------------------
   WATERMARK
------------------------- */
async function addTextWatermark(pdfBytes, options = {}) {
    assertPdfBytes(pdfBytes, "addTextWatermark");

    const {
        text = "CONFIDENTIAL",
        fontSize = 48,
        opacity = 0.15,
        rotationDegrees = 45,
        // couleur gris foncé
        color = { r: 0.2, g: 0.2, b: 0.2 },
        // "center" | "diagonal" (diagonal = center + rotation)
        placement = "diagonal",
    } = options;

    const pdfDoc = await PDFDocument.load(pdfBytes);
    const font = await pdfDoc.embedFont(StandardFonts.HelveticaBold);

    const pages = pdfDoc.getPages();
    for (const page of pages) {
        const { width, height } = page.getSize();

        // on centre le watermark
        const textWidth = font.widthOfTextAtSize(text, fontSize);
        const x = (width - textWidth) / 2 + 50;
        const y = height / 2 -100;

        const rotate = placement === "diagonal" ? degrees(rotationDegrees) : degrees(0);

        page.drawText(text, {
            x,
            y,
            size: fontSize,
            font,
            color: rgb(color.r, color.g, color.b),
            rotate,
            opacity,
        });
    }

    return await pdfDoc.save(); 
}

/* ------------------------
   ADD PAGE NUMBERS
------------------------- */

async function addPageNumbers(pdfBytes, options = {}) {
    assertPdfBytes(pdfBytes, "addPageNumbers");

    const {
        fontSize = 12,
        opacity = 0.9,
        color = { r: 0, g: 0, b: 0 },
        marginX = 40,
        marginY = 25,
        // "bottom-right" | "bottom-center" | "bottom-left" | "top-right" | ...
        position = "bottom-right",
        // template support: {n} {total}
        template = "{n}/{total}",
        startAt = 1, // utile si tu veux commencer à 1 (default)
    } = options;

    const pdfDoc = await PDFDocument.load(pdfBytes);
    const font = await pdfDoc.embedFont(StandardFonts.Helvetica);

    const pages = pdfDoc.getPages();
    const total = pages.length;

    for (let i = 0; i < total; i++) {
        const page = pages[i];
        const { width, height } = page.getSize();

        const n = i + startAt;
        const label = template
            .replace("{n}", String(n))
            .replace("{total}", String(total));

        const textWidth = font.widthOfTextAtSize(label, fontSize);

        let x = marginX;
        let y = marginY;

        // X
        if (position.includes("right")) x = width - marginX - textWidth;
        else if (position.includes("center")) x = (width - textWidth) / 2;

        // Y
        if (position.includes("top")) y = height - marginY - fontSize;
        else if (position.includes("middle")) y = (height - fontSize) / 2;
        // bottom default

        page.drawText(label, {
            x,
            y,
            size: fontSize,
            font,
            color: rgb(color.r, color.g, color.b),
            opacity,
        });
    }

    return await pdfDoc.save(); 
}

/* ------------------------
   DOWNLOAD
------------------------- */
function downloadBytes(filename, bytes, mimeType = "application/pdf") {
    const blob = new Blob([bytes], { type: mimeType });
    const url = URL.createObjectURL(blob);

    const a = document.createElement("a");
    a.href = url;
    a.download = filename;
    a.click();

    URL.revokeObjectURL(url);
}

/* ------------------------
   Export to window
------------------------- */
window.PdfTools = window.PdfTools || {};

Object.assign(window.PdfTools, {
    getPageCount,
    extractPages,
    mergePdfs,
    deletePages,
    reorderPages,
    splitByRanges,
    addTextWatermark,
    addPageNumbers,
    downloadBytes,

    loadPdf,
    renderPageToObjectUrl,
    revokeObjectUrl,
    destroyPdf,
});
