using Microsoft.EntityFrameworkCore.Migrations;

namespace SistemaAvaliacao.Moderno.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cargos",
                columns: table => new
                {
                    IdCargo = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeCargo = table.Column<string>(maxLength: 100, nullable: false),
                    ProximoCargoId = table.Column<int>(nullable: true),
                    TempoMinimoPromocao = table.Column<int>(nullable: false),
                    Funcao = table.Column<string>(nullable: true),
                    Autonomia = table.Column<string>(nullable: true),
                    EscopoAtuacao = table.Column<string>(nullable: true),
                    NivelInterlocucao = table.Column<string>(nullable: true),
                    Ativo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargos", x => x.IdCargo);
                    table.ForeignKey(
                        name: "FK_Cargos_Cargos_ProximoCargoId",
                        column: x => x.ProximoCargoId,
                        principalTable: "Cargos",
                        principalColumn: "IdCargo",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateIndex(
                name: "IX_Cargos_ProximoCargoId",
                table: "Cargos",
                column: "ProximoCargoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cargos");
        }
    }
}
