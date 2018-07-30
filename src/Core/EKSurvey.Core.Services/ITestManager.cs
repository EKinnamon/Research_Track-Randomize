using System.Threading;
using System.Threading.Tasks;
using EKSurvey.Core.Models.Entities;

namespace EKSurvey.Core.Services
{
    public interface ITestManager
    {
        Test Create(int surveyId, string userId);
        Task<Test> CreateAsync(int surveyId, string userId, CancellationToken cancellationToken = default(CancellationToken));

        Test Get(string userId, int surveyId);
        Task<Test> GetAsync(string userId, int surveyId, CancellationToken cancellationToken = default(CancellationToken));

        TestResponse Respond(string userId, int surveyId, string response, int pageId);
        Task<TestResponse> RespondAsync(string userId, int surveyId, string response, int pageId, CancellationToken cancellationToken = default(CancellationToken));
    }
}