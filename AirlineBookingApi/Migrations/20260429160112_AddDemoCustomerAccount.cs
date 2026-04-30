using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDemoCustomerAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$V8uj4i.kgHj7de5nIOw8Uecocdz05pMroStyoy5pXKYWTINmNlsEW");

            // Idempotent insert for demo customer account (Id = 2).
            // Skip if Id=2 already exists or if the email was already created via API with a different Id.
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [Accounts] WHERE [Id] = 2)
   AND NOT EXISTS (SELECT 1 FROM [Accounts] WHERE [Email] = N'customer@airline.com')
BEGIN
    SET IDENTITY_INSERT [Accounts] ON;
    INSERT INTO [Accounts] ([Id], [CreatedAt], [Email], [IsActive], [PasswordHash], [RoleId], [UpdatedAt])
    VALUES (2, '2026-01-01T00:00:00', N'customer@airline.com', 1, '$2a$11$jbGC1/QdSVzglrwxS/q3neeL8jmIynxW7YzQkFF7pkt4mADG67EeC', 1, NULL);
    SET IDENTITY_INSERT [Accounts] OFF;
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [Profiles] WHERE [Id] = 2)
   AND NOT EXISTS (SELECT 1 FROM [Profiles] WHERE [AccountId] = 2)
BEGIN
    SET IDENTITY_INSERT [Profiles] ON;
    INSERT INTO [Profiles] ([Id], [AccountId], [Address], [CreatedAt], [DateOfBirth], [FullName], [PhoneNumber], [UpdatedAt])
    VALUES (2, 2, N'Da Nang', '2026-01-01T00:00:00', NULL, N'Khach Hang Demo', N'0900000002', NULL);
    SET IDENTITY_INSERT [Profiles] OFF;
END
");

            // Reseed IDENTITY so the next auto-generated Id matches MAX(Id) of the table.
            migrationBuilder.Sql(@"DBCC CHECKIDENT('[Accounts]', RESEED);");
            migrationBuilder.Sql(@"DBCC CHECKIDENT('[Profiles]', RESEED);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM [Profiles] WHERE [Id] = 2)
    DELETE FROM [Profiles] WHERE [Id] = 2;
");

            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM [Accounts] WHERE [Id] = 2)
    DELETE FROM [Accounts] WHERE [Id] = 2;
");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$K5e5YxQJZuNJfIHqVp7nNuYXlQwMpT7a3OKxh1G9c8mN4vW5rJZyK");
        }
    }
}
