using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SistemaAvaliacao.Domain.Models;
using SistemaAvaliacao.Infrastructure.Data;

namespace SistemaAvaliacao.Repositories
{
    public class CargoRepository : ICargoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CargoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Cargo GetById(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Cargos WHERE IdCargo = @IdCargo";
                    var param = command.CreateParameter();
                    param.ParameterName = "@IdCargo";
                    param.Value = id;
                    command.Parameters.Add(param);
                    using (var reader = command.ExecuteReader())
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

        public IEnumerable<Cargo> GetAll()
        {
            var cargos = new List<Cargo>();
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Cargos";
                    using (var reader = command.ExecuteReader())
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

        public void Add(Cargo cargo)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO Cargos (Cargo, ProximoCargoId, TempoMinimoPromocao, Funcao, Autonomia, EscopoAtuacao, NivelInterlocucao, Status) 
                                            VALUES (@Cargo, @ProximoCargoId, @TempoMinimoPromocao, @Funcao, @Autonomia, @EscopoAtuacao, @NivelInterlocucao, @Status)";
                    AddParameters(command, cargo);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(Cargo cargo)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE Cargos SET Cargo = @Cargo, ProximoCargoId = @ProximoCargoId, TempoMinimoPromocao = @TempoMinimoPromocao, Funcao = @Funcao, Autonomia = @Autonomia, EscopoAtuacao = @EscopoAtuacao, NivelInterlocucao = @NivelInterlocucao, Status = @Status WHERE IdCargo = @IdCargo";
                    AddParameters(command, cargo);
                    var paramId = command.CreateParameter();
                    paramId.ParameterName = "@IdCargo";
                    paramId.Value = cargo.IdCargo;
                    command.Parameters.Add(paramId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Cargos SET Status = 0 WHERE IdCargo = @IdCargo";
                    var param = command.CreateParameter();
                    param.ParameterName = "@IdCargo";
                    param.Value = id;
                    command.Parameters.Add(param);
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Cargo> GetCargosForExport()
        {
            // Pode ser customizado para exportação
            return GetAll();
        }

        private Cargo MapCargo(IDataRecord record)
        {
            return new Cargo
            {
                IdCargo = Convert.ToInt32(record["IdCargo"]),
                NomeCargo = record["Cargo"].ToString(),
                ProximoCargoId = record["ProximoCargoId"] == DBNull.Value ? (int?)null : Convert.ToInt32(record["ProximoCargoId"]),
                TempoMinimoPromocao = record["TempoMinimoPromocao"] == DBNull.Value ? (int?)null : Convert.ToInt32(record["TempoMinimoPromocao"]),
                Funcao = record["Funcao"].ToString(),
                Autonomia = record["Autonomia"].ToString(),
                EscopoAtuacao = record["EscopoAtuacao"].ToString(),
                NivelInterlocucao = record["NivelInterlocucao"].ToString(),
                Status = Convert.ToBoolean(record["Status"])
            };
        }

        private void AddParameters(IDbCommand command, Cargo cargo)
        {
            var paramCargo = command.CreateParameter();
            paramCargo.ParameterName = "@Cargo";
            paramCargo.Value = cargo.NomeCargo;
            command.Parameters.Add(paramCargo);

            var paramProx = command.CreateParameter();
            paramProx.ParameterName = "@ProximoCargoId";
            paramProx.Value = (object)cargo.ProximoCargoId ?? DBNull.Value;
            command.Parameters.Add(paramProx);

            var paramTempo = command.CreateParameter();
            paramTempo.ParameterName = "@TempoMinimoPromocao";
            paramTempo.Value = (object)cargo.TempoMinimoPromocao ?? DBNull.Value;
            command.Parameters.Add(paramTempo);

            var paramFuncao = command.CreateParameter();
            paramFuncao.ParameterName = "@Funcao";
            paramFuncao.Value = cargo.Funcao ?? string.Empty;
            command.Parameters.Add(paramFuncao);

            var paramAutonomia = command.CreateParameter();
            paramAutonomia.ParameterName = "@Autonomia";
            paramAutonomia.Value = cargo.Autonomia ?? string.Empty;
            command.Parameters.Add(paramAutonomia);

            var paramEscopo = command.CreateParameter();
            paramEscopo.ParameterName = "@EscopoAtuacao";
            paramEscopo.Value = cargo.EscopoAtuacao ?? string.Empty;
            command.Parameters.Add(paramEscopo);

            var paramNivel = command.CreateParameter();
            paramNivel.ParameterName = "@NivelInterlocucao";
            paramNivel.Value = cargo.NivelInterlocucao ?? string.Empty;
            command.Parameters.Add(paramNivel);

            var paramStatus = command.CreateParameter();
            paramStatus.ParameterName = "@Status";
            paramStatus.Value = cargo.Status;
            command.Parameters.Add(paramStatus);
        }
    }
}
