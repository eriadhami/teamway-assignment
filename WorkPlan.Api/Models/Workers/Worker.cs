using System.Text.Json.Serialization;
using WorkPlan.Api.Models.Plans;

namespace WorkPlan.Api.Models.Workers;

public class Worker
{
    public Guid ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [JsonIgnore]
    public IEnumerable<Plan> Plans { get; set; }
}