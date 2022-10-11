using WorkPlan.Api.Models.Shifts;
using WorkPlan.Api.Models.Workers;

namespace WorkPlan.Api.Models.Plans;

public class Plan
{
    public Guid ID { get; set; }

    public Guid WorkerID { get; set; }
    public Worker Worker { get; set; }

    public Guid ShiftID { get; set; }
    public Shift Shift { get; set; }

    public DateOnly Date { get; set; }
}