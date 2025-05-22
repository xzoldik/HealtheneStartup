namespace Domain.Dtos.SessionDtos
{
    public class BookSessionResultDTO
    {
        public bool Success { get; set; }
        public int? SessionId { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ErrorCode { get; set; } 
    }
}

