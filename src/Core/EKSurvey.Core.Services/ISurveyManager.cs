using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EKSurvey.Core.Models.DataTransfer;

namespace EKSurvey.Core.Services
{
    public interface ISurveyManager
    {
        ICollection<UserSurvey> GetUserSurveys(string userId, bool includeCompleted = false);
        Task<ICollection<UserSurvey>> GetSurveysAsync(string userId, bool includeCompleted = false, CancellationToken cancellationToken = default(CancellationToken));
    }
}