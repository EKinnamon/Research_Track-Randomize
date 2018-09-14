using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using EKSurvey.Core.Models.Comparers;
using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Entities;
using EKSurvey.Core.Models.Enums;
using EKSurvey.Core.Services.Exceptions;
using MoreLinq.Extensions;

namespace EKSurvey.Core.Services
{
    public class SurveyManager : ISurveyManager
    {
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly Random _rng;

        public SurveyManager(DbContext dbContext, Random rng, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _rng = rng ?? throw new ArgumentNullException(nameof(rng));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected DbSet<Survey> Surveys => _dbContext.Set<Survey>();
        protected DbSet<Section> Sections => _dbContext.Set<Section>();
        protected DbSet<Page> Pages => _dbContext.Set<Page>();
        protected DbSet<Test> Tests => _dbContext.Set<Test>();
        protected DbSet<TestSectionMarker> TestSectionMarkers => _dbContext.Set<TestSectionMarker>();
        protected DbSet<TestResponse> TestResponses => _dbContext.Set<TestResponse>();

        private UserSection SelectSection(UserSectionGroup sectionGroup)
        {
            var selectionIndex = 0;
            switch (sectionGroup.SelectorType.GetValueOrDefault(SelectorType.Random))
            {
                case SelectorType.Random:
                    selectionIndex = _rng.Next(0, sectionGroup.Count);
                    break;
                case SelectorType.ResponseStandardDeviation:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sectionGroup.SelectorType), sectionGroup.SelectorType, null);
            }

            return sectionGroup[selectionIndex];
        }
        private void MakeActiveSection(IUserSection userSection)
        {
            var sectionId = default(int);
            switch (userSection)
            {
                case UserSection _:
                    sectionId = userSection.Id.GetValueOrDefault();
                    break;
                case UserSectionGroup group:
                    var selectedSection = SelectSection(group);
                    sectionId = selectedSection.Id.GetValueOrDefault();
                    break;
            }

            var section = Sections.Find(sectionId) ?? throw new SectionNotFoundException(sectionId);
            var sectionMarker = new TestSectionMarker
            {
                TestId = userSection.TestId,
                Section = section,
                SectionId = sectionId,
                Started = DateTime.UtcNow
            };

            TestSectionMarkers.Add(sectionMarker);
            _dbContext.SaveChanges();
        }
        private async Task MakeActiveSectionAsync(IUserSection userSection, CancellationToken cancellationToken = default(CancellationToken))
        {
            var sectionId = default(int);
            switch (userSection)
            {
                case UserSection _:
                    sectionId = userSection.Id.GetValueOrDefault();
                    break;
                case UserSectionGroup group:
                    var selectedSection = SelectSection(group);
                    sectionId = selectedSection.Id.GetValueOrDefault();
                    break;
            }

            var section = await Sections.FindAsync(cancellationToken, sectionId) ?? throw new SectionNotFoundException(sectionId);
            var sectionMarker = new TestSectionMarker
            {
                TestId = userSection.TestId,
                Section = section,
                SectionId = sectionId,
                Started = DateTime.UtcNow
            };

            TestSectionMarkers.Add(sectionMarker);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<Survey> GetActiveSurveys()
        {
            var surveys =
                from s in Surveys.Include(t => t.Tests)
                where s.IsActive &&
                      !s.Deleted.HasValue
                select s;

            return surveys;
        }

        public Task<IQueryable<Survey>> GetActiveSurveysAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var surveys =
                from s in Surveys.Include(t => t.Tests)
                where s.IsActive &&
                      !s.Deleted.HasValue
                select s;

            return Task.FromResult(surveys);
        }

        public ICollection<UserSurvey> GetUserSurveys(string userId, bool includeCompleted = false)
        {
            var userSurveys = new HashSet<UserSurvey>(EntityComparer<UserSurvey>.Id);
            var surveys = GetActiveSurveys();

            var tests = from s in surveys
                from t in s.Tests
                where t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase)
                select t;

            var unansweredSurveys =
                from s in surveys
                where !s.Tests.Any(t => t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                select s;

            userSurveys.UnionWith(_mapper.Map<IEnumerable<UserSurvey>>(tests.Where(t => !t.Completed.HasValue)));
            userSurveys.UnionWith(_mapper.Map<IEnumerable<UserSurvey>>(unansweredSurveys)
                .Select(us =>
                {
                    us.UserId = userId;
                    return us;
                }));

            if (includeCompleted)
            {
                userSurveys.UnionWith(_mapper.Map<IEnumerable<UserSurvey>>(tests.Where(t => t.Completed.HasValue)));
            }

            return userSurveys;
        }

