using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tauron.Application.CQRS.Dispatcher.Migrations
{
    public partial class V1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventEntities",
                columns: table => new
                {
                    SequenceNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(nullable: true),
                    EventType = table.Column<int>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    EventName = table.Column<string>(nullable: true),
                    OriginType = table.Column<string>(nullable: true),
                    Version = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<DateTimeOffset>(nullable: false),
                    OperationId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventEntities", x => x.SequenceNumber);
                });

            migrationBuilder.CreateTable(
                name: "ObjectStades",
                columns: table => new
                {
                    Identifer = table.Column<string>(nullable: false),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectStades", x => x.Identifer);
                });

            migrationBuilder.CreateTable(
                name: "PendingMessages",
                columns: table => new
                {
                    SequenceNumber = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: true),
                    EventType = table.Column<int>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    EventName = table.Column<string>(nullable: true),
                    OriginType = table.Column<string>(nullable: true),
                    Version = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<DateTimeOffset>(nullable: false),
                    OperationId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingMessages", x => x.SequenceNumber);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "EventEntities");

            migrationBuilder.DropTable(
                name: "ObjectStades");

            migrationBuilder.DropTable(
                name: "PendingMessages");
        }
    }
}
