using Domain.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Globals
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public int ErrorCode { get; set; } // Custom error codes from SP/business logic

        public static ServiceResult<T> Success(T data) => new ServiceResult<T> { IsSuccess = true, Data = data };
        public static ServiceResult<T> Failure(string errorMessage, int errorCode = -999) => new ServiceResult<T> { IsSuccess = false, ErrorMessage = errorMessage, ErrorCode = errorCode };

        public static ServiceResult<GroupSession> HandleSqlException(SqlException ex)
        {
            // Process all errors in the exception
            foreach (SqlError error in ex.Errors)
            {
                switch (error.State)
                {
                    case 1:
                        // Session ID cannot be null
                        return ServiceResult<GroupSession>.Failure("Session ID is required.", 1);

                    case 2:
                        // Group session does not exist
                        return ServiceResult<GroupSession>.Failure("Group session not found.", 2);

                    case 3:
                        // Session exists but doesn't match status filter
                        return ServiceResult<GroupSession>.Failure("Group session not found with the specified status.", 3);

                    default:
                        // Log unexpected SQL error for investigation
                        // Consider using ILogger here
                        return ServiceResult<GroupSession>.Failure($"Database error: {error.Message}", 99);
                }
            }

            // Fallback if no errors were processed (shouldn't happen)
            return ServiceResult<GroupSession>.Failure($"Database error: {ex.Message}", 99);
        }

    }




}

