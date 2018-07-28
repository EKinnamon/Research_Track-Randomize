using System.Threading;
using System.Threading.Tasks;
using EKSurvey.Core.Models.Entities;

namespace EKSurvey.Core.Services
{
    public interface ITestManager
    {
        Test Create(int surveyId, string userId);
        Task<Test> CreateAsync(int surveyId, string userId, CancellationToken cancellationToken = default(CancellationToken));

        //Test Get(int surveyId, string userId);
        //Task<Test> GetAsync(int surveyId, string userId, CancellationToken cancellationToken = default(CancellationToken));
        //IPage GetPage(int surveyId, string userId);
        //Task<IPage> GetPageAsync(int viewModelSurveyId, string viewModelUserId, CancellationToken cancellationToken = default(CancellationToken));
    }
}