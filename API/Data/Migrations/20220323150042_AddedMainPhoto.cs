using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    public partial class AddedMainPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photo_Items_ItemId",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "IsPreview",
                table: "Photo");

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "Photo",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "PreviewPhotoId",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_PreviewPhotoId",
                table: "Items",
                column: "PreviewPhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Photo_PreviewPhotoId",
                table: "Items",
                column: "PreviewPhotoId",
                principalTable: "Photo",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Photo_Items_ItemId",
                table: "Photo",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Photo_PreviewPhotoId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Photo_Items_ItemId",
                table: "Photo");

            migrationBuilder.DropIndex(
                name: "IX_Items_PreviewPhotoId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PreviewPhotoId",
                table: "Items");

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "Photo",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPreview",
                table: "Photo",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Photo_Items_ItemId",
                table: "Photo",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
