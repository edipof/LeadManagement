using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeadManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addJobId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JobId",
                table: "Leads",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Leads");
        }
    }
}
