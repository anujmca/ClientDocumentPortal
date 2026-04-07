using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientDocumentPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUrlSlugToTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.AddColumn<string>(
                name: "url_slug",
                table: "tenant",
                type: "text",
                nullable: false,
                defaultValue: "");

            // Set url_slug for existing tenants using a safe version of their name + their ID suffix to ensure uniqueness
            migrationBuilder.Sql("UPDATE tenant SET url_slug = lower(regexp_replace(name, '[^a-zA-Z0-9]', '', 'g')) || '-' || substring(id::text, 1, 8) WHERE url_slug = '';");

            migrationBuilder.CreateIndex(
                name: "ix_tenant_url_slug",
                table: "tenant",
                column: "url_slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tenant_url_slug",
                table: "tenant");

            migrationBuilder.DropColumn(
                name: "url_slug",
                table: "tenant");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
        }
    }
}
