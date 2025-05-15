namespace Domain.Dtos.SessionDtos
{
    public class BookSessionResultDTO
    {
        public bool Success { get; set; }
        public int? SessionId { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; } // Optional: for client-side logic
    }
}

