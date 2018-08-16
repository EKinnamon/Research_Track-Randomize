using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using AutoFixture;

using AutoMapper;

using EKSurvey.Core.Models.Entities;
using EKSurvey.Core.Models.Profiles;
using EKSurvey.Core.Services;
using EKSurvey.Tests.Extensions;
using EKSurvey.Tests.JsonConverters;
using FakeItEasy;
using Newtonsoft.Json;

namespace EKSurvey.Tests.Core.Services.Contexts
{
    public class SurveyManagerTestContext : ServiceBaseTestContext<SurveyManager>
    {
        private const string TestsFakeDataPath = "TestData/Tests.json";
        private const string TestResponsesFakeDataPath = "TestData/TestResponses.json";
        private const string TestSectionMarkersFakeDataPath = "TestData/TestSectionMarkers.json";

        public SurveyManagerTestContext()
        {
            Mapper = new MapperConfiguration(GenerateTestConfiguration).CreateMapper();

            Fixture.Register(() => Mapper);
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        }

        private void JoinSurveyTests()
        {
            var tests = new List<Test>();
            foreach (var survey in Surveys)
            {
                var surveyTests = Fixture.Create<List<Test>>();
                surveyTests.ForEach(t =>
                {
                    t.SurveyId = survey.Id;
                    t.Survey = survey;
                    t.Modified = Fixture.Create<int>() % 100 == 0 ? t.Modified : null;
                    t.Completed = Fixture.Create<int>() % 5 >= 1 ? t.Completed : null;
                });
                tests.AddRange(surveyTests);
            }

            Write(tests, TestsFakeDataPath);
        }

        private void JoinTestResponses()
        {
            var responses = new List<TestResponse>();
            var sectionMarkers = new List<TestSectionMarker>();
            foreach (var test in Tests)
            {
                IList<Section> sections;
                IList<Page> pages;
                if (test.Completed.HasValue)
                {
                    sections = test.Survey.Sections.ToList();
                    pages = test.Survey.Sections.SelectMany(s => s.Pages).ToList();
                }
                else
                {
                    var page = test.Survey.Sections.SelectMany(s => s.Pages).Shuffle().Last();
                    var section = page.Section;
                    sections = test.Survey.Sections.OrderBy(s => s.Order).TakeWhile(s => s.Id != section.Id).ToList();
                    pages = sections.SelectMany(p => p.Pages).ToList();
                    // skip last page
                    pages = pages.Take(pages.Count).ToList();
                }

                var testSectionMarkers = sections.Select(s =>
                {
                    var sectionMarker = Fixture.Create<TestSectionMarker>();
                    sectionMarker.TestId = test.Id;
                    sectionMarker.SectionId = s.Id;
                    sectionMarker.Section = s;
                    sectionMarker.Completed = Fixture.Create<DateTime>();

                    return sectionMarker;
                }).ToList();

                testSectionMarkers[sections.Count - 1].Completed = test.Completed;

                var testResponses = pages.Select(p =>
                {
                    var testResponse = Fixture.Create<TestResponse>();
                    testResponse.TestId = test.Id;
                    testResponse.PageId = p.Id;
                    testResponse.Page = p;
                    testResponse.Responded = Fixture.Create<DateTime>();

                    return testResponse;
                }).ToList();

                testResponses[pages.Count - 1].Responded = test.Completed;

                responses.AddRange(testResponses);
                sectionMarkers.AddRange(testSectionMarkers);
            }

            TestResponses = responses;
            TestSectionMarkers = sectionMarkers;

            Write(TestResponses, TestResponsesFakeDataPath);
            Write(TestSectionMarkers, TestSectionMarkersFakeDataPath);
        }

        private void JoinSurveyData()
        {
            var sections = Surveys.SelectMany(s => s.Sections).ToList();
            var index = 0;
            sections.ForEach(s => { s.Id = ++index; });


            foreach (var section in sections)
            {
                section.Survey = Surveys.Single(s => s.Id == section.SurveyId);
            }

            foreach (var page in Surveys.SelectMany(s => s.Sections.SelectMany(ss => ss.Pages)))
            {
                page.Section = sections.Single(s => s.Id == page.SectionId);
            }
        }

        private static void Write<T>(IEnumerable<T> data, string jsonDataPath)
        {
            var settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            using (var file = File.CreateText(jsonDataPath))
            {
                var json = JsonConvert.SerializeObject(data, Formatting.None, settings);
                file.Write(json);
            }
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
        public IList<string> UserIds => Tests.Select(t => t.UserId).ToList();
        public IList<Section> Sections => Surveys.SelectMany(s => s.Sections).ToList();
        public IList<Page> Pages => Sections.SelectMany(s => s.Pages).ToList();

        private IList<Test> _tests;

        public IList<Test> Tests
        {
            get => _tests ??
                   LoadFixtureData<Test>(TestsFakeDataPath, JoinSurveyTests, JoinSurveyData).ToList();
            set => _tests = value;
        }

        private IList<TestResponse> _testResponses;
        public IList<TestResponse> TestResponses
        {
            get => _testResponses ??
                   LoadFixtureData<TestResponse>(TestResponsesFakeDataPath, JoinTestResponses, null).ToList();
            set => _testResponses = value;
        }

        private IList<TestSectionMarker> _testSectionMarkers;
        public IList<TestSectionMarker> TestSectionMarkers
        {
            get => _testSectionMarkers ??
                    LoadFixtureData<TestSectionMarker>(TestSectionMarkersFakeDataPath, JoinTestResponses, null).ToList();
            set => _testSectionMarkers = value;
        }

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
        private static IEnumerable<T> LoadFixtureData<T>(string fixtureDataPath, Action generator, Action joiner)
        {
            if (!File.Exists(fixtureDataPath))
            {
                generator.Invoke();
            }

            var fixtureData = new FixtureData<T>(fixtureDataPath);
            joiner?.Invoke();
            return fixtureData;
        }
    }
}
