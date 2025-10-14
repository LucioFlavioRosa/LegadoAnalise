using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SistemaAvaliacao.Domain.Models;

namespace SistemaAvaliacao.Repositories
{
    /// <summary>
    /// Implementação concreta de ICargoRepository usando ADO.NET.
    /// </summary>
    public class CargoRepository : ICargoRepository
    {
        private readonly string _connectionString;

        public CargoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Cargo> ObterTodos()
        {
            var cargos = new List<Cargo>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM Cargos", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cargos.Add(MapearCargo(reader));
                    }
                }
            }
            return cargos;
        }

        public Cargo ObterPorId(int idCargo)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM Cargos WHERE IdCargo = @IdCargo", conn))
            {
                cmd.Parameters.AddWithValue("@IdCargo", idCargo);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapearCargo(reader);
                }
            }
            return null;
        }

        public void Inserir(Cargo cargo)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(@"INSERT INTO Cargos (Cargo, ProximoCargoId, TempoMinimoPromocao, Funcao, Autonomia, EscopoAtuacao, NivelInterlocucao, Status) VALUES (@Cargo, @ProximoCargoId, @TempoMinimoPromocao, @Funcao, @Autonomia, @EscopoAtuacao, @NivelInterlocucao, @Status)", conn))
            {
                cmd.Parameters.AddWithValue("@Cargo", cargo.NomeCargo);
                cmd.Parameters.AddWithValue("@ProximoCargoId", (object)cargo.ProximoCargoId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TempoMinimoPromocao", cargo.TempoMinimoPromocao);
                cmd.Parameters.AddWithValue("@Funcao", cargo.Funcao);
                cmd.Parameters.AddWithValue("@Autonomia", cargo.Autonomia);
                cmd.Parameters.AddWithValue("@EscopoAtuacao", cargo.EscopoAtuacao);
                cmd.Parameters.AddWithValue("@NivelInterlocucao", cargo.NivelInterlocucao);
                cmd.Parameters.AddWithValue("@Status", cargo.Status);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Atualizar(Cargo cargo)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(@"UPDATE Cargos SET Cargo = @Cargo, ProximoCargoId = @ProximoCargoId, TempoMinimoPromocao = @TempoMinimoPromocao, Funcao = @Funcao, Autonomia = @Autonomia, EscopoAtuacao = @EscopoAtuacao, NivelInterlocucao = @NivelInterlocucao, Status = @Status WHERE IdCargo = @IdCargo", conn))
            {
                cmd.Parameters.AddWithValue("@Cargo", cargo.NomeCargo);
                cmd.Parameters.AddWithValue("@ProximoCargoId", (object)cargo.ProximoCargoId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TempoMinimoPromocao", cargo.TempoMinimoPromocao);
                cmd.Parameters.AddWithValue("@Funcao", cargo.Funcao);
                cmd.Parameters.AddWithValue("@Autonomia", cargo.Autonomia);
                cmd.Parameters.AddWithValue("@EscopoAtuacao", cargo.EscopoAtuacao);
                cmd.Parameters.AddWithValue("@NivelInterlocucao", cargo.NivelInterlocucao);
                cmd.Parameters.AddWithValue("@Status", cargo.Status);
                cmd.Parameters.AddWithValue("@IdCargo", cargo.IdCargo);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Inativar(int idCargo)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("UPDATE Cargos SET Status = 0 WHERE IdCargo = @IdCargo", conn))
            {
                cmd.Parameters.AddWithValue("@IdCargo", idCargo);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public byte[] ExportarParaExcel(IEnumerable<Cargo> cargos)
        {
            // Implementação simplificada: retorna um array vazio.
            // A lógica real de exportação deve ser implementada em ExportacaoService.
            return new byte[0];
        }

        private Cargo MapearCargo(IDataRecord reader)
        {
            return new Cargo
            {
                IdCargo = Convert.ToInt32(reader["IdCargo"]),
                NomeCargo = reader["Cargo"].ToString(),
                ProximoCargoId = reader["ProximoCargoId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["ProximoCargoId"]) : null,
                TempoMinimoPromocao = Convert.ToInt32(reader["TempoMinimoPromocao"]),
                Funcao = reader["Funcao"].ToString(),
                Autonomia = reader["Autonomia"].ToString(),
                EscopoAtuacao = reader["EscopoAtuacao"].ToString(),
                NivelInterlocucao = reader["NivelInterlocucao"].ToString(),
                Status = Convert.ToInt32(reader["Status"])
            };
        }
    }
}