        public async Task<ICollection<UserSurvey>> GetUserSurveysAsync(string userId, bool includeCompleted = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userSurveys = new HashSet<UserSurvey>(EntityComparer<UserSurvey>.Id);
            var surveys = await GetActiveSurveysAsync(cancellationToken);

            var tests = from s in surveys
                from t in s.Tests
                where t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase)
                select t;

            var unansweredSurveys =
                from s in surveys
                where !s.Tests.Any(t => t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                select s;

            userSurveys.UnionWith(_mapper.Map<IEnumerable<UserSurvey>>(tests.Where(t => !t.Completed.HasValue)));
            userSurveys.UnionWith(_mapper.Map<IEnumerable<UserSurvey>>(unansweredSurveys)
                .Select(us =>
                {
                    us.UserId = userId;
                    return us;
                }));

            if (includeCompleted)
            {
                userSurveys.UnionWith(_mapper.Map<IEnumerable<UserSurvey>>(tests.Except(tests.Where(t => !t.Completed.HasValue))));
            }

            return userSurveys;
        }

        public ICollection<IUserSection> GetUserSections(string userId, int surveyId)
        {
            ThrowIfSurveyDoesNotExist(surveyId);

            var sectionStacks = Sections
                .Where(s => s.SurveyId == surveyId)
                .GroupBy(s => s.Order)
                .ToList();

            var userSections = sectionStacks
                .OrderBy(ss => ss.Key)
                .Select<IGrouping<int,Section>, IUserSection>(ss =>
                {
                    var sectionMarker = ss
                        .SelectMany(sm => sm.TestSectionMarkers)
                        .SingleOrDefault(tsm => tsm.Test.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));

                    switch (ss.Count())
                    {
                        case 1 when sectionMarker != null:
                            return _mapper.Map<UserSection>(sectionMarker);
                        case 1:
                            var userSection = _mapper.Map<UserSection>(ss.Single());
                            userSection.UserId = userId;
                            return userSection;
                        default:
                            // fall through to process group section
                            break;
                    }

                    var sectionGroup = _mapper.Map<UserSectionGroup>(ss);
                    sectionGroup.Add(_mapper.Map<UserSection>(sectionMarker));

                    return sectionGroup;
                }).ToList();

