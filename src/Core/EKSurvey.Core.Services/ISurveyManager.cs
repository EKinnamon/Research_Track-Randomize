using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Entities.Surveys;

namespace EKSurvey.Core.Services
{
    public interface ISurveyManager
    {
        IQueryable<Survey> GetActiveSurveys();
        Task<IQueryable<Survey>> GetActiveSurveysAsync(CancellationToken cancellationToken = default(CancellationToken));

        ICollection<UserSurvey> GetUserSurveys(string userId, bool includeCompleted = false);
        Task<ICollection<UserSurvey>> GetUserSurveysAsync(string userId, bool includeCompleted = false, CancellationToken cancellationToken = default(CancellationToken));

        ICollection<IUserSection> GetUserSections(string userId, int surveyId);
        Task<ICollection<IUserSection>> GetUserSectionsAsync(string userId, int surveyId, CancellationToken cancellationToken = default(CancellationToken));

        IUserSection GetCurrentUserSection(string userId, int surveyId);
        Task<IUserSection> GetCurrentUserSectionAsync(string userId, int surveyId, CancellationToken cancellationToken = default(CancellationToken));

        ICollection<UserPage> GetUserPages(string userId, int sectionId);
        Task<ICollection<UserPage>> GetUserPagesAsync(string userId, int sectionId, CancellationToken cancellationToken = default(CancellationToken));

        UserPage GetCurrentUserPage(string userId, int surveyId);
        Task<UserPage> GetCurrentUserPageAsync(string userId, int surveyId, CancellationToken cancellationToken = default(CancellationToken));

        ICollection<UserResponse> GetSectionResponses(string userId, int sectionId);
        Task<ICollection<UserResponse>> GetSectionResponsesAsync(string userId, int sectionId, CancellationToken cancellationToken = default(CancellationToken));

        UserSurvey GetUserSurvey(string userId, int surveyId);
        Task<UserSurvey> GetUserSurveyAsync(string userId, int surveyId, CancellationToken cancellationToken = default(CancellationToken));
    }
} 