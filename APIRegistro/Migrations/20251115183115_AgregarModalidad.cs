using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIRegistro.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModalidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Modalidad",
                table: "Alumno",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Modalidad",
                table: "Alumno");
        }
    }
}
