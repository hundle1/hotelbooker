using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooker.Migrations
{
    /// <inheritdoc />
    public partial class MergeUserContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hotel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HotelName = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    HotelImage = table.Column<string>(type: "TEXT", nullable: true),
                    HotelDescription = table.Column<string>(type: "TEXT", nullable: true),
                    HotelLocation = table.Column<string>(type: "TEXT", nullable: true),
                    HotelRate = table.Column<int>(type: "INTEGER", nullable: true),
                    NumberOfRoom = table.Column<int>(type: "INTEGER", nullable: true),
                    HotelServices = table.Column<string>(type: "TEXT", nullable: true),
                    HotelContact = table.Column<string>(type: "TEXT", nullable: true),
                    HotelEmail = table.Column<string>(type: "TEXT", nullable: true),
                    HotelWebsite = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomName = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Status = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Services = table.Column<string>(type: "TEXT", nullable: true),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    RoomImage = table.Column<string>(type: "TEXT", nullable: true),
                    Furniture = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Discount = table.Column<bool>(type: "INTEGER", nullable: false),
                    DiscountStart = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DiscountEnd = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DiscountPercent = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hotel");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
