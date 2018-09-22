using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Entities;
using EKSurvey.Core.Models.Entities.Surveys;
using EKSurvey.Core.Services.Exceptions;

namespace EKSurvey.Core.Services
{
    public class TestManager : ITestManager
    {
        private readonly DbContext _dbContext;
        private readonly ISurveyManager _surveyManager;
        private readonly IMapper _mapper;

        public DbSet<Survey> Surveys => _dbContext.Set<Survey>();
        public DbSet<Section> Sections => _dbContext.Set<Section>();
        public DbSet<Test> Tests => _dbContext.Set<Test>();
        public DbSet<TestResponse> TestResponses => _dbContext.Set<TestResponse>();
        public DbSet<Page> Pages => _dbContext.Set<Page>();
        public DbSet<TestSectionMarker> TestSectionMarkers => _dbContext.Set<TestSectionMarker>();

        public TestManager(DbContext dbContext, ISurveyManager surveyManager, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _surveyManager = surveyManager ?? throw new ArgumentNullException(nameof(surveyManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected static Test GenerateTest(int surveyId, string userId)
        {
            var test = new Test
            {
                SurveyId = surveyId,
                UserId = userId,
                Started = DateTime.UtcNow
            };

            return test;
        }

        public Test Create(int surveyId, string userId)
        {
            var survey = Surveys.Find(surveyId);
            if (survey == null)
                throw new SurveyNotFoundException(surveyId);

            var test = GenerateTest(surveyId, userId);
            Tests.Add(test);
            _dbContext.SaveChanges();

            return test;
        }

        public async Task<Test> CreateAsync(int surveyId, string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var survey = Surveys.FindAsync(cancellationToken, surveyId);
            if (survey == null)
                throw new SurveyNotFoundException(surveyId);

            var test = GenerateTest(surveyId, userId);
            Tests.Add(test);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return test;
        }

        public Test Get(string userId, int surveyId)
        {
            var test = Tests.SingleOrDefault(t => t.SurveyId == surveyId && t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));
            return test;
        }

        public async Task<Test> GetAsync(string userId, int surveyId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var test = await Tests.SingleOrDefaultAsync(t => t.SurveyId == surveyId && t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase), cancellationToken);
            return test;
        }

        public TestResponse Respond(string userId, int surveyId, string response, int pageId)
        {
            var currentTest = Tests.SingleOrDefault(t => t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && surveyId == t.SurveyId) ?? throw new TestNotFoundException(userId, surveyId);
            var currentPage = Pages.Find(pageId) ?? throw new PageNotFoundException(pageId);
            var testResponse = TestResponses.SingleOrDefault(tr => tr.TestId == currentTest.Id && tr.PageId == pageId);

            var responseExpected = currentPage.GetType().IsAssignableFrom(typeof(IQuestion));

            if (testResponse == null)
            {
                testResponse = new TestResponse
                {
                    TestId = currentTest.Id,
                    PageId = pageId,
                    Response = response,
                    Created = DateTime.UtcNow,
                    Responded = responseExpected && string.IsNullOrWhiteSpace(response) ? (DateTime?)null : DateTime.UtcNow
                };

                TestResponses.Add(testResponse);
            }
            else
            {
                testResponse.Response = response;
                testResponse.Modified = DateTime.UtcNow;
                testResponse.Responded = DateTime.UtcNow;
            }

            _dbContext.SaveChangesAsync();

            return testResponse;
        }

        public async Task<TestResponse> RespondAsync(string userId, int surveyId, string response, int pageId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var currentTest = await Tests.SingleOrDefaultAsync(t => t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && surveyId == t.SurveyId, cancellationToken) ?? throw new TestNotFoundException(userId, surveyId);
            var currentPage = await Pages.FindAsync(cancellationToken, pageId) ?? throw new PageNotFoundException(pageId);
            var testResponse = await TestResponses.SingleOrDefaultAsync(tr => tr.TestId == currentTest.Id && tr.PageId == pageId, cancellationToken);

            var responseExpected = currentPage.GetType().IsAssignableFrom(typeof(IQuestion));

            if (testResponse == null)
            {
                testResponse = new TestResponse
                {
                    TestId = currentTest.Id,
                    PageId = pageId,
                    Response = response,
                    Created = DateTime.UtcNow,
                    Responded = responseExpected && string.IsNullOrWhiteSpace(response) ? (DateTime?) null : DateTime.UtcNow
                };

                TestResponses.Add(testResponse);
            }
            else
            {
                testResponse.Response = response;
                testResponse.Modified = DateTime.UtcNow;
                testResponse.Responded = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return testResponse;
        }

        public UserSurvey CompleteCurrentSection(string userId, int surveyId)
        {
            var section = _surveyManager.GetCurrentUserSection(userId, surveyId);
            var sectionMarker = TestSectionMarkers.SingleOrDefault(tsm => tsm.TestId == section.TestId && tsm.SectionId == section.Id) ?? throw new SectionMarkerNotFoundException(section.TestId.GetValueOrDefault(), section.Id.GetValueOrDefault());

            sectionMarker.Completed = DateTime.UtcNow;

            // Check if the survey is complete
            var sectionMarkers =
                from s in Sections
                join tsm in TestSectionMarkers on s.Id equals tsm.SectionId into sm
                from m in sm.DefaultIfEmpty()
                select new { SectionId = s.Id, m.Completed };

            if (sectionMarkers.All(sm => sm.Completed.HasValue))
            {
                var test = Tests.SingleOrDefault(t => t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && t.SurveyId == surveyId) ?? throw new TestNotFoundException(userId, surveyId);
                test.Completed = DateTime.UtcNow;
            }

            _dbContext.SaveChanges();
            var result = _surveyManager.GetUserSurvey(userId, surveyId);
            return result;
        }

        public async Task<UserSurvey> CompleteCurrentSectionAsync(string userId, int surveyId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var section = await _surveyManager.GetCurrentUserSectionAsync(userId, surveyId, cancellationToken);
            var sectionMarker = await TestSectionMarkers
                                    .SingleOrDefaultAsync(tsm => tsm.TestId == section.TestId && tsm.SectionId == section.Id, cancellationToken) 
                                ?? throw new SectionMarkerNotFoundException(section.TestId.GetValueOrDefault(), section.Id.GetValueOrDefault());

            sectionMarker.Completed = DateTime.UtcNow;

            // Check if the survey is complete
            var sections = await _surveyManager.GetUserSectionsAsync(userId, surveyId, cancellationToken);

            if (sections.Where(s => s.Id != section.Id).All(s => s.Completed.HasValue))
            {
                // Close the test if all the section markers are complete.
                var test = Tests.SingleOrDefault(t => t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && t.SurveyId == surveyId) ?? throw new TestNotFoundException(userId, surveyId);
                test.Completed = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            var result = await _surveyManager.GetUserSurveyAsync(userId, surveyId, cancellationToken);
            return result;
        }
    }
}