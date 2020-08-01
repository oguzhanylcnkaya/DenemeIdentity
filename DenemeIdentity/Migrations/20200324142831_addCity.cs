using Microsoft.EntityFrameworkCore.Migrations;

namespace DenemeIdentity.Migrations
{
    public partial class addCity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "city",
                table: "AspNetUsers");
        }
    }
}
