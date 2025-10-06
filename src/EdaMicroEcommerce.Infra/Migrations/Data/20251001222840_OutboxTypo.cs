using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdaMicroEcommerce.Infra.Migrations.Data
{
    /// <inheritdoc />
    public partial class OutboxTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE outbox_integration_events
                ALTER COLUMN type TYPE integer
                USING type::integer;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "outbox_integration_events",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
