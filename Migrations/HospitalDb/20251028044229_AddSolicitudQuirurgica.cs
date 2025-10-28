using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace proyecto_hospital_version_1.Migrations.HospitalDb
{
    /// <inheritdoc />
    public partial class AddSolicitudQuirurgica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "SolicitudesQuirurgicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteId = table.Column<int>(type: "int", nullable: false),
                    DiagnosticoPrincipal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoCie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcedimientoPrincipal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Procedencia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsGes = table.Column<bool>(type: "bit", nullable: false),
                    EspecialidadOrigen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EspecialidadDestino = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Peso = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Talla = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IMC = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EquiposRequeridos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoMesa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EvaluacionAnestesica = table.Column<bool>(type: "bit", nullable: false),
                    Transfusiones = table.Column<bool>(type: "bit", nullable: false),
                    SalaOperaciones = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TiempoEstimado = table.Column<int>(type: "int", nullable: true),
                    Comorbilidades = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComentariosAdicionales = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prioridad = table.Column<int>(type: "int", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesQuirurgicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesQuirurgicas_Paciente_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Paciente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });






        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesQuirurgicas");
        }
    }
}
