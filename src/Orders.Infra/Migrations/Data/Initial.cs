using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Orders.Infra.Migrations.Data
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "orders");

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<string>(type: "text", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    currency = table.Column<string>(type: "text", nullable: false, defaultValue: "BRL"),
                    discount_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    net_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    payment_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_integration_events",
                schema: "orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<int>(type: "integer", nullable: false),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    retry_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    is_dead_letter = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_integration_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "order_items",
                schema: "orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_items_orders_order_id",
                        column: x => x.order_id,
                        principalSchema: "orders",
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_order_items_order_id",
                schema: "orders",
                table: "order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_items_product_id",
                schema: "orders",
                table: "order_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_customer_id",
                schema: "orders",
                table: "orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_payment_date",
                schema: "orders",
                table: "orders",
                column: "payment_date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_items",
                schema: "orders");

            migrationBuilder.DropTable(
                name: "outbox_integration_events",
                schema: "orders");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "orders");
        }
    }
}
