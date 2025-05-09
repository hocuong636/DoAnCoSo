using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CT_Fas.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCartAndCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Carts");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Carts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<bool>(
                name: "IsOrdered",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_SizeId",
                table: "CartItems",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_ProductSizes_SizeId",
                table: "CartItems",
                column: "SizeId",
                principalTable: "ProductSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_ProductSizes_SizeId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_SizeId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "IsOrdered",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "CartItems");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Carts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
