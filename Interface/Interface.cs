using System.Threading.Tasks;
using CianPlatform.Models;

namespace CianPlatform.Interface
{
    public interface IDataProcessingCian
    {
        Task<ResultModel> DataProcessingAsync(string project, string building);
    }
}
