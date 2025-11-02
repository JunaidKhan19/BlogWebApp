using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlogWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class Reseeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Author", "CategoryId", "Content", "FeatureImagePath", "PublishedDate", "Title" },
                values: new object[,]
                {
                    { 1, "John Doe", 1, "Artificial Intelligence (AI) is becoming increasingly prevalent in our daily lives...", null, new DateTime(2025, 11, 2, 18, 32, 45, 379, DateTimeKind.Local).AddTicks(8052), "The Rise of AI in Everyday Life" },
                    { 2, "Jane Smith", 2, "Living a healthier lifestyle doesn't have to be complicated. Here are 10 simple tips...", null, new DateTime(2025, 11, 2, 18, 32, 45, 379, DateTimeKind.Local).AddTicks(8460), "10 Tips for a Healthier Lifestyle" },
                    { 3, "Emily Johnson", 3, "Looking for your next adventure? Here are the top 5 travel destinations to consider in 2024...", null, new DateTime(2025, 11, 2, 18, 32, 45, 379, DateTimeKind.Local).AddTicks(8463), "Top 5 Travel Destinations for 2024" }
                });
        }
    }
}
