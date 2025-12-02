using MauiBlazorHybrid.Domain.Dtos;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorHybrid.Domain.Helpers
{
    public static class PdfCreator
    {
        public static byte[] CrearPdfBasico(HistorialMedicoDto historial, PacienteDto paciente) {

            var doc = new Document();
            var section = doc.AddSection();

            var titulo = section.AddParagraph("Historial Médico");
            titulo.Format.Font.Size = 16;
            titulo.Format.Font.Bold = true;
            titulo.Format.SpaceAfter = "1cm";

            // Datos del paciente
            section.AddParagraph("Paciente: " + historial.NombrePaciente);
            section.AddParagraph("Edad: " + paciente.Edad);
            section.AddParagraph("Diagnóstico: " + historial.Diagnostico);
            section.AddParagraph("Tratamiento: " + historial.Tratamiento);
            section.AddParagraph("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy"));

            var renderer = new PdfDocumentRenderer(unicode: true)
            {
                Document = doc
            };
            renderer.RenderDocument();

            using var ms = new MemoryStream();
            renderer.PdfDocument.Save(ms, false);
            return ms.ToArray();

        }
    }
}
