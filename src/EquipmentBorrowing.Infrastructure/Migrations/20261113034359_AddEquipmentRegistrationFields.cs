using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipmentBorrowing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentRegistrationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Equipment",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            // Add the identifier temporarily as nullable
            migrationBuilder.AddColumn<string>(
                name: "EquipmentIdentifier",
                table: "Equipment",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            // Give existing equipment records unique legacy identifiers
            migrationBuilder.Sql("""
        UPDATE "Equipment"
        SET "EquipmentIdentifier" =
            'LEGACY-' || REPLACE("EquipmentId"::text, '-', '');
        """);

            // Make the identifier required
            migrationBuilder.AlterColumn<string>(
                name: "EquipmentIdentifier",
                table: "Equipment",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            // Enforce uniqueness
            migrationBuilder.CreateIndex(
                name: "IX_Equipment_EquipmentIdentifier",
                table: "Equipment",
                column: "EquipmentIdentifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Equipment_EquipmentIdentifier",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "EquipmentIdentifier",
                table: "Equipment");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Equipment",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
