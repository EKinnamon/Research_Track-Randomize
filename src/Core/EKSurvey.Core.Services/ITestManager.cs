using System.Threading;
using System.Threading.Tasks;
using EKSurvey.Core.Models.Entities;

namespace EKSurvey.Core.Services
{
    public interface ITestManager
    {
        Test Create(int surveyId, string userId);
        Task<Test> CreateAsync(int surveyId, string userId, CancellationToken cancellationToken = default(CancellationToken));
    }
}