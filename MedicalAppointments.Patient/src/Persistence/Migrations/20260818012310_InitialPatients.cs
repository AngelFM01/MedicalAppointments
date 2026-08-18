using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialPatients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "patients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    document_number = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    sex = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    marital_status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patients", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_patients_document_number",
                table: "patients",
                column: "document_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patients");
        }
    }
}
