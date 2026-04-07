using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientDocumentPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClientIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "client_id",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_client_id",
                table: "AspNetUsers",
                column: "client_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_client_client_id",
                table: "AspNetUsers",
                column: "client_id",
                principalTable: "client",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_client_client_id",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_client_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "client_id",
                table: "AspNetUsers");
        }
    }
}
