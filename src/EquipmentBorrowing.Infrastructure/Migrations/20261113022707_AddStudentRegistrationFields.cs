using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipmentBorrowing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentRegistrationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactInformation",
                table: "Students",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstitutionalNumber",
                table: "Students",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.Sql(
                """
        UPDATE "Students"
        SET "InstitutionalNumber" = 'LEGACY-' || "Id"::text
        WHERE "InstitutionalNumber" IS NULL;
        """);

            migrationBuilder.AlterColumn<string>(
                name: "InstitutionalNumber",
                table: "Students",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_InstitutionalNumber",
                table: "Students",
                column: "InstitutionalNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Students_InstitutionalNumber",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ContactInformation",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "InstitutionalNumber",
                table: "Students");
        }
    }
}
