using Domain.Dtos.FeedbackDtos;
using Domain.Globals;
using Domain.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        public async Task<ServiceResult<int>> CreateFeedbackAsync(FeedbackDto feedback)
        {
            using (SqlConnection connection = new SqlConnection(Connection.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.usp_AddFeedback", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PatientID", feedback.PatientId);
                    cmd.Parameters.AddWithValue("@TherapistID", feedback.TherapistId);
                    cmd.Parameters.AddWithValue("@rating", feedback.Rating);
                    cmd.Parameters.AddWithValue("@ReviewText", feedback.ReviewText ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@sessionID", (object)feedback.SessionId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@groupSessionID", (object)feedback.GroupSessionId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@sessiontype", feedback.SessionType);

                    try
                    {
                        await connection.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync()) // Use ExecuteReaderAsync here!
                        {
                            if (reader.HasRows)
                            {
                                await reader.ReadAsync(); // Read the first (and only) row
                                string resultMessage = reader["Result"].ToString();
                                int newFeedbackId = Convert.ToInt32(reader["NewFeedbackID"]);

                                if (newFeedbackId > 0)
                                {
                                    return ServiceResult<int>.Success(newFeedbackId);
                                }
                                else
                                {
                                    // This scenario might happen if the SP somehow didn't return a positive ID,
                                    // but also didn't throw an exception (unlikely with SCOPE_IDENTITY())
                                    return ServiceResult<int>.Failure("Feedback was not added, and no specific error was reported by the SP.", -1);
                                }
                            }
                            else
                            {
                                // This case should ideally not happen if the SP always returns the SELECT statement
                                return ServiceResult<int>.Failure("Stored procedure did not return expected results.", -1);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        // Your existing robust error handling based on ex.State is excellent.
                        switch (ex.State)
                        {
                            case 1:
                                return ServiceResult<int>.Failure(ex.Message, ex.State);
                            case 2:
                                return ServiceResult<int>.Failure(ex.Message, ex.State);
                            default:
                                return ServiceResult<int>.Failure($"An unexpected database error occurred: {ex.Message}", ex.State);
                        }
                    }
                    catch (Exception ex)
                    {
                        return ServiceResult<int>.Failure($"An unexpected error occurred: {ex.Message}", -99); // Generic error code for unhandled exceptions
                    }
                }
            }
        }

        public async Task<ServiceResult<GetFeedbackDTO>> GetIndividualSessionFeedback(int sessionId) // Returns single DTO
        {
            using (SqlConnection connection = new SqlConnection(Connection.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_GetIndividualSessionFeedback", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SessionID", sessionId);

                    try
                    {
                        await connection.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync()) // Use if (await reader.ReadAsync()) instead of while
                            {
                                // Map the single row to a GetFeedbackDTO
                                var feedback = new GetFeedbackDTO
                                {
                                    FeedbackId = reader.GetInt32("FeedbackId"),
                                    SessionId = reader.GetInt32("SessionId"),
                                    SessionType = reader.GetString("SessionType"),
                                    patientID = reader.GetInt32("patientID"),
                                    PatientFirstName = reader.GetString("PatientFirstName"),
                                    PatientLastName = reader.GetString("PatientLastName"),
                                    TherapistID = reader.GetInt32("TherapistID"),
                                    TherapistFirstName = reader.GetString("TherapistFirstName"),
                                    TherapistLastName = reader.GetString("TherapistLastName"),
                                    ReviewText = reader.GetString("ReviewText"),
                                    Rating = reader.GetInt32("Rating")
                                };
                                return ServiceResult<GetFeedbackDTO>.Success(feedback); // Return the single DTO
                            }
                            else
                            {
                                // No feedback found for this session ID
                                return ServiceResult<GetFeedbackDTO>.Failure("No feedback found for this individual session.", 1);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        // ... (existing error handling for SqlException) ...
                        switch (ex.State)
                        {
                            case 1: // RAISERROR('Invalid SessionID. Individual session does not exist.', 16, 1);
                                return ServiceResult<GetFeedbackDTO>.Failure(ex.Message, ex.State);

                            default:
                                return ServiceResult<GetFeedbackDTO>.Failure($"An unexpected database error occurred: {ex.Message}", ex.State);
                        }
                    }
                    catch (Exception ex)
                    {
                        return ServiceResult<GetFeedbackDTO>.Failure($"An unexpected error occurred: {ex.Message}", -99);
                    }
                }
            }
        }
        public async Task<ServiceResult<IEnumerable<GetFeedbackDTO>>> GetTherapistFeedbacks(int therapistId, string? SessionType = null)
        {
            List<GetFeedbackDTO> feedbacks = new List<GetFeedbackDTO>();

            using (SqlConnection connection = new SqlConnection(Connection.ConnectionString)) // Use the injected connection string
            {
                using (SqlCommand cmd = new SqlCommand("usp_GetTherapistFeedbacks", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TherapistID", therapistId);
                    cmd.Parameters.AddWithValue("@SessionType", (object)SessionType ?? DBNull.Value);

                    try
                    {
                        await connection.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (reader.HasRows)
                                {
                                    // Assuming the stored procedure returns these columns
                                    feedbacks.Add(new GetFeedbackDTO
                                    {
                                        FeedbackId = reader.GetInt32("FeedbackId"),
                                        SessionId = reader.GetInt32("SessionId"),
                                        SessionType = reader.GetString("SessionType"),
                                        patientID = reader.GetInt32("patientID"),
                                        PatientFirstName = reader.GetString("PatientFirstName"),
                                        PatientLastName = reader.GetString("PatientLastName"),
                                        TherapistID = reader.GetInt32("TherapistID"),
                                        TherapistFirstName = reader.GetString("TherapistFirstName"),
                                        TherapistLastName = reader.GetString("TherapistLastName"),
                                        ReviewText = reader.GetString("ReviewText"),
                                        Rating = reader.GetInt32("Rating")
                                    });
                                }
                                else
                                {
                                    return ServiceResult<IEnumerable<GetFeedbackDTO>>.Failure("No feedback found for the specified therapist.", -99); // No rows returned
                                }
                            }
                            return ServiceResult<IEnumerable<GetFeedbackDTO>>.Success(feedbacks);
                        }
                    }
                    catch (SqlException ex)
                    {
                        // Handle specific error states from the SP or general SQL errors
                        switch (ex.State)
                        {
                            case 1: // RAISERROR('Invalid TherapistID. Therapist does not exist.', 16, 1);
                                return ServiceResult<IEnumerable<GetFeedbackDTO>>.Failure(ex.Message, ex.State);
                            case 2: // RAISERROR('Invalid TherapistID. Therapist does not exist.', 16, 1);
                                return ServiceResult<IEnumerable<GetFeedbackDTO>>.Failure(ex.Message, ex.State);

                            default:
                                return ServiceResult<IEnumerable<GetFeedbackDTO>>.Failure($"An unexpected database error occurred: {ex.Message}", ex.State);
                        }
                    }
                    catch (Exception ex)
                    {
                        return ServiceResult<IEnumerable<GetFeedbackDTO>>.Failure($"An unexpected error occurred: {ex.Message}", -99);
                    }
                }
            }
        }
    }
}