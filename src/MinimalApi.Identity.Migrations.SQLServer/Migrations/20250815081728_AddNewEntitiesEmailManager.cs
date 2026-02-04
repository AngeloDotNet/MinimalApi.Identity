using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MinimalApi.Identity.Migrations.SQLServer.Migrations
{
    /// <inheritdoc />
    public partial class AddNewEntitiesEmailManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DropColumn(
                name: "EmailSendingType",
                table: "EmailSending");

            migrationBuilder.DropColumn(
                name: "Sent",
                table: "EmailSending");

            migrationBuilder.RenameColumn(
                name: "ErrorMessage",
                table: "EmailSending",
                newName: "RetrySenderErrorMessage");

            migrationBuilder.RenameColumn(
                name: "ErrorDetails",
                table: "EmailSending",
                newName: "RetrySenderErrorDetails");

            migrationBuilder.AddColumn<int>(
                name: "RetrySender",
                table: "EmailSending",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RetrySenderDate",
                table: "EmailSending",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeEmailSendingId",
                table: "EmailSending",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeEmailStatusId",
                table: "EmailSending",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TypeEmailSending",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeEmailSending", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeEmailStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeEmailStatus", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 2,
                column: "Value",
                value: "FullName");

            migrationBuilder.UpdateData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 3,
                column: "Value",
                value: "License");

            migrationBuilder.UpdateData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 4,
                column: "Value",
                value: "Module");

            migrationBuilder.UpdateData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 5,
                column: "Value",
                value: "Permission");

            migrationBuilder.InsertData(
                table: "TypeEmailSending",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Email sent during user registration", "RegisterUser" },
                    { 2, "Email sent for changing email address", "ChangeEmail" },
                    { 3, "Email sent for password reset requests", "ForgotPassword" }
                });

            migrationBuilder.InsertData(
                table: "TypeEmailStatus",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Email has been sent successfully", "Sent" },
                    { 2, "Email is pending to be sent", "Pending" },
                    { 3, "Email sending failed", "Failed" },
                    { 4, "Email sending has been cancelled", "Cancelled" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailSending_TypeEmailSendingId",
                table: "EmailSending",
                column: "TypeEmailSendingId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailSending_TypeEmailStatusId",
                table: "EmailSending",
                column: "TypeEmailStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailSending_TypeEmailSending_TypeEmailSendingId",
                table: "EmailSending",
                column: "TypeEmailSendingId",
                principalTable: "TypeEmailSending",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailSending_TypeEmailStatus_TypeEmailStatusId",
                table: "EmailSending",
                column: "TypeEmailStatusId",
                principalTable: "TypeEmailStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailSending_TypeEmailSending_TypeEmailSendingId",
                table: "EmailSending");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailSending_TypeEmailStatus_TypeEmailStatusId",
                table: "EmailSending");

            migrationBuilder.DropTable(
                name: "TypeEmailSending");

            migrationBuilder.DropTable(
                name: "TypeEmailStatus");

            migrationBuilder.DropIndex(
                name: "IX_EmailSending_TypeEmailSendingId",
                table: "EmailSending");

            migrationBuilder.DropIndex(
                name: "IX_EmailSending_TypeEmailStatusId",
                table: "EmailSending");

            migrationBuilder.DropColumn(
                name: "RetrySender",
                table: "EmailSending");

            migrationBuilder.DropColumn(
                name: "RetrySenderDate",
                table: "EmailSending");

            migrationBuilder.DropColumn(
                name: "TypeEmailSendingId",
                table: "EmailSending");

            migrationBuilder.DropColumn(
                name: "TypeEmailStatusId",
                table: "EmailSending");

            migrationBuilder.RenameColumn(
                name: "RetrySenderErrorMessage",
                table: "EmailSending",
                newName: "ErrorMessage");

            migrationBuilder.RenameColumn(
                name: "RetrySenderErrorDetails",
                table: "EmailSending",
                newName: "ErrorDetails");

            migrationBuilder.AddColumn<string>(
                name: "EmailSendingType",
                table: "EmailSending",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Sent",
                table: "EmailSending",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 2,
                column: "Value",
                value: "AuthPolicyRead");

            migrationBuilder.UpdateData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 3,
                column: "Value",
                value: "AuthPolicyWrite");

            migrationBuilder.UpdateData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 4,
                column: "Value",
                value: "Claim");

            migrationBuilder.UpdateData(
                table: "ClaimType",
                keyColumn: "Id",
                keyValue: 5,
                column: "Value",
                value: "ClaimRead");

            migrationBuilder.InsertData(
                table: "ClaimType",
                columns: new[] { "Id", "Default", "Type", "Value" },
                values: new object[,]
                {
                    { 6, true, "Permission", "ClaimWrite" },
                    { 7, true, "Permission", "Licenza" },
                    { 8, true, "Permission", "LicenzaRead" },
                    { 9, true, "Permission", "LicenzaWrite" },
                    { 10, true, "Permission", "Modulo" },
                    { 11, true, "Permission", "ModuloRead" },
                    { 12, true, "Permission", "ModuloWrite" },
                    { 13, true, "Permission", "Profilo" },
                    { 14, true, "Permission", "ProfiloRead" },
                    { 15, true, "Permission", "ProfiloWrite" },
                    { 16, true, "Permission", "Ruolo" },
                    { 17, true, "Permission", "RuoloRead" },
                    { 18, true, "Permission", "RuoloWrite" }
                });
        }
    }
}
