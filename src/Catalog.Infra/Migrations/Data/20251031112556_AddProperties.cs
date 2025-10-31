using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infra.Migrations.Data
{
    /// <inheritdoc />
    public partial class AddProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "occured_at_utc",
                schema: "catalog",
                table: "reservations",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                schema: "catalog",
                table: "reservations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "occured_at_utc",
                schema: "catalog",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "quantity",
                schema: "catalog",
                table: "reservations");
        }
    }
}
