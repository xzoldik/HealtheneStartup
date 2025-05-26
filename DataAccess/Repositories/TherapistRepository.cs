using Domain.Dtos.TherapistDto;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class TherapistRepository : ITherapistRepository
    {
        public async Task<bool> AddAddAdditionalInformationTherapist(AdditionalInformationTherapistDto request)
        {
            using SqlConnection connection = new SqlConnection(Domain.Globals.Connection.ConnectionString);
            using SqlCommand command = new SqlCommand("usp_AddAdditionalInformationTherapist", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@TherapistId", request.TherapistId);
            command.Parameters.AddWithValue("@Specialization", request.Specialization ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Bio", request.Bio ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Rating", request.Rating ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Diploma", request.Diploma ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@YearsOfExperience", request.YearsOfExperience ?? (object)DBNull.Value);
            try
            {
                await connection.OpenAsync();
                int result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                await connection.CloseAsync();

            }
        }

        public async Task<List<Therapist>> GetTop3PsychotherapistMatches(int UserID)
        {
            using SqlConnection connection = new SqlConnection(Domain.Globals.Connection.ConnectionString);
            using SqlCommand command = new SqlCommand("usp_GetTop3PsychotherapistMatches", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserID", UserID);
            try
            {
                await connection.OpenAsync();
                using SqlDataReader reader = await command.ExecuteReaderAsync();
                List<Therapist> therapists = new List<Therapist>();
                while (await reader.ReadAsync())
                {
                    Therapist therapist = await GetTherapistByUserId(reader.GetInt32(0));
                    therapists.Add(therapist);
                }
                return therapists;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                await connection.CloseAsync();
            }


        }

        public async Task<Therapist> GetTherapistByTherapistId(int TherapistID)
        {

            {
                using SqlConnection connection = new SqlConnection(Domain.Globals.Connection.ConnectionString);
                using SqlCommand command = new SqlCommand("usp_GetTherapistByTherapistId", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TherapistId", TherapistID);
                try
                {
                    await connection.OpenAsync();
                    using SqlDataReader reader = await command.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        return new Therapist
                        {
                            TherapistId = reader.GetInt32(0),
                            UserID = reader.GetInt32(1),
                            Specialization = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Bio = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Rating = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                            Diploma = reader.IsDBNull(5) ? null : reader.GetString(5),
                            YearsOfExperience = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                            TherapistFirstName = reader.GetString(7),
                            TherapistLastName = reader.GetString(8),
                            TherapistEmail = reader.GetString(9),
                            TherapistPhoneNumber = reader.GetString(10),
                            ProfilePicture = reader.IsDBNull(11) ? null : reader.GetString(11),
                            Username = reader.GetString(12),
                            CountryName = reader.GetString(13),
                            Age = reader.GetInt32(14)
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
                finally
                {
                    await connection.CloseAsync();
                }

            }
        }

        public async Task<Therapist> GetTherapistByUserId(int UserID)
        {
            using SqlConnection connection = new SqlConnection(Domain.Globals.Connection.ConnectionString);
            using SqlCommand command = new SqlCommand("usp_GetTherapistByUserId", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserID", UserID);
            try
            {
                await connection.OpenAsync();
                using SqlDataReader reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Therapist
                    {
                        TherapistId = reader.GetInt32(0),
                        UserID = reader.GetInt32(1),
                        Specialization = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Bio = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Rating = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                        Diploma = reader.IsDBNull(5) ? null : reader.GetString(5),
                        YearsOfExperience = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                        TherapistFirstName = reader.GetString(7),
                        TherapistLastName = reader.GetString(8),
                        TherapistEmail = reader.GetString(9),
                        TherapistPhoneNumber = reader.GetString(10),
                        ProfilePicture = reader.IsDBNull(11) ? null : reader.GetString(11),
                        Username = reader.GetString(12),
                        CountryName = reader.GetString(13),
                        Age = reader.GetInt32(14)
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                await connection.CloseAsync();
            }

        }
    }
}

