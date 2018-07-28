using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Entities;
using EKSurvey.Core.Services.Exceptions;

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
        protected DbSet<Section> Sections => _dbContext.Set<Section>();
        protected DbSet<Page> Pages => _dbContext.Set<Page>();

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

        private static Action<IMappingOperationOptions> Opt(string userId) => o => o.Items.Add("userId", userId);

        public ICollection<UserSurvey> GetUserSurveys(string userId, bool includeCompleted = false)
        {
            IEnumerable<UserSurvey> results;

            if (includeCompleted)
            {
                results = _mapper.Map<IEnumerable<UserSurvey>>(GetActiveSurveys(), Opt(userId));
                return new HashSet<UserSurvey>(results);
            }

            var surveys =
                from s in GetActiveSurveys()
                where !s.Tests.Any(st => st.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && st.Completed.HasValue)
                select s;

            results = _mapper.Map<IEnumerable<UserSurvey>>(surveys, Opt(userId));
            return new HashSet<UserSurvey>(results);
        }

        public async Task<ICollection<UserSurvey>> GetUserSurveysAsync(string userId, bool includeCompleted = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<UserSurvey> results;

            if (includeCompleted)
            {
                results = _mapper.Map<IEnumerable<UserSurvey>>(await GetActiveSurveysAsync(cancellationToken), Opt(userId));
                return new HashSet<UserSurvey>(results);
            }

            var surveys =
                from s in await GetActiveSurveysAsync(cancellationToken)
                where !s.Tests.Any(st => st.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && st.Completed.HasValue)
                select s;

            results = _mapper.Map<IEnumerable<UserSurvey>>(surveys, Opt(userId));
            return new HashSet<UserSurvey>(results);
        }

        public ICollection<UserSection> GetUserSections(string userId, int surveyId)
        {
            var survey = Surveys.Find(surveyId) ?? throw new SurveyNotFoundException(surveyId);
            var results = _mapper.Map<ICollection<UserSection>>(survey.Sections, Opt(userId));
            return new HashSet<UserSection>(results.OrderBy(i => i.Order));
        }

        public async Task<ICollection<UserSection>> GetUserSectionsAsync(string userId, int surveyId)
        {
            var survey = await Surveys.FindAsync(surveyId) ?? throw new SurveyNotFoundException(surveyId);
            var results = _mapper.Map<ICollection<UserSection>>(survey.Sections, Opt(userId));
            return new HashSet<UserSection>(results.OrderBy(i => i.Order));
        }

        public UserPage GetCurrentUserPage(string userId, int surveyId)
        {
            var sections = GetUserSections(userId, surveyId);
            var currentSection = sections.First(s => !s.Completed.HasValue);
            var pages = GetUserPages(userId, currentSection.Id);

            return pages.OrderBy(p => p.Page.Order).FirstOrDefault(p => !p.Responded.HasValue);
        }

        public  ICollection<UserPage> GetUserPages(string userId, int sectionId)
        {
            var section = Sections.Find(sectionId) ?? throw new SectionNotFoundException(sectionId);
            var results = _mapper.Map<ICollection<UserPage>>(section.Pages, Opt(userId));
            return new HashSet<UserPage>(results.OrderBy(i => i.Page.Order));
        }


        public Task<UserPage> GetCurrentUserPageAsync(string userId, int surveyId, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

    }
}
