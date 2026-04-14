using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IEEE_RegSys.Migrations
{
    /// <inheritdoc />
    public partial class AddPromocodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PromoCodeId",
                table: "Attendees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PromoCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountPercentage = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UsageLimit = table.Column<int>(type: "int", nullable: false),
                    UsedCount = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendees_PromoCodeId",
                table: "Attendees",
                column: "PromoCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendees_PromoCodes_PromoCodeId",
                table: "Attendees",
                column: "PromoCodeId",
                principalTable: "PromoCodes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendees_PromoCodes_PromoCodeId",
                table: "Attendees");

            migrationBuilder.DropTable(
                name: "PromoCodes");

            migrationBuilder.DropIndex(
                name: "IX_Attendees_PromoCodeId",
                table: "Attendees");

            migrationBuilder.DropColumn(
                name: "PromoCodeId",
                table: "Attendees");
        }
    }
}
