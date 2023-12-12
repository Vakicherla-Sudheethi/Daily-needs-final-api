using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyNeeds1.Migrations
{
    /// <inheritdoc />
    public partial class NewImagePic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploadImg",
                table: "products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadImg",
                table: "products");
        }
    }
}
