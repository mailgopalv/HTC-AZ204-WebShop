using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Contoso.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOrderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAddress",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Orders",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Orders",
                newName: "UserName");

            migrationBuilder.AddColumn<string>(
                name: "UserAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
