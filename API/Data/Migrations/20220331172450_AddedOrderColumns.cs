using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Data.Migrations
{
    public partial class AddedOrderColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WhenWillBeAvaliable",
                table: "Units",
                newName: "WhenWillBeAvailable");

            migrationBuilder.RenameColumn(
                name: "IsAvaliable",
                table: "Units",
                newName: "IsAvailable");

            migrationBuilder.AlterColumn<bool>(
                name: "Disabled",
                table: "Locations",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Locations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Locations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "DeliverymanRequest",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UnitId = table.Column<int>(type: "integer", nullable: true),
                    ClientId = table.Column<int>(type: "integer", nullable: true),
                    DeliveryManId = table.Column<int>(type: "integer", nullable: true),
                    DeliveryLocationId = table.Column<int>(type: "integer", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeliveryInProcess = table.Column<bool>(type: "boolean", nullable: false),
                    Cancelled = table.Column<bool>(type: "boolean", nullable: false),
                    DeliveryCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    ClientGotDelivery = table.Column<bool>(type: "boolean", nullable: false),
                    InUsage = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_ClientId",
                        column: x => x.ClientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_DeliveryManId",
                        column: x => x.DeliveryManId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Locations_DeliveryLocationId",
                        column: x => x.DeliveryLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_AppUserId",
                table: "Locations",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LocationId",
                table: "AspNetUsers",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientId",
                table: "Orders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryLocationId",
                table: "Orders",
                column: "DeliveryLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryManId",
                table: "Orders",
                column: "DeliveryManId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UnitId",
                table: "Orders",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Locations_LocationId",
                table: "AspNetUsers",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_AspNetUsers_AppUserId",
                table: "Locations",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Locations_LocationId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_AspNetUsers_AppUserId",
                table: "Locations");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Locations_AppUserId",
                table: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LocationId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "DeliverymanRequest",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "WhenWillBeAvailable",
                table: "Units",
                newName: "WhenWillBeAvaliable");

            migrationBuilder.RenameColumn(
                name: "IsAvailable",
                table: "Units",
                newName: "IsAvaliable");

            migrationBuilder.AlterColumn<bool>(
                name: "Disabled",
                table: "Locations",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);
        }
    }
}
