using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KrizzShop_DataAccess.Migrations
{
    public partial class addOrderDetailsAndHeaders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InquiryDetail_InquiryHeaders_InquiryHeaderId",
                table: "InquiryDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_InquiryDetail_Products_ProductId",
                table: "InquiryDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InquiryDetail",
                table: "InquiryDetail");

            migrationBuilder.RenameTable(
                name: "InquiryDetail",
                newName: "InquiryDetails");

            migrationBuilder.RenameIndex(
                name: "IX_InquiryDetail_ProductId",
                table: "InquiryDetails",
                newName: "IX_InquiryDetails_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_InquiryDetail_InquiryHeaderId",
                table: "InquiryDetails",
                newName: "IX_InquiryDetails_InquiryHeaderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InquiryDetails",
                table: "InquiryDetails",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "OrderHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShippingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinalOrderTotal = table.Column<double>(type: "float", nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentDueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderHeaders_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderHeaderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PricePerPiece = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_OrderHeaders_OrderHeaderId",
                        column: x => x.OrderHeaderId,
                        principalTable: "OrderHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderHeaderId",
                table: "OrderDetails",
                column: "OrderHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_CreatedByUserId",
                table: "OrderHeaders",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InquiryDetails_InquiryHeaders_InquiryHeaderId",
                table: "InquiryDetails",
                column: "InquiryHeaderId",
                principalTable: "InquiryHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InquiryDetails_Products_ProductId",
                table: "InquiryDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InquiryDetails_InquiryHeaders_InquiryHeaderId",
                table: "InquiryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InquiryDetails_Products_ProductId",
                table: "InquiryDetails");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "OrderHeaders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InquiryDetails",
                table: "InquiryDetails");

            migrationBuilder.RenameTable(
                name: "InquiryDetails",
                newName: "InquiryDetail");

            migrationBuilder.RenameIndex(
                name: "IX_InquiryDetails_ProductId",
                table: "InquiryDetail",
                newName: "IX_InquiryDetail_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_InquiryDetails_InquiryHeaderId",
                table: "InquiryDetail",
                newName: "IX_InquiryDetail_InquiryHeaderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InquiryDetail",
                table: "InquiryDetail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InquiryDetail_InquiryHeaders_InquiryHeaderId",
                table: "InquiryDetail",
                column: "InquiryHeaderId",
                principalTable: "InquiryHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InquiryDetail_Products_ProductId",
                table: "InquiryDetail",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
