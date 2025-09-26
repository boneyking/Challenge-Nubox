using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Migrations
{
    /// <inheritdoc />
    public partial class AddAsistenciaEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionesPartner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompaniaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombrePartner = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    TipoIntegracionPreferido = table.Column<int>(type: "int", nullable: false),
                    IntervaloSincronizacion = table.Column<TimeSpan>(type: "time", nullable: false),
                    UltimaSincronizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProximaSincronizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SincronizacionAutomatica = table.Column<bool>(type: "bit", nullable: false),
                    NotificarErrores = table.Column<bool>(type: "bit", nullable: false),
                    EmailNotificaciones = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TimeoutSegundos = table.Column<int>(type: "int", nullable: false),
                    MaxReintentos = table.Column<int>(type: "int", nullable: false),
                    TamañoLoteMaximo = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesPartner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracionesPartner_Companias_CompaniaId",
                        column: x => x.CompaniaId,
                        principalTable: "Companias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Trabajadores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompaniaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Dni = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EsActivo = table.Column<bool>(type: "bit", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaSalida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdExternoPartner = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UltimaSincronizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trabajadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trabajadores_Companias_CompaniaId",
                        column: x => x.CompaniaId,
                        principalTable: "Companias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransaccionesSincronizacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompaniaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoIntegracion = table.Column<int>(type: "int", nullable: false),
                    SistemaOrigen = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaPeriodo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegistrosProcesados = table.Column<int>(type: "int", nullable: false),
                    RegistrosTotales = table.Column<int>(type: "int", nullable: false),
                    RegistrosErrores = table.Column<int>(type: "int", nullable: false),
                    DetalleRequest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetalleResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MensajeError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DuracionMs = table.Column<long>(type: "bigint", nullable: false),
                    TamañoArchivoMB = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ProcesadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransaccionesSincronizacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransaccionesSincronizacion_Companias_CompaniaId",
                        column: x => x.CompaniaId,
                        principalTable: "Companias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LicenciasMedicas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrabajadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoLicencia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumeroLicencia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Diagnostico = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EsAprobada = table.Column<bool>(type: "bit", nullable: false),
                    IdExternoPartner = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenciasMedicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LicenciasMedicas_Trabajadores_TrabajadorId",
                        column: x => x.TrabajadorId,
                        principalTable: "Trabajadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosAsistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrabajadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraEntrada = table.Column<TimeSpan>(type: "time", nullable: true),
                    HoraSalida = table.Column<TimeSpan>(type: "time", nullable: true),
                    HorasRegulares = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    HorasExtras = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdExternoPartner = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaRecepcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosAsistencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosAsistencia_Trabajadores_TrabajadorId",
                        column: x => x.TrabajadorId,
                        principalTable: "Trabajadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResumenesAsistencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrabajadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Año = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    Semana = table.Column<int>(type: "int", nullable: true),
                    Quincena = table.Column<int>(type: "int", nullable: true),
                    TotalHorasRegulares = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    TotalHorasExtras = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    DiasAsistencia = table.Column<int>(type: "int", nullable: false),
                    DiasInasistencia = table.Column<int>(type: "int", nullable: false),
                    DiasTardanza = table.Column<int>(type: "int", nullable: false),
                    DiasLicencia = table.Column<int>(type: "int", nullable: false),
                    DiasVacaciones = table.Column<int>(type: "int", nullable: false),
                    EsProcesado = table.Column<bool>(type: "bit", nullable: false),
                    FechaProcesamiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LiquidacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumenesAsistencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumenesAsistencia_Trabajadores_TrabajadorId",
                        column: x => x.TrabajadorId,
                        principalTable: "Trabajadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesPartner_CompaniaId",
                table: "ConfiguracionesPartner",
                column: "CompaniaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LicenciasMedicas_NumeroLicencia",
                table: "LicenciasMedicas",
                column: "NumeroLicencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LicenciasMedicas_TrabajadorPeriodo",
                table: "LicenciasMedicas",
                columns: new[] { "TrabajadorId", "FechaInicio", "FechaFin" });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAsistencia_FechaEstado",
                table: "RegistrosAsistencia",
                columns: new[] { "Fecha", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAsistencia_IdExternoPartner",
                table: "RegistrosAsistencia",
                column: "IdExternoPartner");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAsistencia_TrabajadorFecha",
                table: "RegistrosAsistencia",
                columns: new[] { "TrabajadorId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_ResumenesAsistencia_ProcesadoPeriodo",
                table: "ResumenesAsistencia",
                columns: new[] { "EsProcesado", "Año", "Mes" });

            migrationBuilder.CreateIndex(
                name: "IX_ResumenesAsistencia_TrabajadorPeriodo",
                table: "ResumenesAsistencia",
                columns: new[] { "TrabajadorId", "Año", "Mes", "Semana", "Quincena" },
                unique: true,
                filter: "[Semana] IS NOT NULL AND [Quincena] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Trabajadores_CompaniaDni",
                table: "Trabajadores",
                columns: new[] { "CompaniaId", "Dni" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trabajadores_IdExternoPartner",
                table: "Trabajadores",
                column: "IdExternoPartner");

            migrationBuilder.CreateIndex(
                name: "IX_TransaccionesSincronizacion_CompaniaFecha",
                table: "TransaccionesSincronizacion",
                columns: new[] { "CompaniaId", "FechaInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_TransaccionesSincronizacion_EstadoFecha",
                table: "TransaccionesSincronizacion",
                columns: new[] { "Estado", "FechaInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_TransaccionesSincronizacion_TransactionId",
                table: "TransaccionesSincronizacion",
                column: "TransactionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionesPartner");

            migrationBuilder.DropTable(
                name: "LicenciasMedicas");

            migrationBuilder.DropTable(
                name: "RegistrosAsistencia");

            migrationBuilder.DropTable(
                name: "ResumenesAsistencia");

            migrationBuilder.DropTable(
                name: "TransaccionesSincronizacion");

            migrationBuilder.DropTable(
                name: "Trabajadores");
        }
    }
}
