using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System;
using System.Threading.Tasks;
using proyecto_hospital_version_1.Data._Legacy; // Añadir esto si no está

using Hospital.Api.DTOs;

namespace proyecto_hospital_version_1.Components.Shared
{
    public partial class ConsentimientoPDF
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        // Aseguramos inicialización para evitar nulls, aunque Blazor inyecta si están en el padre.
        [Parameter]
        public PacienteDto? Paciente { get; set; } // Inicializar para evitar NRE al acceder a propiedades si es null
        // ******* NOTA: Este Paciente = new PacienteHospital() es un truco de inicialización.
        // ******* Si Paciente real es null en el padre, se seguirá usando la instancia default.
        // ******* El check "Paciente == null" DEBE usarse.

        [Parameter]
        public string Procedimiento { get; set; } = string.Empty;

        [Parameter]
        public string Lateralidad { get; set; } = string.Empty;

        [Parameter]
        public string Extremidad { get; set; } = string.Empty;

        // ** AÑADIR ESTE CICLO DE VIDA PARA DEBUGGING **
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Console.WriteLine($"[ConsentimientoPDF.OnParametersSet] Recibido Paciente: {Paciente?.NombreCompleto ?? "NULL"}");
            Console.WriteLine($"[ConsentimientoPDF.OnParametersSet] Recibido Procedimiento: {Procedimiento ?? "NULL"}");
            Console.WriteLine($"[ConsentimientoPDF.OnParametersSet] Recibido Lateralidad: {Lateralidad ?? "NULL"}");
            Console.WriteLine($"[ConsentimientoPDF.OnParametersSet] Recibido Extremidad: {Extremidad ?? "NULL"}");
        }


        public async Task GenerarPdf()
        {
            Console.WriteLine("[ConsentimientoPDF.GenerarPdf] Método de generación de PDF invocado.");

            // Volvemos a loguear por si los parámetros cambiaron entre OnParametersSet y esta llamada
            Console.WriteLine($"[ConsentimientoPDF.GenerarPdf] DEBUG: Paciente (en GenerarPdf): {Paciente?.NombreCompleto ?? "NULL"}");
            Console.WriteLine($"[ConsentimientoPDF.GenerarPdf] DEBUG: Procedimiento (en GenerarPdf): {Procedimiento ?? "NULL"}");

            if (Paciente == null || string.IsNullOrWhiteSpace(Paciente.Rut)) // Añadir rut como criterio básico
            {
                await JSRuntime.InvokeVoidAsync("alert", "Error: No hay datos válidos del paciente para generar el PDF. Por favor, asegúrese de seleccionar un paciente.");
                Console.Error.WriteLine("[ConsentimientoPDF.GenerarPdf] ERROR: Paciente es nulo o no tiene RUT.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Procedimiento))
            {
                await JSRuntime.InvokeVoidAsync("alert", "Error: No se ha especificado un procedimiento para generar el PDF.");
                Console.Error.WriteLine("[ConsentimientoPDF.GenerarPdf] ERROR: Procedimiento es nulo o vacío.");
                return;
            }


            // Configurar QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;

            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header()
                            .AlignCenter()
                            .Text("Resumen de Solicitud y Consentimiento")
                            .SemiBold().FontSize(18).FontColor(Colors.Black);

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                column.Spacing(20);

                                column.Item().Column(infoColumn =>
                                {
                                    infoColumn.Item().PaddingBottom(5).Text("1. Datos del Paciente").FontSize(14).SemiBold();
                                    infoColumn.Item().Grid(grid =>
                                    {
                                        grid.Columns(2);
                                        grid.Item().Text($"Nombre:").Bold();
                                        grid.Item().Text($"{Paciente.NombreCompleto ?? "N/A"}"); // Uso de ?? "N/A" para seguridad

                                        grid.Item().Text($"RUT:").Bold();
                                        grid.Item().Text($"{Paciente.Rut ?? "N/A"}-{Paciente.Dv ?? "N/A"}");

                                        grid.Item().Text($"Edad:").Bold();
                                        grid.Item().Text($"{Paciente.EdadCompleta ?? "N/A"}"); // Asumiendo que EdadCompleta es string o tiene ToString
                                                                                               // Si es int, Paciente.EdadCompleta.ToString()
                                        grid.Item().Text($"Sexo:").Bold();
                                        grid.Item().Text($"{Paciente.Sexo ?? "N/A"}");
                                    });
                                });

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
                                    consentColumn.Item().PaddingBottom(5).Text("3. Consentimiento").FontSize(14).SemiBold();

                                    consentColumn.Item().Text(txt =>
                                    {
                                        txt.Span("Yo, ");
                                        txt.Span($"{Paciente.NombreCompleto ?? "N/A"}").Bold();
                                        txt.Span(", RUT ");
                                        txt.Span($"{Paciente.Rut ?? "N/A"}-{Paciente.Dv ?? "N/A"}").Bold();
                                        txt.Span(", en pleno uso de mis facultades, declaro que he sido informado(a) y doy mi consentimiento para la realización del procedimiento: ");
                                        txt.Span($"'{Procedimiento}'").Bold();
                                        txt.Span(".");
                                    });

                                    consentColumn.Item().PaddingTop(10).Text("Comprendo los riesgos, beneficios y alternativas, y he tenido la oportunidad de aclarar todas mis dudas con el equipo médico.");
                                });

                                column.Item().PaddingTop(3, Unit.Centimetre).Grid(grid =>
                                {
                                    grid.Columns(2);
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
                                x.Span("Documento confidencial - Hospital Padre Hurtado         ");
                                x.CurrentPageNumber();
                            });
                    });
                });

                Console.WriteLine("[ConsentimientoPDF.GenerarPdf] Iniciando generación de PDF a MemoryStream.");
                using var stream = new MemoryStream();
                document.GeneratePdf(stream);
                var fileContent = stream.ToArray();
                Console.WriteLine($"[ConsentimientoPDF.GenerarPdf] PDF generado. Tamaño: {fileContent.Length} bytes.");

                await DescargarArchivo(fileContent, $"Consentimiento_{Paciente.Rut}_{DateTime.Now:yyyyMMdd}.pdf");
                Console.WriteLine("[ConsentimientoPDF.GenerarPdf] Intento de descarga de archivo JS invocado.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ConsentimientoPDF.GenerarPdf] ERROR FATAL al generar/descargar PDF: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace); // Imprime el stack trace completo
                await JSRuntime.InvokeVoidAsync("alert", $"Error crítico al generar el PDF: {ex.Message}\nConsulte la consola del navegador para más detalles.");
            }
        }

        private async Task DescargarArchivo(byte[] fileContent, string fileName)
        {
            var contentType = "application/pdf";
            try
            {
                await JSRuntime.InvokeVoidAsync("descargarArchivo",
                    fileName,
                    contentType,
                    Convert.ToBase64String(fileContent));
                Console.WriteLine($"[ConsentimientoPDF.DescargarArchivo] Función JS 'descargarArchivo' llamada con éxito para {fileName}.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ConsentimientoPDF.DescargarArchivo] ERROR al invocar JS 'descargarArchivo': {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);
                await JSRuntime.InvokeVoidAsync("alert", $"Error al descargar el archivo: {ex.Message}\nVerifique la función 'descargarArchivo' en JS.");
            }
        }
    }
}