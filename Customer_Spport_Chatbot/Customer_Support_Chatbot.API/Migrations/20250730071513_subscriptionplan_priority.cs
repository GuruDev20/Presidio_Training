﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer_Support_Chatbot.API.Migrations
{
    /// <inheritdoc />
    public partial class subscriptionplan_priority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "SubscriptionPlans",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "SubscriptionPlans");
        }
    }
}
