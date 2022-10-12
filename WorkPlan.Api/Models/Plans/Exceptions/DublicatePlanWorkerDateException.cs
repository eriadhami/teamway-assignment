using Xeptions;

namespace WorkPlan.Api.Models.Plans.Exceptions;

public class DublicatePlanWorkerDateException : Xeption
{
    public DublicatePlanWorkerDateException()
            : base(message: "Dublicate worker and date combination found on plan.") { }
}