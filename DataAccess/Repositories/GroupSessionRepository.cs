using Domain.Dtos.GroupSessionDtos;
using Domain.Globals;
using Domain.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class GroupSessionRepository : IGroupSessionRepository
    {
        public async Task<(int sessionId, int returnCode, string errorMessage)> BookGroupSessionAsync(BookGroupSessionDto sessionRequest)
        {
            int sessionId = -1;
            int returnCode = 0;
            string errorMessage = string.Empty;
            using SqlConnection sqlConnection = new SqlConnection(Connection.ConnectionString);
            using SqlCommand sqlCommand = new SqlCommand("usp_BookGroupSession", sqlConnection);
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@TherapistID", sessionRequest.TherapistID);
            sqlCommand.Parameters.AddWithValue("ScheduledStartTime", sessionRequest.scheduledStartTime);
            sqlCommand.Parameters.AddWithValue("@Duration", sessionRequest.duration);
            sqlCommand.Parameters.AddWithValue("@description", sessionRequest.Description);
            sqlCommand.Parameters.AddWithValue("@maxParticipants", sessionRequest.MaxParticipants);

            SqlParameter sessionIdParam = new SqlParameter("@newSessionID", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            sqlCommand.Parameters.Add(sessionIdParam);

            SqlParameter returnValue = new SqlParameter
            {
                Direction = ParameterDirection.ReturnValue,
                SqlDbType = SqlDbType.Int
            };
            sqlCommand.Parameters.Add(returnValue);
            try
            {
                await sqlConnection.OpenAsync();
                await sqlCommand.ExecuteNonQueryAsync();
                sessionId = sessionIdParam.Value != DBNull.Value ? Convert.ToInt32(sessionIdParam.Value) : -1;
                returnCode = returnValue.Value != DBNull.Value ? Convert.ToInt32(returnValue.Value) : -99;

            }
            catch (SqlException ex)
            {

                errorMessage = ex.Message;

            }
            return (sessionId, returnCode, errorMessage);

        }

        public async Task<ServiceResult<bool>> ChangeGroupSessionStatusAsync(int sessionId, string status)
        {
            using (SqlConnection sqlConnection = new SqlConnection(Connection.ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("usp_UpdateGroupSessionStatus", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@GroupSessionID", sessionId);
                    sqlCommand.Parameters.AddWithValue("@NewStatus", status);
                    try
                    {
                        await sqlConnection.OpenAsync();
                        await sqlCommand.ExecuteNonQueryAsync(); // We don't need to check rowsAffected here directly for success
                                                                 // as the SP handles the success/failure logic via RAISERROR
                        return ServiceResult<bool>.Success(true); // If no exception is thrown, it means the SP completed successfully.
                    }
                    catch (SqlException ex)
                    {
                        // Check for your custom error states from the stored procedure
                        switch (ex.State)
                        {
                            case 1: // Invalid GroupSessionID
                            case 2: // Session not found
                            case 3: // Invalid status provided
                            case 4:
                            case 5:// Status is already the same, no change needed
                                return ServiceResult<bool>.Failure(ex.Message, ex.State);
                            default:
                                // General database error not specifically handled by custom states
                                return ServiceResult<bool>.Failure($"Database error: {ex.Message}", -99);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Catch any other unexpected errors
                        return ServiceResult<bool>.Failure($"An unexpected error occurred: {ex.Message}", -100);
                    }
                }
            }
        }
        public async Task<ServiceResult<GroupSession>> GetGroupSessionByIdAsync(int sessionId, string? status = null)
        {
            GroupSession? groupSession = null;
            using (SqlConnection sqlConnection = new SqlConnection(Connection.ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("usp_GetGroupSessionById", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@sessionID", sessionId);
                    sqlCommand.Parameters.AddWithValue("@StatusFilter", status ?? (object)DBNull.Value);

                    try
                    {
                        await sqlConnection.OpenAsync();
                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            bool sessionDetailsRead = false;

                            while (await reader.ReadAsync())
                            {
                                if (!sessionDetailsRead)
                                {
                                    // Initialize GroupSession object from the first row
                                    groupSession = new GroupSession
                                    {
                                        GroupSessionID = reader.GetInt32(reader.GetOrdinal("GroupSessionID")),
                                        TherapistID = reader.GetInt32(reader.GetOrdinal("TherapistID")),
                                        TherapistFirstName = reader.GetString(reader.GetOrdinal("TherapistFirstName")),
                                        TherapistLastName = reader.GetString(reader.GetOrdinal("TherapistLastName")),
                                        ScheduledStartTime = reader.GetDateTime(reader.GetOrdinal("ScheduledStartTime")),
                                        Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                        SessionType = reader.GetString(reader.GetOrdinal("SessionType")),
                                        Status = reader.GetString(reader.GetOrdinal("Status")),
                                        MaxParticipants = reader.GetInt32(reader.GetOrdinal("MaxParticipants")),
                                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                        Participants = new List<ParticipantDTO>() // Initialize the list
                                    };
                                    sessionDetailsRead = true;
                                }

                                // If groupSession is still null, it means no session details were read (e.g., empty result set)
                                if (groupSession == null) continue;

                                // Add participant data if available in the current row
                                if (!reader.IsDBNull(reader.GetOrdinal("PatientID")))
                                {
                                    groupSession.Participants!.Add(new ParticipantDTO
                                    {
                                        PatientID = reader.GetInt32(reader.GetOrdinal("PatientID")),
                                        PatientFirstName = reader.GetString(reader.GetOrdinal("PatientFirstName")),
                                        PatientLastName = reader.GetString(reader.GetOrdinal("PatientLastName")),
                                        JoinedAt = reader.GetDateTime(reader.GetOrdinal("JoinedAt"))
                                    });
                                }
                            }
                        }

                        if (groupSession == null)
                        {
                            // This should not happen now because SP raises errors,
                            // but keeping as fallback
                            return ServiceResult<GroupSession>.Failure("Group session not found.", -2);
                        }

                        return ServiceResult<GroupSession>.Success(groupSession);
                    }
                    catch (SqlException ex)
                    {
                        // Handle specific error states from the stored procedure
                        return ServiceResult<GroupSession>.HandleSqlException(ex);
                    }
                    catch (Exception ex)
                    {
                        return ServiceResult<GroupSession>.Failure($"An unexpected error occurred: {ex.Message}", -100);
                    }
                }
            }
        }


        public async Task<ServiceResult<List<GroupSession>>> GetGroupSessionsByPatientIdAsync(int patientId, string? status = null)
        {
            List<GroupSession> groupSessions = new List<GroupSession>();

            using (SqlConnection sqlConnection = new SqlConnection(Connection.ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("usp_GetGroupSessionsByPatientId", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@PatientId", patientId);
                    sqlCommand.Parameters.AddWithValue("@StatusFilter", status);
                    try
                    {
                        await sqlConnection.OpenAsync();
                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var session = new GroupSession
                                {
                                    GroupSessionID = reader.GetInt32(reader.GetOrdinal("GroupSessionID")),
                                    TherapistID = reader.GetInt32(reader.GetOrdinal("TherapistID")),
                                    TherapistFirstName = reader.GetString(reader.GetOrdinal("TherapistFirstName")),
                                    TherapistLastName = reader.GetString(reader.GetOrdinal("TherapistLastName")),
                                    ScheduledStartTime = reader.GetDateTime(reader.GetOrdinal("ScheduledStartTime")),
                                    Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                    SessionType = reader.GetString(reader.GetOrdinal("SessionType")),
                                    Status = reader.GetString(reader.GetOrdinal("Status")),
                                    MaxParticipants = reader.IsDBNull(reader.GetOrdinal("MaxParticipants")) ? null : reader.GetInt32(reader.GetOrdinal("MaxParticipants")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    Participants = new List<ParticipantDTO>() // Initialize the participants list
                                };

                                if (!reader.IsDBNull(reader.GetOrdinal("PatientID")))
                                {
                                    session.Participants.Add(new ParticipantDTO
                                    {
                                        PatientID = reader.GetInt32(reader.GetOrdinal("PatientID")),
                                        PatientFirstName = reader.GetString(reader.GetOrdinal("PatientFirstName")),
                                        PatientLastName = reader.GetString(reader.GetOrdinal("PatientLastName")),
                                        JoinedAt = reader.GetDateTime(reader.GetOrdinal("JoinedAt"))
                                    });
                                }
                                groupSessions.Add(session);
                            }
                        }

                        return ServiceResult<List<GroupSession>>.Success(groupSessions);
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine($"SQL Error in GetGroupSessionsByPatientIdAsync: {ex.Message} (Error Number: {ex.Number}, State: {ex.State})");
                        if (ex.State == 1 || ex.State == 2 || ex.State == 3 || ex.State == 4) // Check if these are the states from RAISERROR
                        {
                            return ServiceResult<List<GroupSession>>.Failure(ex.Message, ex.State);
                        }
                        return ServiceResult<List<GroupSession>>.Failure($"Database error: {ex.Message}", -99);
                    }
                    catch (Exception ex)
                    {
                        return ServiceResult<List<GroupSession>>.Failure($"An unexpected error occurred: {ex.Message}", -100);
                    }
                }
            }
        }

        public async Task<ServiceResult<List<GroupSession>>> GetGroupSessionsByTherapistIdAsync(int therapistId, string? status = null)
        {
            using (SqlConnection connection = new SqlConnection(Connection.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("usp_GetGroupSessionsByTherapistId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TherapistId", therapistId);
                    command.Parameters.AddWithValue("@StatusFilter", status ?? (object)DBNull.Value);
                    await connection.OpenAsync();
                    try
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {

                            List<GroupSession> groupSessions = new List<GroupSession>();
                            while (await reader.ReadAsync())
                            {
                                GroupSession session = new GroupSession
                                {
                                    GroupSessionID = reader.GetInt32(reader.GetOrdinal("GroupSessionID")),
                                    TherapistID = reader.GetInt32(reader.GetOrdinal("TherapistID")),
                                    TherapistFirstName = reader.GetString(reader.GetOrdinal("TherapistFirstName")),
                                    TherapistLastName = reader.GetString(reader.GetOrdinal("TherapistLastName")),
                                    ScheduledStartTime = reader.GetDateTime(reader.GetOrdinal("ScheduledStartTime")),
                                    Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                    SessionType = reader.GetString(reader.GetOrdinal("SessionType")),
                                    Status = reader.GetString(reader.GetOrdinal("Status")),
                                    MaxParticipants = reader.IsDBNull(reader.GetOrdinal("MaxParticipants")) ? null : reader.GetInt32(reader.GetOrdinal("MaxParticipants")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"))
                                };
                                groupSessions.Add(session);
                            }
                            return ServiceResult<List<GroupSession>>.Success(groupSessions);
                        }


                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine($"SQL Error in GetGroupSessionsByTherapistIdAsync: {ex.Message} (Error Number: {ex.Number}, State: {ex.State})");
                        if (ex.State == 1 || ex.State == 2 || ex.State == 3 || ex.State == 4) // Check if these are the states from RAISERROR
                        {
                            return ServiceResult<List<GroupSession>>.Failure(ex.Message, ex.State);
                        }
                        return ServiceResult<List<GroupSession>>.Failure($"Database error: {ex.Message}", -99);
                    }
                    catch (Exception ex)
                    {
                        return ServiceResult<List<GroupSession>>.Failure($"An unexpected error occurred: {ex.Message}", -100);

                    }


                }
            }
        }

        public async Task<int> JoinGroupSessionAsync(int patientId, int sessionId)
        {
            using (SqlConnection connection = new SqlConnection(Connection.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("usp_JoinGroupSession", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PatientID", patientId);
                    command.Parameters.AddWithValue("@sessionID", sessionId);

                    SqlParameter returnValue = new SqlParameter
                    {
                        Direction = ParameterDirection.ReturnValue,
                        SqlDbType = SqlDbType.Int
                    };
                    command.Parameters.Add(returnValue);
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    return returnValue.Value != DBNull.Value ? Convert.ToInt32(returnValue.Value) : -99;
                }
            }

        }


    }
}
