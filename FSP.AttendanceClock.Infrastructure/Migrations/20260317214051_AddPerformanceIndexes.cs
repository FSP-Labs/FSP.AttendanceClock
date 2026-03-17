using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSP.AttendanceClock.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceAudits_Attendances_AttendanceId",
                table: "AttendanceAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceAudits_Users_ChangedByUserId",
                table: "AttendanceAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Users_UserId",
                table: "Attendances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemLogs",
                table: "SystemLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttendanceAudits",
                table: "AttendanceAudits");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Usuarios");

            migrationBuilder.RenameTable(
                name: "SystemLogs",
                newName: "RegistrosSistema");

            migrationBuilder.RenameTable(
                name: "Attendances",
                newName: "Fichajes");

            migrationBuilder.RenameTable(
                name: "AttendanceAudits",
                newName: "AuditoriasFichajes");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_UserId",
                table: "Fichajes",
                newName: "IX_Fichajes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceAudits_ChangedByUserId",
                table: "AuditoriasFichajes",
                newName: "IX_AuditoriasFichajes_ChangedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceAudits_AttendanceId",
                table: "AuditoriasFichajes",
                newName: "IX_AuditoriasFichajes_AttendanceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RegistrosSistema",
                table: "RegistrosSistema",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fichajes",
                table: "Fichajes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditoriasFichajes",
                table: "AuditoriasFichajes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Username",
                table: "Usuarios",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosSistema_Timestamp",
                table: "RegistrosSistema",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Fichajes_Timestamp",
                table: "Fichajes",
                column: "Timestamp");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditoriasFichajes_Fichajes_AttendanceId",
                table: "AuditoriasFichajes",
                column: "AttendanceId",
                principalTable: "Fichajes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditoriasFichajes_Usuarios_ChangedByUserId",
                table: "AuditoriasFichajes",
                column: "ChangedByUserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fichajes_Usuarios_UserId",
                table: "Fichajes",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditoriasFichajes_Fichajes_AttendanceId",
                table: "AuditoriasFichajes");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditoriasFichajes_Usuarios_ChangedByUserId",
                table: "AuditoriasFichajes");

            migrationBuilder.DropForeignKey(
                name: "FK_Fichajes_Usuarios_UserId",
                table: "Fichajes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Username",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RegistrosSistema",
                table: "RegistrosSistema");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosSistema_Timestamp",
                table: "RegistrosSistema");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fichajes",
                table: "Fichajes");

            migrationBuilder.DropIndex(
                name: "IX_Fichajes_Timestamp",
                table: "Fichajes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditoriasFichajes",
                table: "AuditoriasFichajes");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "RegistrosSistema",
                newName: "SystemLogs");

            migrationBuilder.RenameTable(
                name: "Fichajes",
                newName: "Attendances");

            migrationBuilder.RenameTable(
                name: "AuditoriasFichajes",
                newName: "AttendanceAudits");

            migrationBuilder.RenameIndex(
                name: "IX_Fichajes_UserId",
                table: "Attendances",
                newName: "IX_Attendances_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditoriasFichajes_ChangedByUserId",
                table: "AttendanceAudits",
                newName: "IX_AttendanceAudits_ChangedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditoriasFichajes_AttendanceId",
                table: "AttendanceAudits",
                newName: "IX_AttendanceAudits_AttendanceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemLogs",
                table: "SystemLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttendanceAudits",
                table: "AttendanceAudits",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceAudits_Attendances_AttendanceId",
                table: "AttendanceAudits",
                column: "AttendanceId",
                principalTable: "Attendances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceAudits_Users_ChangedByUserId",
                table: "AttendanceAudits",
                column: "ChangedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Users_UserId",
                table: "Attendances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
