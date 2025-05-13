using Domain.Dtos.TherapistDto;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class TherapistRepo : ITherapist
    {
        public async Task<bool> AddAddAdditionalInformationTherapist(AdditionalInformationTherapistDto request)
        {
            using SqlConnection connection = new SqlConnection(Domain.Globals.Connection.ConnectionString);
            using SqlCommand command = new SqlCommand("sp_AddAdditionalInformationTherapist", connection);
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

        public async Task<List<Therapist>> GetTop3PsychotherapistMatches(int PatientID)
        {
            throw new NotImplementedException();
        }
    }
}

