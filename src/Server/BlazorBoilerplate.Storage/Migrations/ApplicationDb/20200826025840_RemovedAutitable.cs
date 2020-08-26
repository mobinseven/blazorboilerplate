using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlazorBoilerplate.Storage.Migrations.ApplicationDb
{
    public partial class RemovedAutitable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_AspNetUsers_CreatedById",
                table: "Todos");

            migrationBuilder.DropForeignKey(
                name: "FK_Todos_AspNetUsers_ModifiedById",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_CreatedById",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_ModifiedById",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Todos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Todos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Todos",
                type: "datetime2(7)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById",
                table: "Todos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Todos",
                type: "datetime2(7)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Todos_CreatedById",
                table: "Todos",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_ModifiedById",
                table: "Todos",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_AspNetUsers_CreatedById",
                table: "Todos",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_AspNetUsers_ModifiedById",
                table: "Todos",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
