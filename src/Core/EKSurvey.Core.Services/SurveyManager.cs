using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Entities;

namespace EKSurvey.Core.Services
{
    public class SurveyManager : ISurveyManager
    {
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;

        public SurveyManager(DbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected DbSet<Survey> Surveys => _dbContext.Set<Survey>();

        public IQueryable<Survey> GetActiveSurveys()
        {
            var surveys =
                from s in Surveys.Include(s => s.Tests)
                where s.IsActive &&
                      !s.Deleted.HasValue
                select s;

            return surveys;
        }

        public Task<IQueryable<Survey>> GetActiveSurveysAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var surveys =
                from s in Surveys.Include(s => s.Tests)
                where s.IsActive &&
                      !s.Deleted.HasValue
                select s;

            return Task.FromResult(surveys);
        }

        public ICollection<UserSurvey> GetUserSurveys(string userId, bool includeCompleted = false)
        {
            IEnumerable<UserSurvey> results;

            void Opt(IMappingOperationOptions o) => o.Items.Add("userId", userId);

            if (includeCompleted)
            {
                results = _mapper.Map<IEnumerable<UserSurvey>>(GetActiveSurveys(), Opt);
                return new HashSet<UserSurvey>(results);
            }

            var surveys =
                from s in GetActiveSurveys()
                where !s.Tests.Any(st => st.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && st.Completed.HasValue)
                select s;

            results = _mapper.Map<IEnumerable<UserSurvey>>(surveys, Opt);
            return new HashSet<UserSurvey>(results);
        }

        public async Task<ICollection<UserSurvey>> GetUserSurveysAsync(string userId, bool includeCompleted = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<UserSurvey> results;

            void Opt(IMappingOperationOptions o) => o.Items.Add("userId", userId);

            if (includeCompleted)
            {
                results = _mapper.Map<IEnumerable<UserSurvey>>(await GetActiveSurveysAsync(cancellationToken), Opt);
                return new HashSet<UserSurvey>(results);
            }

            var surveys =
                from s in await GetActiveSurveysAsync(cancellationToken)
                where !s.Tests.Any(st => st.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && st.Completed.HasValue)
                select s;

            results = _mapper.Map<IEnumerable<UserSurvey>>(surveys, Opt);
            return new HashSet<UserSurvey>(results);
        }
    }
}
