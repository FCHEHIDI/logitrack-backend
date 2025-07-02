using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogiTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderIdToInventoryItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemList",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "InventoryItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_OrderId",
                table: "InventoryItems",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Orders_OrderId",
                table: "InventoryItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Orders_OrderId",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_OrderId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "InventoryItems");

            migrationBuilder.AddColumn<string>(
                name: "ItemList",
                table: "Orders",
                type: "TEXT",
                nullable: true);
        }
    }
}
