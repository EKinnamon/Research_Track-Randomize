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
using FakeItEasy;

namespace EKSurvey.Tests.Core.Services.Contexts
{
    public class SurveyManagerTestContext : ServiceBaseTestContext<SurveyManager>
    {
        private const string SurveyFakeDataPath = "TestData/Surveys.json";
        private const string TestResponsesFakeDataPath = "TestData/TestResponses.json";
        private const string TestSectionMarkersFakeDataPath = "TestData/TestSectionMarkers.json";

        public SurveyManagerTestContext()
        {
            Mapper = new MapperConfiguration(GenerateTestConfiguration).CreateMapper();

            Fixture.Register(() => Mapper);
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            LoadTestResponseData();
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

        public void PrepareServiceConfiguration()
        {
            DbContext = Fixture.Freeze<Fake<DbContext>>();
            Rng = Fixture.Freeze<Fake<Random>>();
            SurveySet = Fixture.Freeze<Fake<DbSet<Survey>>>();
            SectionSet = Fixture.Freeze<Fake<DbSet<Section>>>();
            TestResponseSet = Fixture.Freeze<Fake<DbSet<TestResponse>>>();

            Fixture.Inject(DbContext.FakedObject);
            Fixture.Inject(Rng.FakedObject);
        }

        public void PrepareTestEntities(bool includeCompleted = false)
        {
            if (includeCompleted)
            {
                UserId = UserIds.Shuffle().First();
                SurveyId = Surveys.Shuffle().First().Id;
            }
            else
            {
                var completedTest = Tests.Where(t => !t.Completed.HasValue).Shuffle().First();
                UserId = completedTest.UserId;
                SurveyId = completedTest.SurveyId;
            }
        }

        public IList<Survey> Surveys { get; set; } = (FixtureData<Survey>.Load(SurveyFakeDataPath) ?? Fixture.CreateMany<Survey>(20).FixtureCallback(SurveyGenerate).CacheAs(SurveyFakeDataPath)).ToList();

        private static void SurveyGenerate(IEnumerable<Survey> surveys)
        {
            foreach (var survey in surveys)
            {
                foreach (var test in survey.Tests)
                {
                    test.Modified = Fixture.Create<int>() % 100 == 0 ? test.Modified : null;
                    test.Completed = Fixture.Create<int>() % 5 >= 1 ? test.Completed : null;
                }
            }
        }


        public IList<string> UserIds => Tests.Select(t => t.UserId).ToList();
        public IList<Section> Sections => Surveys.SelectMany(s => s.Sections).ToList();
        public IList<Page> Pages => Sections.SelectMany(s => s.Pages).ToList();
        public IList<Test> Tests => Surveys.SelectMany(s => s.Tests).ToList();

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

        private void LoadTestResponseData()
        {
            var testResponses = FixtureData<TestResponse>.Load(TestResponsesFakeDataPath);
            var testSectionMarkers = FixtureData<TestSectionMarker>.Load(TestSectionMarkersFakeDataPath);

            if (testResponses == null || testSectionMarkers == null)
            {
                if (testResponses == null)
                {

                }
            }

            TestResponses = testResponses.ToList();
            TestSectionMarkers = testSectionMarkers.ToList();
        }

    }
}
