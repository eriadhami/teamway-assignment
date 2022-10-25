using Xeptions;

namespace WorkPlan.Api.Models.Plans.Exceptions;

public class DuplicatePlanWorkerDateException : Xeption
{
    public DuplicatePlanWorkerDateException()
            : base(message: "Dublicate worker and date combination found on plan.") { }
}