using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Banking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Details = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BankId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    HmacSecretKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VerificationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BankId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    IdentityUserId = table.Column<string>(type: "text", nullable: true),
                    IdentityPayloadJson = table.Column<string>(type: "text", nullable: true),
                    QrSignature = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationRequests", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BankClients",
                columns: new[] { "Id", "BankId", "CreatedAt", "HmacSecretKey", "IsActive", "Name" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "BANK-001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "shared-hmac-secret-key-change-in-production", true, "SecureFill National Bank" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BankClients_BankId",
                table: "BankClients",
                column: "BankId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VerificationRequests_BankId",
                table: "VerificationRequests",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationRequests_ExpiresAt",
                table: "VerificationRequests",
                column: "ExpiresAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BankClients");

            migrationBuilder.DropTable(
                name: "VerificationRequests");
        }
    }
}
