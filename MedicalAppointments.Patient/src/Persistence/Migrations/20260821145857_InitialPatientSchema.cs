using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialPatientSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Patients",
                schema: "dbo",
                columns: table => new
                {
                    PacienteID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoPaciente = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    TipoDocumento = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    NumeroDocumento = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    Sexo = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false),
                    EstadoCivil = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    Telefono = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    TelefonoSecundario = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Pais = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Ocupacion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TipoSangre = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.PacienteID);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyContacts",
                schema: "dbo",
                columns: table => new
                {
                    ContactoEmergenciaID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteID = table.Column<long>(type: "bigint", nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Parentesco = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefono = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    TelefonoSecundario = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    Prioridad = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyContacts", x => x.ContactoEmergenciaID);
                    table.ForeignKey(
                        name: "FK_EmergencyContacts_Patients_PacienteID",
                        column: x => x.PacienteID,
                        principalSchema: "dbo",
                        principalTable: "Patients",
                        principalColumn: "PacienteID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContacts_Patient",
                schema: "dbo",
                table: "EmergencyContacts",
                column: "PacienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_CodigoPaciente",
                schema: "dbo",
                table: "Patients",
                column: "CodigoPaciente",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_TipoDocumento_NumeroDocumento",
                schema: "dbo",
                table: "Patients",
                columns: new[] { "TipoDocumento", "NumeroDocumento" },
                unique: true);

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM dbo.Patients) AND NOT EXISTS (SELECT 1 FROM dbo.EmergencyContacts)
                BEGIN
                    INSERT INTO dbo.Patients
                    (CodigoPaciente, TipoDocumento, NumeroDocumento, Nombres, Apellidos,
                     FechaNacimiento, Sexo, EstadoCivil, Telefono, Email, Direccion, Ciudad, Pais, TipoSangre)
                    VALUES
                    ('PAC-0001', 'DUI', '01234567-8', 'María José',   'González López', '1990-04-12', 'F', 'Soltera', '7000-1111', 'maria.gonzalez@mail.com', 'Col. Escalón, Calle Principal #12', 'San Salvador', 'El Salvador', 'O+'),
                    ('PAC-0002', 'DUI', '09876543-2', 'Jorge Alberto', 'Martínez Ruiz',  '1985-11-30', 'M', 'Casado',  '7000-2222', 'jorge.martinez@mail.com', 'Res. Las Flores, Pje. 3 #45',       'Soyapango',    'El Salvador', 'A+');

                    INSERT INTO dbo.EmergencyContacts
                    (PacienteID, NombreCompleto, Parentesco, Telefono, Prioridad) VALUES
                    (1, 'Ana González',   'Madre',  '7000-1112', 1),
                    (1, 'Pedro López',     'Esposo', '7000-1113', 2),
                    (2, 'Rosa Ruiz',       'Esposa', '7000-2223', 1);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmergencyContacts",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Patients",
                schema: "dbo");
        }
    }
}
