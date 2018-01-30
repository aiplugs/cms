using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Aiplugs.CMS.Web.Migrations
{
    public partial class AppendSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData("Folders", "Name", "Home");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
