using Microsoft.EntityFrameworkCore.Migrations;

namespace treino_api.Migrations
{
    public partial class PessoaEventoAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PessoasEventos",
                columns: table => new
                {
                    PessoaID = table.Column<int>(nullable: false),
                    EventoID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PessoasEventos", x => new { x.EventoID, x.PessoaID });
                    table.ForeignKey(
                        name: "FK_PessoasEventos_Eventos_EventoID",
                        column: x => x.EventoID,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PessoasEventos_Pessoas_PessoaID",
                        column: x => x.PessoaID,
                        principalTable: "Pessoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PessoasEventos_PessoaID",
                table: "PessoasEventos",
                column: "PessoaID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PessoasEventos");
        }
    }
}
