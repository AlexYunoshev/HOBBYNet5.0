using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class changeHobbiesAndAddExplorePosts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Locations_LocationId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "SubHobbyUser");

            migrationBuilder.DropTable(
                name: "SubHobbies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Locations",
                table: "Locations");

            migrationBuilder.RenameTable(
                name: "Locations",
                newName: "Location");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Location",
                table: "Location",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ExplorePosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExplorePosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExplorePosts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HobbyUser",
                columns: table => new
                {
                    HobbiesId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HobbyUser", x => new { x.HobbiesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_HobbyUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HobbyUser_Hobbies_HobbiesId",
                        column: x => x.HobbiesId,
                        principalTable: "Hobbies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExploreComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExploreComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExploreComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExploreComments_ExplorePosts_PostId",
                        column: x => x.PostId,
                        principalTable: "ExplorePosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExploreLikes",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExploreLikes", x => new { x.UserId, x.PostId });
                    table.ForeignKey(
                        name: "FK_ExploreLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExploreLikes_ExplorePosts_PostId",
                        column: x => x.PostId,
                        principalTable: "ExplorePosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExplorePostHobby",
                columns: table => new
                {
                    ExplorePostsId = table.Column<int>(type: "int", nullable: false),
                    HobbiesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExplorePostHobby", x => new { x.ExplorePostsId, x.HobbiesId });
                    table.ForeignKey(
                        name: "FK_ExplorePostHobby_ExplorePosts_ExplorePostsId",
                        column: x => x.ExplorePostsId,
                        principalTable: "ExplorePosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExplorePostHobby_Hobbies_HobbiesId",
                        column: x => x.HobbiesId,
                        principalTable: "Hobbies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExploreComments_PostId",
                table: "ExploreComments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ExploreComments_UserId",
                table: "ExploreComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExploreLikes_PostId",
                table: "ExploreLikes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_ExplorePostHobby_HobbiesId",
                table: "ExplorePostHobby",
                column: "HobbiesId");

            migrationBuilder.CreateIndex(
                name: "IX_ExplorePosts_UserId",
                table: "ExplorePosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HobbyUser_UsersId",
                table: "HobbyUser",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Location_LocationId",
                table: "AspNetUsers",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Location_LocationId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ExploreComments");

            migrationBuilder.DropTable(
                name: "ExploreLikes");

            migrationBuilder.DropTable(
                name: "ExplorePostHobby");

            migrationBuilder.DropTable(
                name: "HobbyUser");

            migrationBuilder.DropTable(
                name: "ExplorePosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Location",
                table: "Location");

            migrationBuilder.RenameTable(
                name: "Location",
                newName: "Locations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Locations",
                table: "Locations",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SubHobbies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HobbyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubHobbies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubHobbies_Hobbies_HobbyId",
                        column: x => x.HobbyId,
                        principalTable: "Hobbies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubHobbyUser",
                columns: table => new
                {
                    SubHobbiesId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubHobbyUser", x => new { x.SubHobbiesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_SubHobbyUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubHobbyUser_SubHobbies_SubHobbiesId",
                        column: x => x.SubHobbiesId,
                        principalTable: "SubHobbies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubHobbies_HobbyId",
                table: "SubHobbies",
                column: "HobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_SubHobbyUser_UsersId",
                table: "SubHobbyUser",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Locations_LocationId",
                table: "AspNetUsers",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
