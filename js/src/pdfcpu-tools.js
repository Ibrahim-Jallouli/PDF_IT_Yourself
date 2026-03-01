import { Pdfcpu } from "pdfcpu-wasm";

let _pdfcpu = null;

function getPdfcpu() {
    if (!_pdfcpu) _pdfcpu = new Pdfcpu();
    return _pdfcpu;
}