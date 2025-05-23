using Domain.Dtos.SessionDtos;
using Domain.Globals;
using Domain.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DataAccess.Repositories
{
    public class SessionRepository : ISessionRepository

    {
        public async Task<(int sessionId, int returnCode, string errorMessage)> BookIndividualSessionAsync(BookSessionDTO session)
        {
            int newSessionId = -1;
            int spReturnCode = -1; // Stored procedure's own return value
            string errorMessage = "";

            using (SqlConnection connection = new SqlConnection(Connection.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("dbo.usp_BookIndividualSession", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TherapistID", session.therapistId);
                    command.Parameters.AddWithValue("@PatientID", session.patientId);
                    command.Parameters.AddWithValue("@ScheduledStartTime", session.scheduledStartTime);
                    command.Parameters.AddWithValue("@Duration", session.duration);
                    command.Parameters.AddWithValue("@SessionType", session.sessionType);
                    command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value = session.description as object ?? DBNull.Value;

                    SqlParameter newSessionIdParam = new SqlParameter("@NewSessionID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(newSessionIdParam);

                    SqlParameter returnValueParam = new SqlParameter // To capture the RETURN value of SP
                    {
                        ParameterName = "@Return_Value",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.ReturnValue
                    };
                    command.Parameters.Add(returnValueParam);

                    try
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();

                        spReturnCode = (int)returnValueParam.Value; // SP's explicit RETURN value

                        if (newSessionIdParam.Value != DBNull.Value)
                        {
                            newSessionId = (int)newSessionIdParam.Value;
                        }

                        if (newSessionId <= 0 && spReturnCode < 0) // Check both, SP might return error via NewSessionID or RETURN
                        {
                            // Error message might be raised by RAISERROR,
                            // or you might have a convention to return messages via an OUTPUT param
                            errorMessage = $"Stored procedure failed with code: {spReturnCode}. Session ID output: {newSessionId}.";
                            // In a real app, you'd capture the RAISERROR message if possible,
                            // or rely on the spReturnCode to map to a user-friendly message in BLL.
                        }
                    }
                    catch (SqlException ex)
                    {
                        // Log exception ex
                        errorMessage = ex.Message; // This will catch RAISERROR messages
                        newSessionId = -99; // Indicate application-level error during DB call
                        spReturnCode = -99;
                    }

                }
            }
            return (newSessionId, spReturnCode, errorMessage);
        }

        public async Task<ServiceResult<bool>> ChangeIndividualSessionStatusAsync(int sessionId, string status)
        {
            using (SqlConnection sqlConnection = new SqlConnection(Connection.ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("usp_UpdateIndividualSessionStatus", sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@SessionID", sessionId);
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
                            case 4:// Status is already the same, no change needed
                            case 5:// The status did not change
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

        public async Task<GetSessionsByRoleID> GetSessionsByPatientIdAsync(int patientId, string? status)
        {
            var sessions = new List<SessionDataModel>();
            int returnCode = 0;
            string errorMessage = "Success";

            using (SqlConnection connection = new SqlConnection(Connection.ConnectionString))
            using (SqlCommand command = new SqlCommand("dbo.usp_GetSessionsByPatientId", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PatientID", patientId);
                command.Parameters.AddWithValue("@status", (object?)status ?? DBNull.Value);

                // Prepare to capture the return value
                var returnValueParam = new SqlParameter("@ReturnValue", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                command.Parameters.Add(returnValueParam);

                try
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            sessions.Add(new SessionDataModel
                            {
                                SessionID = reader.GetInt32(reader.GetOrdinal("SessionID")),
                                TherapistID = reader.GetInt32(reader.GetOrdinal("TherapistID")),
                                PatientID = reader.GetInt32(reader.GetOrdinal("PatientID")),
                                TherapistFirstName = reader.GetString(reader.GetOrdinal("TherapistFirstName")),
                                TherapistLastName = reader.GetString(reader.GetOrdinal("TherapistLastName")),
                                PatientFirstName = reader.GetString(reader.GetOrdinal("PatientFirstName")),
                                PatientLastName = reader.GetString(reader.GetOrdinal("PatientLastName")),
                                SessionType = reader.GetString(reader.GetOrdinal("SessionType")),
                                ScheduledStartTime = reader.GetDateTime(reader.GetOrdinal("ScheduledStartTime")),
                                Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                ActualStartTime = reader.IsDBNull(reader.GetOrdinal("ActualStartTime")) ? null : reader.GetDateTime(reader.GetOrdinal("ActualStartTime")),
                                FeedbackID = reader.IsDBNull(reader.GetOrdinal("FeedbackID")) ? null : reader.GetInt32(reader.GetOrdinal("FeedbackID")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"))
                            });
                        }
                    }

                    // Get the return value from the stored procedure
                    returnCode = (int)(returnValueParam.Value ?? 0);

                    if (returnCode == 0)
                    {
                        if (sessions.Count == 0)
                        {
                            // Patient exists but has no sessions
                            errorMessage = "No sessions found for this patient.";
                            returnCode = -2;
                            return new GetSessionsByRoleID { returnCode = returnCode, returnMessage = errorMessage, sessions = null };
                        }
                        else
                        {
                            errorMessage = "Success";
                            return new GetSessionsByRoleID { returnCode = returnCode, returnMessage = errorMessage, sessions = sessions };
                        }
                    }
                    else if (returnCode == -1)
                    {
                        errorMessage = "Patient not found.";
                        return new GetSessionsByRoleID { returnCode = returnCode, returnMessage = errorMessage, sessions = null };
                    }
                    else if (returnCode == -2)
                    {
                        errorMessage = "No sessions found for this patient.";
                        return new GetSessionsByRoleID { returnCode = returnCode, returnMessage = errorMessage, sessions = null };
                    }
                    else
                    {
                        errorMessage = "Unknown error occurred.";
                        return new GetSessionsByRoleID { returnCode = returnCode, returnMessage = errorMessage, sessions = null };
                    }
                }
                catch (SqlException ex)
                {
                    // Map known error messages to return codes
                    if (ex.Message.Contains("Patient not found"))
                    {
                        returnCode = -1;
                        errorMessage = "Patient not found.";
                    }
                    else if (ex.Message.Contains("No sessions found"))
                    {
                        returnCode = -2;
                        errorMessage = "No sessions found for this patient.";
                    }
                    else
                    {
                        returnCode = -99;
                        errorMessage = "Database error: " + ex.Message;
                    }
                    return new GetSessionsByRoleID { returnCode = returnCode, returnMessage = errorMessage, sessions = null };
                }
                catch (Exception ex)
                {
                    return new GetSessionsByRoleID { returnCode = -99, returnMessage = $"{ex.Message}", sessions = null };
                }
            }
        }
        public async Task<GetSessionsByRoleID> GetSessionsByTherapistIdAsync(int therapistID, string? status)
        {
            var sessions = new List<SessionDataModel>();
            int returnCode = 0;
            string errorMessage = "Success";

            using (SqlConnection connection = new SqlConnection(Connection.ConnectionString))
            using (SqlCommand command = new SqlCommand("dbo.usp_GetSessionsByTherapistID", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TherapistID", therapistID);
                command.Parameters.AddWithValue("@status", (object?)status ?? DBNull.Value);

                // Prepare to capture the return value
                var returnValueParam = new SqlParameter("@ReturnValue", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                command.Parameters.Add(returnValueParam);

                try
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            sessions.Add(new SessionDataModel
                            {
                                SessionID = reader.GetInt32(reader.GetOrdinal("SessionID")),
                                TherapistID = reader.GetInt32(reader.GetOrdinal("TherapistID")),
                                PatientID = reader.GetInt32(reader.GetOrdinal("PatientID")),
                                TherapistFirstName = reader.GetString(reader.GetOrdinal("TherapistFirstName")),
                                TherapistLastName = reader.GetString(reader.GetOrdinal("TherapistLastName")),
                                PatientFirstName = reader.GetString(reader.GetOrdinal("PatientFirstName")),
                                PatientLastName = reader.GetString(reader.GetOrdinal("PatientLastName")),
                                SessionType = reader.GetString(reader.GetOrdinal("SessionType")),
                                ScheduledStartTime = reader.GetDateTime(reader.GetOrdinal("ScheduledStartTime")),
                                Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                ActualStartTime = reader.IsDBNull(reader.GetOrdinal("ActualStartTime")) ? null : reader.GetDateTime(reader.GetOrdinal("ActualStartTime")),
                                FeedbackID = reader.IsDBNull(reader.GetOrdinal("FeedbackID")) ? null : reader.GetInt32(reader.GetOrdinal("FeedbackID")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"))
                            });
                        }
                    }

                    // Get the return value from the stored procedure
                    returnCode = (int)(returnValueParam.Value ?? 0);

                    if (returnCode == 0)
                    {
                        if (sessions.Count == 0)
                        {
                            // Patient exists but has no sessions
                            errorMessage = "No sessions found for this therapist.";
                            returnCode = -2;
                            return new GetSessionsByRoleID { returnCode = returnCode, returnMessage = errorMessage, sessions = null };
                        }
                        else
                        {
                            errorMessage = "Success";
                            return new GetSessionsByRoleID { returnCode = returnCode, returnMessage = errorMessage, sessions = sessions };
                        }
                    }
                    else if (returnCode == -1)
                    {
                        errorMessage = "therapist not found.";
                        return new GetSessionsByRoleID { returnCode = returnCode, returnMessage = errorMessage, sessions = null };
                    }
                    else if (returnCode == -2)
                    {
                        errorMessage = "No sessions found for this therapist.";
                        return new GetSessionsByRoleID { returnCode = returnCode, returnMessage = errorMessage, sessions = null };
                    }
                    else
                    {
                        errorMessage = "Unknown error occurred.";
                        return new GetSessionsByRoleID { returnCode = returnCode, returnMessage = errorMessage, sessions = null };
                    }
                }
                catch (SqlException ex)
                {
                    // Map known error messages to return codes
                    if (ex.Message.Contains("therapist not found"))
                    {
                        returnCode = -1;
                        errorMessage = "therapist not found.";
                    }
                    else if (ex.Message.Contains("No sessions found"))
                    {
                        returnCode = -2;
                        errorMessage = "No sessions found for this therapist.";
                    }
                    else
                    {
                        returnCode = -99;
                        errorMessage = "Database error: " + ex.Message;
                    }
                    return new GetSessionsByRoleID { returnCode = returnCode, returnMessage = errorMessage, sessions = null };
                }
                catch (Exception ex)
                {
                    return new GetSessionsByRoleID { returnCode = -99, returnMessage = $"{ex.Message}", sessions = null };
                }
            }
        }

    }
}

