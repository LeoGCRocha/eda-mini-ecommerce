using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orders.Infra.Migrations.Data
{
    /// <inheritdoc />
    public partial class SagaStateCanBeNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<object>(
                name: "state_data",
                schema: "orders",
                table: "saga_entity",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(object),
                oldType: "jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<object>(
                name: "state_data",
                schema: "orders",
                table: "saga_entity",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(object),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}
