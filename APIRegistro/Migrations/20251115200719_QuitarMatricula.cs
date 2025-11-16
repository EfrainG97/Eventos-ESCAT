using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIRegistro.Migrations
{
    /// <inheritdoc />
    public partial class QuitarMatricula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Matricula",
                table: "Alumno");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Matricula",
                table: "Alumno",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
