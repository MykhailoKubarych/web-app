using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebShop.Api.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: false),
                    role = table.Column<string>(nullable: false),
                    salt = table.Column<string>(nullable: false),
                    password_hash = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
