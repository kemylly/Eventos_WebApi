using Microsoft.EntityFrameworkCore.Migrations;

namespace treino_api.Migrations
{
    public partial class RelacaoAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventoIdId",
                table: "Pessoas",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_EventoIdId",
                table: "Pessoas",
                column: "EventoIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_Eventos_EventoIdId",
                table: "Pessoas",
                column: "EventoIdId",
                principalTable: "Eventos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
