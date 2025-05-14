using Domain.Dtos.SessionDtos;
using Domain.Globals;
using Domain.Interfaces;
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
    public class SessionRepository:ISessionRepository

    {

        public SessionRepository(ISessionRepository sessionRepository) { 
        }

        public async Task<(int sessionId, int returnCode, string errorMessage)> BookIndividualSessionAsync(SessionDTO session)
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

    }


    }

