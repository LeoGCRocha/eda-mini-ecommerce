using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Catalog.Infra.Migrations.Data
{
    /// <inheritdoc />
    public partial class InitialCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalog");

            migrationBuilder.CreateTable(
                name: "outbox_integration_events",
                schema: "catalog",
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
                name: "products",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    base_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "TRUE"),
                    created_at_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inventory_items",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    available_quantity = table.Column<int>(type: "integer", nullable: false),
                    reserved_quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    reorder_level = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventory_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_inventory_items_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "catalog",
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservations",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    occured_at_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    inventory_item_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservations", x => x.id);
                    table.ForeignKey(
                        name: "fk_reservations_inventory_items_inventory_item_id",
                        column: x => x.inventory_item_id,
                        principalSchema: "catalog",
                        principalTable: "inventory_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_inventory_items_product_id",
                schema: "catalog",
                table: "inventory_items",
                column: "product_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_products_is_active",
                schema: "catalog",
                table: "products",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_products_name",
                schema: "catalog",
                table: "products",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_reservations_inventory_item_id",
                schema: "catalog",
                table: "reservations",
                column: "inventory_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservations_order_id",
                schema: "catalog",
                table: "reservations",
                column: "order_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_integration_events",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "reservations",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "inventory_items",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "products",
                schema: "catalog");
        }
    }
}
