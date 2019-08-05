using Kampus.Models;
using Kampus.Persistence.Entities.TaskRelated;

namespace Kampus.Application.Mappers
{
    public interface ITaskMapper
    {
        TaskModel Map(TaskEntry taskEntry);
    }
}
