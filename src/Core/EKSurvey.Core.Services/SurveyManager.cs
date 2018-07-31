using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Entities;
using EKSurvey.Core.Models.Enums;
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
        protected DbSet<Test> Tests => _dbContext.Set<Test>();

        private Action<IMappingOperationOptions> Opt(string userId) => o =>
        {
            o.Items.Add("dbContext", _dbContext);
            o.Items.Add("userId", userId);
        };

        private static UserSection SelectSection(IList<UserSection> sections, SelectorType selectorType)
        {
            var rng = new Random();
            var selectionIndex = 0;
            switch (selectorType)
            {
                case SelectorType.Random:
                    selectionIndex = rng.Next(0, sections.Count);
                    break;
                case SelectorType.ResponseStandardDeviation:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(selectorType), selectorType, null);
            }

            return sections[selectionIndex];
        }
        private void MakeActiveSection(UserSection userSection)
        {
            var section = Sections.Find(userSection.Id) ?? throw new SectionNotFoundException(userSection.Id);

            var sectionMarker = new TestSectionMarker
            {
                TestId = userSection.TestId,
                SectionId = userSection.Id,
                Started = DateTime.UtcNow,
            };

            section.TestSectionMarkers.Add(sectionMarker);
            _dbContext.SaveChanges();
            userSection = _mapper.Map<UserSection>(section, Opt(userSection.UserId));
        }
        private async Task MakeActiveSectionAsync(UserSection userSection)
        {
            var section = await Sections.FindAsync(userSection.Id) ?? throw new SectionNotFoundException(userSection.Id);
            var sectionMarker = new TestSectionMarker
            {
                TestId = userSection.TestId,
                SectionId = userSection.Id,
                Started = DateTime.UtcNow
            };

            section.TestSectionMarkers.Add(sectionMarker);
            await _dbContext.SaveChangesAsync();
            userSection = _mapper.Map<UserSection>(section, Opt(userSection.UserId));
        }

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

        public async Task<ICollection<UserSection>> GetUserSectionsAsync(string userId, int surveyId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var survey = await Surveys.FindAsync(cancellationToken, surveyId) ?? throw new SurveyNotFoundException(surveyId);
            var results = _mapper.Map<ICollection<UserSection>>(survey.Sections, Opt(userId));
            return new HashSet<UserSection>(results.OrderBy(i => i.Order));
        }

        public UserSection GetCurrentUserSection(string userId, int surveyId)
        {
            var survey = Surveys.Find(surveyId) ?? throw new SurveyNotFoundException(surveyId);
            var sections = GetUserSections(userId, surveyId);
            var activeSection = sections.FirstOrDefault(s => s.Started.HasValue && !s.Completed.HasValue);

            if (activeSection != null)
                return activeSection;
            
            // create a new active section
            var availableSections =
                from s in sections
                where !s.Started.HasValue
                orderby s.Order
                select s;

            var sectionStacks =
                from s in availableSections
                group s by s.Order
                into st
                select new { Order = st.Key, Stack = st.ToList() };

            var stack = sectionStacks
                .OrderBy(st => st.Order)
                .FirstOrDefault()?.Stack ?? new List<UserSection>();

            if (!stack.Any())
                return null;

            activeSection = stack.Count == 1 
                ? stack.First() 
                : SelectSection(stack, stack.First().SelectorType.GetValueOrDefault());

            MakeActiveSection(activeSection);

            return activeSection;
        }

        public async Task<UserSection> GetCurrentUserSectionAsync(string userId, int surveyId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var survey = await Surveys.FindAsync(cancellationToken, surveyId) ?? throw new SurveyNotFoundException(surveyId);
            var sections = await GetUserSectionsAsync(userId, surveyId, cancellationToken);
            var activeSection = sections.FirstOrDefault(s => s.Started.HasValue && !s.Completed.HasValue);

            if (activeSection != null)
                return activeSection;

            // create a new active section;
            var availableSections = 
                from s in sections
                where !s.Started.HasValue
                orderby s.Order
                select s;

            var sectionStacks =
                from s in availableSections
                group s by s.Order
                into st
                select new { Order = st.Key, Stack = st.ToList() };

            var stack = sectionStacks
                .OrderBy(st => st.Order)
                .FirstOrDefault()?.Stack ?? new List<UserSection>();

            if (!stack.Any())
                return null;

            activeSection = stack.Count == 1
                ? stack.First()
                : SelectSection(stack, stack.First().SelectorType.GetValueOrDefault());

            await MakeActiveSectionAsync(activeSection);

            return activeSection;
        }

        public ICollection<UserPage> GetUserPages(string userId, int sectionId)
        {
            var section = Sections.Find(sectionId) ?? throw new SectionNotFoundException(sectionId);
            var results = _mapper.Map<ICollection<UserPage>>(section.Pages, Opt(userId));
            return new HashSet<UserPage>(results.OrderBy(i => i.Page.Order));
        }

        public async Task<ICollection<UserPage>> GetUserPagesAsync(string userId, int sectionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var section = await Sections.FindAsync(cancellationToken, sectionId) ?? throw new SectionNotFoundException(sectionId);
            var results = _mapper.Map<ICollection<UserPage>>(section.Pages, Opt(userId));
            return new HashSet<UserPage>(results.OrderBy(i => i.Page.Order));
        }

        public UserPage GetCurrentUserPage(string userId, int surveyId)
        {
            var activeSection = GetCurrentUserSection(userId, surveyId);
            var pages = GetUserPages(userId, activeSection.Id);

            return pages.OrderBy(p => p.Page.Order).FirstOrDefault(p => !p.Responded.HasValue);
        }

        public async Task<UserPage> GetCurrentUserPageAsync(string userId, int surveyId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var activeSection = await GetCurrentUserSectionAsync(userId, surveyId, cancellationToken);
            var pages = await GetUserPagesAsync(userId, activeSection.Id, cancellationToken);

            return pages.OrderBy(p => p.Page.Order).FirstOrDefault(p => !p.Responded.HasValue);
        }

        public ICollection<UserResponse> GetSectionResponses(string userId, int id)
        {
            var section = Sections.Find(id) ?? throw new SectionNotFoundException(id);
            var test = Tests.Find(userId, section.SurveyId) ?? throw new TestNotFoundException(userId, id);

            var sectionResponses =
                from p in section.Pages
                from r in p.TestResponses
                where r.TestId == test.Id
                orderby r.Page.Order
                select r;

            var responses = _mapper.Map<ICollection<UserResponse>>(sectionResponses, Opt(userId));

            return responses;
        }

        public async Task<ICollection<UserResponse>> GetSectionResponsesAsync(string userId, int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var section = await Sections.FindAsync(cancellationToken, id) ?? throw new SectionNotFoundException(id);
            var test = await Tests.FindAsync(cancellationToken, userId, section.SurveyId) ?? throw new TestNotFoundException(userId, id);

            var sectionResponses =
                from p in section.Pages
                from r in p.TestResponses
                where r.TestId == test.Id
                orderby r.Page.Order
                select r;

            var responses = _mapper.Map<ICollection<UserResponse>>(sectionResponses, Opt(userId));

            return responses;
        }
    }
}
