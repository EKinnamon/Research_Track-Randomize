using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Data;

namespace EKSurvey.Core.Services
{
    public class SurveyManager
    {
        private readonly SurveyDbContext _dbContext;
        private readonly IMapper _mapper;

        public SurveyManager(SurveyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public ICollection<UserSurvey> GetSurveys(string userId, bool includeCompleted = false)
        {
            var userTests =
                from t in _dbContext.Tests
                where t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) &&
                      !t.Completed.HasValue
                select t;

            if (includeCompleted)
            {
                var surveys =
                    from s in _dbContext.Surveys
                    where !s.Deleted.HasValue
                    select s;

                return new HashSet<UserSurvey>(_mapper.Map<IEnumerable<UserSurvey>>(surveys));
            }
        }
    }
}
