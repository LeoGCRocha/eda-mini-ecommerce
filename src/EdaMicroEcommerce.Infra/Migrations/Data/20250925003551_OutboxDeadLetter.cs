using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdaMicroEcommerce.Infra.Migrations.Data
{
    /// <inheritdoc />
    public partial class OutboxDeadLetter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_dead_letter",
                table: "outbox_integration_events",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_dead_letter",
                table: "outbox_integration_events");
        }
    }
}
