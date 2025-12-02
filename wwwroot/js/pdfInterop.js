window.pdfInterop = {
    loadPdfBase64: async function (base64, containerId, scale) {
        // Decodifica Base64 a binario
        const raw = atob(base64);
        const rawLength = raw.length;
        const array = new Uint8Array(new ArrayBuffer(rawLength));
        for (let i = 0; i < rawLength; i++) {
            array[i] = raw.charCodeAt(i);
        }

        // Carga el PDF desde el binario
        const pdf = await window.pdfjsLib.getDocument({ data: array }).promise;
        const container = document.getElementById(containerId);
        container.innerHTML = "";

        // Renderiza todas las páginas
        for (let pageNumber = 1; pageNumber <= pdf.numPages; pageNumber++) {
            const page = await pdf.getPage(pageNumber);
            const viewport = page.getViewport({ scale: 2.0 });

            const canvas = document.createElement("canvas");
            const context = canvas.getContext("2d");
            canvas.height = viewport.height;
            canvas.width = viewport.width;

            // ?? Ajuste visual sin perder nitidez
            canvas.style.width = "100%";
            canvas.style.height = "auto";

            container.appendChild(canvas);

            const renderContext = { canvasContext: context, viewport: viewport };
            await page.render(renderContext).promise;
        }

    }
};
