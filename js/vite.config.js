import { defineConfig } from "vite";
import path from "node:path";

export default defineConfig({
  build: {
    lib: {
      entry: path.resolve(process.cwd(), "src/pdf-tools.js"),
      name: "PdfToolsLib",
      formats: ["iife"],
      fileName: () => "pdf-tools.bundle.js",
    },
    outDir: path.resolve(process.cwd(), "../wwwroot/dist"),
    emptyOutDir: true,
  },
});