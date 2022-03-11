using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortalOgloszeniowy.Migrations
{
    public partial class _013 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isPremium",
                table: "Adverts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isPremium",
                table: "Adverts");
        }
    }
}
