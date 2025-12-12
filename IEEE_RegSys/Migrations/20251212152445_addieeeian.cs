using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IEEE_RegSys.Migrations
{
    /// <inheritdoc />
    public partial class addieeeian : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsIEEEIAN",
                table: "Attendees",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsIEEEIAN",
                table: "Attendees");
        }
    }
}
