﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Tasks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateDeadlineCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeadLine",
                table: "Tasks",
                newName: "Deadline");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Deadline",
                table: "Tasks",
                newName: "DeadLine");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
