using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using proyecto_hospital_version_1.Models; 
using System; 
namespace proyecto_hospital_version_1.Components.Shared
{
    public partial class ConsentimientoPDF
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter]
        public PacienteHospital? Paciente { get; set; }

        [Parameter]
        public string Procedimiento { get; set; }  

        [Parameter]
        public string Lateralidad { get; set; }  

        [Parameter]
        public string Extremidad { get; set; } 

        public async Task GenerarPdf()
        {
            if (Paciente == null)
            {
                await JSRuntime.InvokeVoidAsync("alert", "Error: No hay datos del paciente para generar el PDF.");
                return;
            }

            // Configurar QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12)); // Tamaño de fuente base

                    page.Header()
                        .AlignCenter()
                        .Text("Resumen de Solicitud y Consentimiento") // Título actualizado
                        .SemiBold().FontSize(18).FontColor(Colors.Black);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(20); // Más espacio entre elementos

                            
                            column.Item().Column(infoColumn =>
                            {

                                infoColumn.Item().PaddingBottom(5).Text("1. Datos del Paciente").FontSize(14).SemiBold();
                                infoColumn.Item().Grid(grid =>
                                {
                                    grid.Columns(2); // Dos columnas
                                    grid.Item().Text($"Nombre:").Bold();
                                    grid.Item().Text($"{Paciente.NombreCompleto}");

                                    grid.Item().Text($"RUT:").Bold();
                                    grid.Item().Text($"{Paciente.rut}-{Paciente.dv}");

                                    grid.Item().Text($"Edad:").Bold();
                                    grid.Item().Text($"{Paciente.EdadCompleta}");

                                    grid.Item().Text($"Sexo:").Bold();
                                    grid.Item().Text($"{Paciente.sexo}");
                                });
                            });

                            // --- SECCIÓN DEL PROCEDIMIENTO ---
                            column.Item().Column(procColumn =>
                            {
                                procColumn.Item().PaddingBottom(8).Text("2. Detalles del Procedimiento").FontSize(14).SemiBold();

                                procColumn.Item().Grid(grid =>
                                {
                                    grid.Columns(2);
                                    grid.VerticalSpacing(8);
                                    grid.HorizontalSpacing(10);

                                    grid.Item().Text("Procedimiento Principal:").Bold();
                                    grid.Item().Text($"{Procedimiento}");

                                    if (!string.IsNullOrEmpty(Lateralidad))
                                    {
                                        grid.Item().Text("Lateralidad:").Bold();
                                        grid.Item().Text(Lateralidad);
                                    }

                                    if (!string.IsNullOrEmpty(Extremidad))
                                    {
                                        grid.Item().Text("Extremidad:").Bold();
                                        grid.Item().Text(Extremidad);
                                    }
                                });
                            });


                            column.Item().Column(consentColumn =>
                            {
                                // --- CORRECCIÓN 3 ---
                                // .PaddingBottom(5) movido ANTES de .Text()
                                consentColumn.Item().PaddingBottom(5).Text("3. Consentimiento").FontSize(14).SemiBold();

                                // Tu texto personalizado
                                consentColumn.Item().Text(txt =>
                                {
                                    txt.Span("Yo, ");
                                    txt.Span($"{Paciente.NombreCompleto}").Bold();
                                    txt.Span(", RUT ");
                                    txt.Span($"{Paciente.rut}-{Paciente.dv}").Bold();
                                    txt.Span(", en pleno uso de mis facultades, declaro que he sido informado(a) y doy mi consentimiento para la realización del procedimiento: ");
                                    txt.Span($"'{Procedimiento}'").Bold();
                                    txt.Span(".");
                                });

                                consentColumn.Item().PaddingTop(10).Text("Comprendo los riesgos, beneficios y alternativas, y he tenido la oportunidad de aclarar todas mis dudas con el equipo médico.");
                            });


                            // --- Espacio para firmas ---
                            column.Item().PaddingTop(3, Unit.Centimetre).Grid(grid =>
                            {
                                grid.Columns(2); // Dos columnas para las firmas
                                grid.Item().AlignCenter().Column(col =>
                                {
                                    col.Item().Text("_________________________________________");
                                    col.Item().Text("Firma del Paciente o Representante Legal");
                                });
                                grid.Item().AlignCenter().Column(col =>
                                {
                                    col.Item().Text("_________________________________________");
                                    col.Item().Text("Firma del Médico Tratante");
                                });
                            });

                            column.Item().PaddingTop(1, Unit.Centimetre).AlignCenter().Text($"Fecha de Emisión: {DateTime.Now:dd/MM/yyyy HH:mm}");
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Documento confidencial - Hospital Padre Hurtado        ");
                            x.CurrentPageNumber();
                        });
                });
            });

            // Generar PDF como stream
            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            var fileContent = stream.ToArray();

            // Descargar el PDF
            await DescargarArchivo(fileContent, $"Consentimiento_{Paciente.rut}_{DateTime.Now:yyyyMMdd}.pdf");
        }

        private async Task DescargarArchivo(byte[] fileContent, string fileName)
        {
            var contentType = "application/pdf";
            await JSRuntime.InvokeVoidAsync("descargarArchivo",
                fileName,
                contentType,
                Convert.ToBase64String(fileContent));
        }
    }
}

