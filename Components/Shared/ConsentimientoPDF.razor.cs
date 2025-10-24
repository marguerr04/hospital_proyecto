// Components/Shared/ConsentimientoPDF.razor.cs
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace proyecto_hospital_version_1.Components.Shared
{
    public partial class ConsentimientoPdf
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter]
        public string NombrePaciente { get; set; } = "";

        [Parameter]
        public string RutPaciente { get; set; } = "";

        [Parameter]
        public string Procedimiento { get; set; } = "";

        [Parameter]
        public string Lateralidad { get; set; } = "";

        [Parameter]
        public string Extremidad { get; set; } = "";

        [Parameter]
        public string Procedencia { get; set; } = "";

        private async Task GenerarPdf()
        {
            // Configurar QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .AlignCenter()
                        .Text("CONSENTIMIENTO INFORMADO")
                        .SemiBold().FontSize(16).FontColor(Colors.Blue.Darken3);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            // Información del paciente
                            column.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(infoColumn =>
                            {
                                infoColumn.Item().Text($"PACIENTE: {NombrePaciente}").SemiBold();
                                infoColumn.Item().Text($"RUT: {RutPaciente}");
                                infoColumn.Item().Text($"PROCEDENCIA: {Procedencia}");
                            });

                            // Detalles del procedimiento
                            column.Item().Text($"PROCEDIMIENTO QUIRÚRGICO: {Procedimiento}").SemiBold();

                            if (!string.IsNullOrEmpty(Lateralidad))
                                column.Item().Text($"LATERALIDAD: {Lateralidad}");

                            if (!string.IsNullOrEmpty(Extremidad))
                                column.Item().Text($"EXTREMIDAD: {Extremidad}");

                            // Contenido del consentimiento
                            column.Item().Text("DECLARO QUE:").SemiBold().FontSize(12);
                            column.Item().Text("1. He sido informado(a) sobre la naturaleza del procedimiento quirúrgico, sus beneficios, riesgos y alternativas.");
                            column.Item().Text("2. Comprendo los posibles riesgos y complicaciones asociadas al procedimiento.");
                            column.Item().Text("3. He tenido la oportunidad de hacer todas las preguntas necesarias.");
                            column.Item().Text("4. Doy mi consentimiento voluntario para la realización del procedimiento.");

                            // Espacio para firmas
                            column.Item().PaddingTop(2, Unit.Centimetre).Column(firmaColumn =>
                            {
                                firmaColumn.Item().Text("_________________________________________");
                                firmaColumn.Item().Text("Firma del Paciente o Representante Legal");

                                firmaColumn.Item().PaddingTop(1, Unit.Centimetre).Text("_________________________________________");
                                firmaColumn.Item().Text("Firma del Médico Tratante");

                                firmaColumn.Item().PaddingTop(1, Unit.Centimetre).Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Hospital - ");
                            x.Span(DateTime.Now.ToString("dd/MM/yyyy"));
                        });
                });
            });

            // Generar PDF como stream - FORMA CORREGIDA
            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            var fileContent = stream.ToArray();

            // Descargar el PDF
            await DescargarArchivo(fileContent, $"Consentimiento_{NombrePaciente}_{DateTime.Now:yyyyMMddHHmm}.pdf");
        }

        private async Task DescargarArchivo(byte[] fileContent, string fileName)
        {
            var contentType = "application/pdf";

            // Usar IJSRuntime para descargar
            await JSRuntime.InvokeVoidAsync("descargarArchivo",
                fileName,
                contentType,
                Convert.ToBase64String(fileContent));
        }
    }
}