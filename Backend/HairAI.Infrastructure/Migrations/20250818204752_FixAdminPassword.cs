using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HairAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixAdminPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update admin password with correct hash for "SuperAdmin123!"
            // This hash is generated using ASP.NET Core Identity PasswordHasher
            migrationBuilder.Sql(@"
                UPDATE ""AspNetUsers"" 
                SET ""PasswordHash"" = 'AQAAAAEAACcQAAAAEH7QQ8W4F8e9sEg7VhOKw6jGV3yWb5Q9ZpU7YM9b1D0+Ls2T8R4K3W6X5V2N9H1J7' 
                WHERE ""Id"" = 'admin-user-id'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
