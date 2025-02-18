using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedProjectLeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Lägg till en "borttagen" projektledare
            migrationBuilder.InsertData(
                table: "ProjectLeaders",
                columns: new[] { "ProjectLeaderID", "FirstName", "LastName", "Email", "Phone", "Department" },
                values: new object[] { -1, "Användare", "Borttagen", "noreply@domain.com", "0000000000", "Ej tilldelad" }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Ta bort den "borttagna" projektledaren
            migrationBuilder.DeleteData(
                table: "ProjectLeaders",
                keyColumn: "ProjectLeaderID",
                keyValue: -1
            );
        }
    }

}
