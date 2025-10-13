using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SistemaAvaliacao.Models;

namespace SistemaAvaliacao.Repositories
{
    public class CargoRepository : ICargoRepository
    {
        private readonly string _connectionString;

        public CargoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Cargo> GetAll()
        {
            var cargos = new List<Cargo>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM Cargos", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cargos.Add(MapCargo(reader));
                        }
                    }
                }
            }
            return cargos;
        }

        public Cargo GetById(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM Cargos WHERE IdCargo = @IdCargo", conn))
                {
                    cmd.Parameters.AddWithValue("@IdCargo", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapCargo(reader);
                        }
                    }
                }
            }
            return null;
        }

        public void Add(Cargo cargo)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("INSERT INTO Cargos (Cargo, ProximoCargoId, TempoMinimo, Funcao, Autonomia, EscopoAtuacao, NivelInterlocucao, Status) VALUES (@Cargo, @ProximoCargoId, @TempoMinimo, @Funcao, @Autonomia, @EscopoAtuacao, @NivelInterlocucao, @Status)", conn))
                {
                    cmd.Parameters.AddWithValue("@Cargo", cargo.Nome);
                    cmd.Parameters.AddWithValue("@ProximoCargoId", (object)cargo.ProximoCargoId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TempoMinimo", cargo.TempoMinimo);
                    cmd.Parameters.AddWithValue("@Funcao", cargo.Funcao);
                    cmd.Parameters.AddWithValue("@Autonomia", cargo.Autonomia);
                    cmd.Parameters.AddWithValue("@EscopoAtuacao", cargo.EscopoAtuacao);
                    cmd.Parameters.AddWithValue("@NivelInterlocucao", cargo.NivelInterlocucao);
                    cmd.Parameters.AddWithValue("@Status", cargo.Status);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Cargo cargo)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("UPDATE Cargos SET Cargo = @Cargo, ProximoCargoId = @ProximoCargoId, TempoMinimo = @TempoMinimo, Funcao = @Funcao, Autonomia = @Autonomia, EscopoAtuacao = @EscopoAtuacao, NivelInterlocucao = @NivelInterlocucao, Status = @Status WHERE IdCargo = @IdCargo", conn))
                {
                    cmd.Parameters.AddWithValue("@Cargo", cargo.Nome);
                    cmd.Parameters.AddWithValue("@ProximoCargoId", (object)cargo.ProximoCargoId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TempoMinimo", cargo.TempoMinimo);
                    cmd.Parameters.AddWithValue("@Funcao", cargo.Funcao);
                    cmd.Parameters.AddWithValue("@Autonomia", cargo.Autonomia);
                    cmd.Parameters.AddWithValue("@EscopoAtuacao", cargo.EscopoAtuacao);
                    cmd.Parameters.AddWithValue("@NivelInterlocucao", cargo.NivelInterlocucao);
                    cmd.Parameters.AddWithValue("@Status", cargo.Status);
                    cmd.Parameters.AddWithValue("@IdCargo", cargo.IdCargo);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("UPDATE Cargos SET Status = 0 WHERE IdCargo = @IdCargo", conn))
                {
                    cmd.Parameters.AddWithValue("@IdCargo", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private Cargo MapCargo(IDataReader reader)
        {
            return new Cargo
            {
                IdCargo = Convert.ToInt32(reader["IdCargo"]),
                Nome = reader["Cargo"].ToString(),
                ProximoCargoId = reader["ProximoCargoId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["ProximoCargoId"]) : null,
                TempoMinimo = reader["TempoMinimo"] != DBNull.Value ? Convert.ToInt32(reader["TempoMinimo"]) : 0,
                Funcao = reader["Funcao"].ToString(),
                Autonomia = reader["Autonomia"].ToString(),
                EscopoAtuacao = reader["EscopoAtuacao"].ToString(),
                NivelInterlocucao = reader["NivelInterlocucao"].ToString(),
                Status = Convert.ToBoolean(reader["Status"])
            };
        }
    }
}