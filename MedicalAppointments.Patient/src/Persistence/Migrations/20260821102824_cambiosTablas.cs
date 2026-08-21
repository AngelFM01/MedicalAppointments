using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class cambiosTablas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "fechaCreacion",
                table: "EmergencyContacts",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "nombreComleto",
                table: "EmergencyContacts",
                newName: "NombreCompleto");

            migrationBuilder.RenameColumn(
                name: "Id_CE",
                table: "EmergencyContacts",
                newName: "IdContactEmerg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "EmergencyContacts",
                newName: "fechaCreacion");

            migrationBuilder.RenameColumn(
                name: "NombreCompleto",
                table: "EmergencyContacts",
                newName: "nombreComleto");

            migrationBuilder.RenameColumn(
                name: "IdContactEmerg",
                table: "EmergencyContacts",
                newName: "Id_CE");
        }
    }
}
