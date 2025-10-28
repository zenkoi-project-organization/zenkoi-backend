using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updateClassification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnqualifiedCount",
                schema: "dbo",
                table: "ClassificationStages",
                newName: "ShowQualifiedCount");

            migrationBuilder.RenameColumn(
                name: "QualifiedCount",
                schema: "dbo",
                table: "ClassificationStages",
                newName: "PondQualifiedCount");

            migrationBuilder.RenameColumn(
                name: "UnqualifiedCount",
                schema: "dbo",
                table: "ClassificationRecords",
                newName: "ShowQualifiedCount");

            migrationBuilder.RenameColumn(
                name: "QualifiedCount",
                schema: "dbo",
                table: "ClassificationRecords",
                newName: "PondQualifiedCount");

            migrationBuilder.AddColumn<int>(
                name: "CullQualifiedCount",
                schema: "dbo",
                table: "ClassificationRecords",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CullQualifiedCount",
                schema: "dbo",
                table: "ClassificationRecords");

            migrationBuilder.RenameColumn(
                name: "ShowQualifiedCount",
                schema: "dbo",
                table: "ClassificationStages",
                newName: "UnqualifiedCount");

            migrationBuilder.RenameColumn(
                name: "PondQualifiedCount",
                schema: "dbo",
                table: "ClassificationStages",
                newName: "QualifiedCount");

            migrationBuilder.RenameColumn(
                name: "ShowQualifiedCount",
                schema: "dbo",
                table: "ClassificationRecords",
                newName: "UnqualifiedCount");

            migrationBuilder.RenameColumn(
                name: "PondQualifiedCount",
                schema: "dbo",
                table: "ClassificationRecords",
                newName: "QualifiedCount");
        }
    }
}
