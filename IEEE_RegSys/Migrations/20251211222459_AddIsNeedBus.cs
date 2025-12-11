using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IEEE_RegSys.Migrations
{
    /// <inheritdoc />
    public partial class AddIsNeedBus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNeedBus",
                table: "Attendees",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNeedBus",
                table: "Attendees");
        }
    }
}
