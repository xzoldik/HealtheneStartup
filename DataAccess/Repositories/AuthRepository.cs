using Domain.Dtos.AuthDtos;
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
    public class AuthRepository : IAuthRepo
    {
        public Task<bool> CheckUserExistAsync(int userID)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUserDto> FindUserWithEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUserDto> FindUserWithIDAsync(string UserID) 
        {
            using SqlConnection connection = new SqlConnection(Connection.ConnectionString);
            using SqlCommand command = new SqlCommand("usp_FindUserWithID", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserID", UserID);
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                ApplicationUserDto user = new ApplicationUserDto
                {
                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("Phone")),
                    ProfilePicture = reader.IsDBNull(reader.GetOrdinal("ProfilePicture"))
                    ? null
                    :    reader.GetString(reader.GetOrdinal("ProfilePicture")),
                    CountryID = reader.GetInt32(reader.GetOrdinal("CountryID")),
                    Role = reader.GetString(reader.GetOrdinal("UserRole"))
                };
                return user;
            }
            else
            {
                return null;

            }


        } 

        public Task<ApplicationUserDto> FindUserWithPhoneAsync(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUserDto> FindUserWithUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<int> LoginUserWithPhoneNumberAsync(LoginWithPhoneNumberDTO request)
        {
            using SqlConnection connection = new SqlConnection(Connection.ConnectionString);
            using SqlCommand command = new SqlCommand("usp_LoginWithPhone", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Phone", request.Phone);

            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                int userID = reader.GetInt32(reader.GetOrdinal("UserID"));
                string storedHash = reader.GetString(reader.GetOrdinal("PasswordHash"));

                if (PasswordHasher.VerifyPassword(request.Password, storedHash))
                {
                    return userID; // Login successful
                }
            }

            return -1; // Login failed
        }


        public async Task<int> LoginUserWithUsernameOrEmailAsync(LoginWithEmailOrUsernameDTO request)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(Connection.ConnectionString);
                using SqlCommand command = new SqlCommand("usp_LoginUserWithUsernameOrEmail", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@LoginIdentifier", request.LoginIdentifier);

                await connection.OpenAsync();
                using SqlDataReader reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    // Retrieve the UserID and PasswordHash from the database
                    int userID = reader.GetInt32(reader.GetOrdinal("UserID"));
                    string storedHash = reader.GetString(reader.GetOrdinal("PasswordHash"));

                    // Verify the password
                    if (PasswordHasher.VerifyPassword(request.Password, storedHash))
                    {
                        return userID; // Login successful
                    }
                    else
                    {
                        throw new Exception("Invalid login credentials."); // Password mismatch
                    }
                }
                else
                {
                    throw new Exception("Invalid login credentials."); // No user found
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                throw new Exception("An error occurred while logging in.", ex);
            }
        }


        public async Task<int> RegisterUserAsync(RegisterApplicationUserDTO user)
        {
            using SqlConnection connection = new SqlConnection(Connection.ConnectionString);
            using SqlCommand command = new SqlCommand("usp_RegisterUserApplication", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Phone", user.PhoneNumber);
            command.Parameters.AddWithValue("@PasswordHash", PasswordHasher.HashPassword(user.Password));
            command.Parameters.AddWithValue("@UserRole", user.Role);
            command.Parameters.AddWithValue("@CountryID", user.CountryID);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@ProfilePicture", user.ProfilePicture);

            // Add return parameter
            SqlParameter returnValue = new SqlParameter("@UserID", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.ReturnValue;
            command.Parameters.Add(returnValue);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return (int)returnValue.Value; // Return the actual result from the stored procedure
        }


    }
}
