public class DoctorvailabilityInfo
{
    public long Id { get; set; }
    public string DocumentType { get; set; }
    public string FirstName { get; set; }
    public string IdentityNumber { get; set; }
    public string LastName { get; set; }
    public string Tel { get; set; }
    public string Address { get; set; }
    public string SpeName { get; set; }

    public List<CitaDoctor> Citas { get; set; } = new List<CitaDoctor>();
}

public class CitaDoctor
{
    public short Slot { get; set; }
    public DateTime Appointment_start_time { get; set; }
    public string State { get; set; }
}

public class DoctorAvailabilityResponse
{
    public List<DoctorvailabilityInfo> Doctores { get; set; }
    public int? TotalCount { get; set; }
}
public class DoctorAvailabilityChatResponse
{
    public List<DoctorvailabilityInfo> Doctores { get; set; }

    public List<DateTime> DateRange {get; set;}
    public int SpeId {get; set;}

}

