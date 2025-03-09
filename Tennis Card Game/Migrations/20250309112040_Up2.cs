using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tennis_Card_Game.Migrations
{
    /// <inheritdoc />
    public partial class Up2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the existing index
            migrationBuilder.DropIndex(
                name: "IX_Players_UserId",
                table: "Players");

            // Drop the default constraint on UserId if it exists
            migrationBuilder.Sql(@"
                DECLARE @var sysname;
                SELECT @var = [d].[name]
                FROM [sys].[default_constraints] [d]
                INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Players]') AND [c].[name] = N'UserId');
                IF @var IS NOT NULL EXEC(N'ALTER TABLE [Players] DROP CONSTRAINT [' + @var + '];');
            ");

            // Allow NULL values in the UserId column
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Players",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: false);

            // Set UserId to NULL for rows with Id between 201 and 232
            migrationBuilder.Sql(@"
                UPDATE Players
                SET UserId = NULL
                WHERE Id BETWEEN 201 AND 232;
            ");

            // Add a filtered unique index on UserId, excluding rows where UserId is NULL
            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId",
                table: "Players",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            // Add the foreign key constraint only if it does not already exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Players_AspNetUsers_UserId'
                )
                BEGIN
                    ALTER TABLE [Players]
                    ADD CONSTRAINT [FK_Players_AspNetUsers_UserId]
                    FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
                    ON DELETE NO ACTION; -- Use NO ACTION instead of RESTRICT
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_AspNetUsers_UserId", // Corrected table name to AspNetUsers
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_UserId",
                table: "Players");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Players",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId",
                table: "Players",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_AspNetUsers_UserId", // Corrected table name to AspNetUsers
                table: "Players",
                column: "UserId",
                principalTable: "AspNetUsers", // Corrected table name to AspNetUsers
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction); // Use NoAction instead of Restrict
        }
    }
}