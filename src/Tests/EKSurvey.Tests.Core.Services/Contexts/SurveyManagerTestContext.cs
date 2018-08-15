using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoFixture;

using AutoMapper;

using EKSurvey.Core.Models.Entities;
using EKSurvey.Core.Models.Profiles;
using EKSurvey.Core.Services;
using EKSurvey.Tests.Extensions;
using EKSurvey.Tests.JsonConverters;
using FakeItEasy;

namespace EKSurvey.Tests.Core.Services.Contexts
{
    public class SurveyManagerTestContext : ServiceBaseTestContext<SurveyManager>
    {
        [Flags]
        public enum JoinNeeds
        {
            SurveyTests,
            TestResponses
        }
        public SurveyManagerTestContext()
        {
            Mapper = new MapperConfiguration(GenerateTestConfiguration).CreateMapper();

            Fixture.Register(() => Mapper);
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            JoinSurveyTests();
            JoinTestResponses();
        }

        private void JoinSurveyTests()
        {
            foreach (var survey in Surveys)
            {
                var tests = Fixture.Create<List<Test>>();
                tests.ForEach(t =>
                {
                    t.SurveyId = survey.Id;
                    t.Survey = survey;
                });

                survey.Tests = tests;
            }
        }

        private void JoinTestResponses()
        {
            var responses = new List<TestResponse>();
            var sectionMarkers = new List<TestSectionMarker>();
            foreach (var test in Tests)
            {
                if (test.Completed.HasValue)
                {

                }

                if (test.Completed.HasValue)
                {
                    var pages = test.Survey.Sections.SelectMany(s => s.Pages);
                    foreach (var page in pages)
                    {
                        var response = Fixture.Create<TestResponse>();
                        response.TestId = test.Id;
                        response.PageId = page.Id;
                        response.Page = page;
                        responses.Add(response);
                    }

                    foreach (var section in test.Survey.Sections)
                    {
                        var sectionMarker = Fixture.Create<TestSectionMarker>();
                        sectionMarker.TestId = test.Id;
                        sectionMarker.SectionId = section.Id;
                        sectionMarker.Section = section;
                        sectionMarkers.Add(sectionMarker);
                    }
                }
                else
                {
                    var page = test.Survey.Sections.SelectMany(s => s.Pages).Shuffle().Last();
                    var section = page.SectionId;
                    var finishedSections = test.Survey.Sections.OrderBy(s => s.Order).TakeWhile(s => s.Id != section);
                }
            }

            TestResponses = responses;
            TestSectionMarkers = sectionMarkers;
        }


        public Fake<DbContext> DbContext { get; set; }
        public Fake<Random> Rng { get; set; }
        public Fake<DbSet<Survey>> SurveySet { get; set; }
        public Fake<DbSet<Section>> SectionSet { get; set; }
        public Fake<DbSet<TestResponse>> TestResponseSet { get; set; }
        public Fake<DbSet<TestSectionMarker>> TestSectionMarkerSet { get; set; }

        private static void GenerateTestConfiguration(IMapperConfigurationExpression config)
        {
            config.AddProfile<DtoModelProfile>();
        }

        public void PrepareServiceConfiguration(bool needsHeirarchy = false, bool includeCompleted = false)
        {
            DbContext = Fixture.Freeze<Fake<DbContext>>();
            Rng = Fixture.Freeze<Fake<Random>>();
            SurveySet = Fixture.Freeze<Fake<DbSet<Survey>>>();
            SectionSet = Fixture.Freeze<Fake<DbSet<Section>>>();
            TestResponseSet = Fixture.Freeze<Fake<DbSet<TestResponse>>>();

            Fixture.Inject(DbContext.FakedObject);
            Fixture.Inject(Rng.FakedObject);

            if (!needsHeirarchy)
                return;

            if (!includeCompleted)
            {
                var completedSurveys = Surveys.Where(s => s.Tests.Any(t => t.Completed.HasValue)).ToList();
                var completedTests = completedSurveys.SelectMany(s => s.Tests).Where(t => t.Completed.HasValue).ToList();
                var userIds = completedTests.Select(t => t.UserId).ToList();
                UserId = userIds.Shuffle().First();
                var surveyIds = completedSurveys.Select(s => s.Id).ToList();
                SurveyId = surveyIds[Fixture.Create<int>() % surveyIds.Count];
            }
            else
            {
                UserId = UserIds[Fixture.Create<int>() % UserIds.Count];
            }
        }

        public IList<Survey> Surveys { get; set; } = new FixtureData<Survey>("TestData/Surveys.json", new PageConverter()).ToList();
        public IList<Test> Tests => Surveys.SelectMany(s => s.Tests).ToList();
        public IList<string> UserIds => Tests.Select(t => t.UserId).ToList();
        public IList<Section> Sections => Surveys.SelectMany(s => s.Sections).ToList();
        public IList<Page> Pages => Sections.SelectMany(s => s.Pages).ToList();
        public IList<TestResponse> TestResponses { get; set; }
        public IList<TestSectionMarker> TestSectionMarkers { get; set; }

        public string UserId { get; set; }
        public int SurveyId { get; set; }

        public void PrepareServiceHelperCalls()
        {
            var surveysQueryable = Surveys.AsQueryable();
            SetupDbSetCalls(SurveySet.FakedObject, surveysQueryable);
            A.CallTo(() => SurveySet.FakedObject.Include(A<string>._)).Returns(SurveySet.FakedObject);
            A.CallTo(() => DbContext.FakedObject.Set<Survey>()).Returns(SurveySet.FakedObject);

            var sectionsQueryable = Sections.AsQueryable();
            SetupDbSetCalls(SectionSet.FakedObject, sectionsQueryable);
            A.CallTo(() => DbContext.FakedObject.Set<Section>()).Returns(SectionSet.FakedObject);

            var testResponseQueryable = TestResponses.AsQueryable();
            SetupDbSetCalls(TestResponseSet.FakedObject, testResponseQueryable);
            A.CallTo(() => DbContext.FakedObject.Set<TestResponse>()).Returns(TestResponseSet.FakedObject);
        }

        private static void SetupDbSetCalls<T>(IQueryable<T> fake, IQueryable<T> fakeData) where T : class
        {
            A.CallTo(() => fake.GetEnumerator()).Returns(fakeData.GetEnumerator());
            A.CallTo(() => fake.Provider).Returns(fakeData.Provider);
            A.CallTo(() => fake.Expression).Returns(fakeData.Expression);
            A.CallTo(() => fake.ElementType).Returns(fakeData.ElementType);
        }

        public void PrepareDataHeirarchy(JoinNeeds joinNeeds)
        {
            if (joinNeeds.HasFlag(JoinNeeds.SurveyTests))
            {
                JoinSurveyTests();
            }

            if (joinNeeds.HasFlag(JoinNeeds.TestResponses))
            {
                JoinTestResponses();
            }
        }
    }
}
