using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class addUserType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserTypeId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserTypeId",
                table: "AspNetUsers");
        }
    }
}
