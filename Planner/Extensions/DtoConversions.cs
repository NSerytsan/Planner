
using Planner.Dtos.Plan;

namespace Planner.Extensions
{
    public static class DtoConversion
    {
        public static PlanDto ToPlanDto(this Planner.Data.Plan plan)
        {
            return new PlanDto()
            {
                id = plan.Id,
                title = plan.Name,
                description = plan.Description,
                start = plan.StartDate,
                end = plan.EndDate,
                color = plan.Color
            };
        }

        public static IEnumerable<PlanDto> ToPlanDtos(this IEnumerable<Planner.Data.Plan> plans)
        {
            return from plan in plans
                   select plan.ToPlanDto();
        }
    }
}