using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooker.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReleaseDateNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Room",
                type: "datetime2",
                nullable: true, // Cho phép NULL
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Room",
                type: "datetime2",
                nullable: false, // Khôi phục lại không cho phép NULL
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

    }
}
