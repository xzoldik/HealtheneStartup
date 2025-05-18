public class SessionDataModel
{
    public int SessionID { get; set; }
    public int TherapistID { get; set; }
    public int PatientID { get; set; }
    public string SessionType { get; set; }
    public DateTime ScheduledStartTime { get; set; }
    public int Duration { get; set; } // In minutes
    public string Status { get; set; } = string.Empty;
    public DateTime? ActualStartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int? FeedbackID { get; set; }
    public string? Description { get; set; }

    // Optional: Add related data if commonly fetched,
    // or create specific "View" models for joined data.
    public string TherapistFirstName { get; set; } // Example from a join
    public string TherapistLastName { get; set; } // Example from a join
    public string PatientFirstName { get; set; }  // Example from a join
    public string PatientLastName { get; set; }   // Example from a join
}