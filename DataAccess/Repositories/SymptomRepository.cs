using Domain.Dtos.SymptomDtos;
using Domain.Globals;
using Domain.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class SymptomRepository : ISymptomRepository
    {
        public async Task<ServiceResult<int>> AddSymptomAsync(AddSymptomDto symptoms)
        {
            int lastInsertedId = -1;
            using (SqlConnection connection = new SqlConnection(Connection.ConnectionString))
            {
                await connection.OpenAsync();
                foreach (var symptom in symptoms.SymptomTypes)
                {
                    using (SqlCommand command = new SqlCommand("usp_AddPatientSymptom", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PatientID", symptoms.PatientID);
                        command.Parameters.AddWithValue("@SymptomTypeID", symptom.SymptomTypeID);
                        command.Parameters.AddWithValue("@Severity", symptom.Severity);
                        command.Parameters.AddWithValue("@TherapistID", symptoms.TherapistID);
                        command.Parameters.AddWithValue("@Notes", (object?)symptom.Notes ?? DBNull.Value);

                        var outputParam = new SqlParameter("@NewSymptomID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputParam);

                        try
                        {
                            await command.ExecuteNonQueryAsync();
                            lastInsertedId = (int)outputParam.Value;
                        }
                        catch (SqlException ex)
                        {
                            // Optionally: collect all errors and return them
                            return ServiceResult<int>.Failure(ex.Message, ex.State);
                        }
                        catch (Exception ex)
                        {
                            return ServiceResult<int>.Failure(ex.Message, 0);
                        }
                    }
                }
            }
            return ServiceResult<int>.Success(lastInsertedId);
        }

        public async Task<ServiceResult<IEnumerable<SymptomHistoricDto>>> GetPatientSymptomsAsync(int patientId)
        {
            using(SqlConnection connection = new SqlConnection(Connection.ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("usp_GetPatientSymptoms", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PatientID", patientId);
                    try
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            List<SymptomHistoricDto> symptoms = new List<SymptomHistoricDto>();
                            while (await reader.ReadAsync())
                            {
                                symptoms.Add(new SymptomHistoricDto
                                {
                                    SymptomID = reader.GetInt32(reader.GetOrdinal("SymptomID")),
                                    SymptomCategory = reader.GetString(reader.GetOrdinal("SymptomCategory")),
                                    SymptomTypeDescription = reader.GetString(reader.GetOrdinal("Description")),
                                    Severity = reader.GetDecimal(reader.GetOrdinal("Severity")),
                                    RecordedAt = reader.GetDateTime(reader.GetOrdinal("RecordedAt")),
                                    TherapistID = reader.GetInt32(reader.GetOrdinal("TherapistID")),
                                    TherapistFirstName = reader.GetString(reader.GetOrdinal("TherapistFirstName")),
                                    TherapistLastName = reader.GetString(reader.GetOrdinal("TherapistLastName")),
                                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes"))
                                });
                            }
                            return ServiceResult<IEnumerable<SymptomHistoricDto>>.Success(symptoms);
                        }
                    }
                    catch (SqlException ex)
                    {
                        return ServiceResult<IEnumerable<SymptomHistoricDto>>.Failure(ex.Message, ex.State);
                    }
                    catch (Exception ex)
                    {
                        return ServiceResult<IEnumerable<SymptomHistoricDto>>.Failure(ex.Message, 99);
                    }
                }
            }
        }
    }
}
