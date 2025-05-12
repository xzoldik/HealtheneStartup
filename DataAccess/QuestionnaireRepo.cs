using System.Data.SqlClient;
using System.Threading.Tasks; 
using System.Collections.Generic;
using System;
using System.Linq;
using Domain.Models;
using Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Domain.Globals;
using Domain.Dtos.QuestionnaireDtos;
using System.Data;


namespace DataAccess
{
    public class QuestionnaireRepo : IQuestionnaireRepo
    {



        public Task<Questionnaire> CreateQuestionnaireAsync(Questionnaire questionnaire)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteQuestionnaireAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Questionnaire>> GetAllQuestionnairesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Questionnaire?> GetQuestionnaireByIdAsync(int id)
        {
            Questionnaire? questionnaire = null; // Use nullable Questionnaire
            using (var connection = new SqlConnection(Connection.ConnectionString))
            {
                await connection.OpenAsync();
               

                using (SqlCommand command = new SqlCommand("sp_GetQuestionnaireById", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@QuestionnaireID", id);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (questionnaire == null)
                            {
                                questionnaire = new Questionnaire
                                {
                                    QuestionnaireID = (int)reader["QuestionnaireID"],
                                    QuestionnaireTitle = reader["QuestionnaireTitle"].ToString()!,
                                    QuestionnaireDescription = reader["QuestionnaireDescription"].ToString()!,
                                    Questions = new List<Question>()
                                };
                            }

                            int questionId = (int)reader["QuestionID"];
                            Question? currentQuestion = questionnaire.Questions.FirstOrDefault(q => q.QuestionID == questionId); // Use nullable Question

                            if (currentQuestion == null)
                            {
                                currentQuestion = new Question
                                {
                                    QuestionID = questionId,
                                    QuestionnaireID = (int)reader["QuestionnaireID"],
                                    QuestionContent = reader["QuestionContent"].ToString()!,
                                    QuestionOrder = (int)reader["QuestionOrder"],
                                    Answers = new List<Answer>()
                                };
                                questionnaire.Questions.Add(currentQuestion);
                            }

                            if (reader["AnswerID"] != DBNull.Value)
                            {
                                currentQuestion.Answers.Add(new Answer
                                {
                                    AnswerID = (int)reader["AnswerID"],
                                    QuestionID = (int)reader["QuestionID"],
                                    OptionText = reader["OptionText"].ToString()!,
                                    Score = reader["Score"] == DBNull.Value ? (int?)null : (int)reader["Score"] // Use nullable int?
                                });
                            }
                        }
                    }
                }
            }
            return questionnaire;
        }

        public Task<bool> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

            public async Task<bool> SavePatientPreferencesAsync(PreferencesPatientDto preferences)
            {
                using SqlConnection connection = new SqlConnection(Connection.ConnectionString);
                using SqlCommand command = new SqlCommand("sp_SavePreferences", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@UserID", preferences.UserID);
                command.Parameters.AddWithValue("@UserRole", preferences.UserRole);
                command.Parameters.AddWithValue("@Gender", preferences.Gender);
                command.Parameters.AddWithValue("@Religion", preferences.Religion);
                command.Parameters.AddWithValue("@TreatingExperience", preferences.TreatingExperience ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Age", preferences.Age);
                command.Parameters.AddWithValue("@MaritalStatus", preferences.MaritalStatus);
                command.Parameters.AddWithValue("@ReligionImportance", preferences.ReligionImportance);
                command.Parameters.AddWithValue("@PsychiatryTreatment", preferences.PsychiatryTreatment ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@TreatmentReason", preferences.TreatmentReason ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PhysicalHealth", preferences.PhysicalHealth ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FoodHabits", preferences.FoodHabits ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Depression", preferences.Depression ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Employment", preferences.Employment ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Intimacy", preferences.Intimacy ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Alcoholic", preferences.Alcoholic ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Suicide", preferences.Suicide ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PreferedLanguage", preferences.PreferedLanguage);
                command.Parameters.AddWithValue("@PreferedGender", preferences.PreferedGender);
                command.Parameters.AddWithValue("@ExpectationTherapist", preferences.ExpectationTherapist ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DirectOrGentel", preferences.DirectOrGentel);
                command.Parameters.AddWithValue("@StructuredOrFlexible", preferences.StructuredOrFlexible);
                command.Parameters.AddWithValue("@OfficialOrCasual", preferences.OfficialOrCasual);

                try
                {
                    await connection.OpenAsync();
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
                catch (SqlException sqlEx)
                {
                    // Log SQL-specific exceptions
                    Console.WriteLine($"SQL Error: {sqlEx.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    // Log general exceptions
                    Console.WriteLine($"Error: {ex.Message}");
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }

        public async Task<bool> SaveTherapistPreferencesAsync(PreferencesTherapistDto preferences)
        {
            using SqlConnection connection = new SqlConnection(Connection.ConnectionString);
            using SqlCommand command = new SqlCommand("sp_SaveTherapistPreferences", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserID", preferences.UserID);
            command.Parameters.AddWithValue("@UserRole", preferences.UserRole);
            command.Parameters.AddWithValue("@Gender", preferences.Gender);
            command.Parameters.AddWithValue("@Religion", preferences.Religion);
            command.Parameters.AddWithValue("@TreatingExperience", preferences.TreatingExperience ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Age", preferences.Age);
            command.Parameters.AddWithValue("@MaritalStatus", preferences.MaritalStatus);
            command.Parameters.AddWithValue("@ReligionImportance", preferences.ReligionImportance);
            command.Parameters.AddWithValue("@PsychiatryTreatment", preferences.PsychiatryTreatment ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Alcoholic", preferences.Alcoholic ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Suicide", preferences.Suicide ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PreferedLanguage", preferences.PreferedLanguage);
            command.Parameters.AddWithValue("@PreferedGender", preferences.PreferedGender);
            command.Parameters.AddWithValue("@DirectOrGentel", preferences.DirectOrGentel);
            command.Parameters.AddWithValue("@StructuredOrFlexible", preferences.StructuredOrFlexible);
            command.Parameters.AddWithValue("@OfficialOrCasual", preferences.OfficialOrCasual);

            try
            {
                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (SqlException sqlEx)
            {
                // Log SQL-specific exceptions
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Log general exceptions
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public Task<Questionnaire?> UpdateQuestionnaireAsync(Questionnaire questionnaire)
        {
            throw new NotImplementedException();
        }



    }
}