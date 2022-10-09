namespace WorkPlan.Api.Models.Shifts;

public class Shift
{
    public Guid ID { get; set; }
    public string Name { get; set; }
    public TimeOnly StartHour { get; set; }
    public TimeOnly EndHour { get; set; }
}