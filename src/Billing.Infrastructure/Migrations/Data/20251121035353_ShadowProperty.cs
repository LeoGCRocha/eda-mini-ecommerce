using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Infrastructure.Migrations.Data
{
    /// <inheritdoc />
    public partial class ShadowProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "row_version",
                schema: "billing",
                table: "payments");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "billing",
                table: "payments",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateIndex(
                name: "ix_payments_order_id",
                schema: "billing",
                table: "payments",
                column: "order_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_payments_order_id",
                schema: "billing",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "billing",
                table: "payments");

            migrationBuilder.AddColumn<byte[]>(
                name: "row_version",
                schema: "billing",
                table: "payments",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