            return userSections.OrderBy(i => i.Order).ToHashSet();
        }

        public async Task<ICollection<IUserSection>> GetUserSectionsAsync(string userId, int surveyId, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfSurveyDoesNotExist(surveyId);

            var sectionStacks = await Sections
                .Where(s => s.SurveyId == surveyId)
                .GroupBy(s => s.Order)
                .ToListAsync(cancellationToken);

            var userSections = sectionStacks
                .OrderBy(ss => ss.Key)
                .Select<IGrouping<int, Section>, IUserSection>(ss =>
                {
                    var sectionMarker = ss
                        .SelectMany(sm => sm.TestSectionMarkers)
                        .SingleOrDefault(tsm => tsm.Test.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));

                    switch (ss.Count())
                    {
                        case 1 when sectionMarker != null:
                            return _mapper.Map<UserSection>(sectionMarker);
                        case 1:
                            var userSection = _mapper.Map<UserSection>(ss.Single());
                            userSection.UserId = userId;
                            return userSection;
                        default:
                            // fall through to process group section
                            break;
                    }

                    var sectionGroup = _mapper.Map<UserSectionGroup>(ss);
                    sectionGroup.Add(_mapper.Map<UserSection>(sectionMarker));

                    return sectionGroup;
                }).ToList();

            return userSections.OrderBy(i => i.Order).ToHashSet();
        }

        private void ThrowIfSurveyDoesNotExist(int surveyId)
        {
            var survey = GetActiveSurveys().SingleOrDefault(s => s.Id == surveyId);

            if (survey == null)
                throw new SurveyNotFoundException(surveyId);
        }

        private async Task ThrowIfSurveyDoesNotExistAsync(int surveyId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var survey = await (await GetActiveSurveysAsync(cancellationToken)).SingleOrDefaultAsync(s => s.Id == surveyId, cancellationToken);

            if (survey == null)
                throw new SurveyNotFoundException(surveyId);
        }

        public IUserSection GetCurrentUserSection(string userId, int surveyId)
        {
            ThrowIfSurveyDoesNotExist(surveyId);
            var sections = GetUserSections(userId, surveyId);
            var activeSection = sections.FirstOrDefault(s => s.Started.HasValue && !s.Completed.HasValue);

            if (activeSection != null)
                return activeSection;

            var availableSections =
                from s in sections
                where !s.Started.HasValue
                orderby s.Order
                select s;

            activeSection = availableSections.FirstOrDefault();

            MakeActiveSection(activeSection);

            sections = GetUserSections(userId, surveyId);
            activeSection = sections.FirstOrDefault(s => s.Started.HasValue && !s.Completed.HasValue);

            return activeSection;
        }

        public async Task<IUserSection> GetCurrentUserSectionAsync(string userId, int surveyId, CancellationToken cancellationToken = default(CancellationToken))
        {
            await ThrowIfSurveyDoesNotExistAsync(surveyId, cancellationToken);
            var sections = await GetUserSectionsAsync(userId, surveyId, cancellationToken);
            var activeSection = sections.FirstOrDefault(s => s.Started.HasValue && !s.Completed.HasValue);

            if (activeSection != null)
                return activeSection;

            var availableSections =
                from s in sections
                where !s.Started.HasValue
                orderby s.Order
                select s;

            activeSection = availableSections.FirstOrDefault();

            await MakeActiveSectionAsync(activeSection, cancellationToken);

            sections = await GetUserSectionsAsync(userId, surveyId, cancellationToken);
            activeSection = sections.FirstOrDefault(s => s.Started.HasValue && !s.Completed.HasValue);

            return activeSection;
        }

        public ICollection<UserPage> GetUserPages(string userId, int sectionId)
        {
            var section = Sections.Find(sectionId) ?? throw new SectionNotFoundException(sectionId);
            var responses = section
                .Pages
                .SelectMany(p => p.TestResponses)
                .Where(tr => tr.Test.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));

            var results = _mapper.Map<ICollection<UserPage>>(responses);
            return results.ToHashSet();
        }

        public async Task<ICollection<UserPage>> GetUserPagesAsync(string userId, int sectionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var section = await Sections.FindAsync(cancellationToken, sectionId) ?? throw new SectionNotFoundException(sectionId);
            var responses = section
                .Pages
                .SelectMany(p => p.TestResponses)
                .Where(tr => tr.Test.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));

            var results = _mapper.Map<ICollection<UserPage>>(responses);
            return results.ToHashSet();
        }

        public UserPage GetCurrentUserPage(string userId, int surveyId)
        {
            var activeSection = GetCurrentUserSection(userId, surveyId);
            var pages = GetUserPages(userId, activeSection.Id.GetValueOrDefault());

            return pages.OrderBy(p => p.Page.Order).FirstOrDefault(p => !p.Responded.HasValue);
        }

        public async Task<UserPage> GetCurrentUserPageAsync(string userId, int surveyId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var activeSection = await GetCurrentUserSectionAsync(userId, surveyId, cancellationToken);
            var pages = await GetUserPagesAsync(userId, activeSection.Id.GetValueOrDefault(), cancellationToken);

            return pages.OrderBy(p => p.Page.Order).FirstOrDefault(p => !p.Responded.HasValue);
        }

        public ICollection<UserResponse> GetSectionResponses(string userId, int sectionId)
        {
            var section = Sections.Find(sectionId) ?? throw new SectionNotFoundException(sectionId);
            var test = Tests.SingleOrDefault(t => t.UserId.Equals(userId,StringComparison.OrdinalIgnoreCase) && t.SurveyId == section.SurveyId) ?? throw new TestNotFoundException(userId, sectionId);

            var sectionResponses =
                from p in section.Pages
                from r in p.TestResponses
                where r.TestId == test.Id
                orderby r.Page.Order
                select r;

            var responses = _mapper.Map<ICollection<UserResponse>>(sectionResponses);

            return responses;
        }

        public async Task<ICollection<UserResponse>> GetSectionResponsesAsync(string userId, int sectionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var section = await Sections.FindAsync(cancellationToken, sectionId) ?? throw new SectionNotFoundException(sectionId);
            var test = await Tests.SingleOrDefaultAsync(t => t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && t.SurveyId == section.SurveyId, cancellationToken) ?? throw new TestNotFoundException(userId, sectionId);

            var sectionResponses =
                from p in section.Pages
                from r in p.TestResponses
                where r.TestId == test.Id
                orderby r.Page.Order
                select r;

            var responses = _mapper.Map<ICollection<UserResponse>>(sectionResponses);

            return responses;
        }

        public UserSurvey GetUserSurvey(string userId, int surveyId)
        {
            var survey = GetActiveSurveys().SingleOrDefault(s => s.Id == surveyId);
            if (survey == null)
                return null;

            var test = survey.Tests.SingleOrDefault(t => t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));
            var userSurvey = test != null 
                ? _mapper.Map<UserSurvey>(test) 
                : _mapper.Map<UserSurvey>(survey);

            return userSurvey;
        }

        public async Task<UserSurvey> GetUserSurveyAsync(string userId, int surveyId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var survey = (await GetActiveSurveysAsync(cancellationToken)).SingleOrDefault(s => s.Id == surveyId);
            if (survey == null)
                return null;

            var test = survey.Tests.SingleOrDefault(t => t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));
            var userSurvey = test != null 
                ? _mapper.Map<UserSurvey>(test) 
                : _mapper.Map<UserSurvey>(survey);

            return userSurvey;
        }
    }
}
