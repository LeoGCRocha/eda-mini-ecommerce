using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orders.Infra.Migrations.Data
{
    /// <inheritdoc />
    public partial class OrdersAdapt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "state_data",
                schema: "orders",
                table: "saga_entity");

            migrationBuilder.AddColumn<int>(
                name: "reservation_status",
                schema: "orders",
                table: "order_items",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reservation_status",
                schema: "orders",
                table: "order_items");

            migrationBuilder.AddColumn<object>(
                name: "state_data",
                schema: "orders",
                table: "saga_entity",
                type: "jsonb",
                nullable: true);
        }
    }
}
