using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using AutoFixture;

using AutoMapper;

using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Entities;
using EKSurvey.Core.Models.Enums;
using EKSurvey.Core.Models.Profiles;
using EKSurvey.Core.Services;

using FakeItEasy;

using MoreLinq;

namespace EKSurvey.Tests.Core.Services.Contexts
{
    public class SurveyManagerTestContext : ServiceBaseTestContext<SurveyManager>
    {
        public SurveyManagerTestContext()
        {
            Mapper = new MapperConfiguration(GenerateTestConfiguration).CreateMapper();

            Fixture.Register(() => Mapper);
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        public Fake<DbContext> DbContext { get; set; }
        public Fake<Random> Rng { get; set; }
        public Fake<DbSet<Survey>> SurveySet { get; set; }
        public Fake<DbSet<Section>> SectionSet { get; set; }
        public Fake<DbSet<Page>> PageSet { get; set; }
        public Fake<DbSet<Test>> TestSet { get; set; }
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
            PageSet = Fixture.Freeze<Fake<DbSet<Page>>>();
            TestSet = Fixture.Freeze<Fake<DbSet<Test>>>();
            TestResponseSet = Fixture.Freeze<Fake<DbSet<TestResponse>>>();
            TestSectionMarkerSet = Fixture.Freeze<Fake<DbSet<TestSectionMarker>>>();

            Fixture.Inject(DbContext.FakedObject);
            Fixture.Inject(Rng.FakedObject);
        }

        public void PrepareTestEntities(bool includeCompleted = false)
        {
            UserId = Guid.NewGuid().ToString();
            var surveyId = 0;
            var sectionId = 0;

            IEnumerable<UserSurvey> UserSurveyBuilder()
            {
                while (true)
                {
                    var surveyComplete = surveyId % 2 == 0;
                    var userSurvey = Fixture
                        .Build<UserSurvey>()
                        .With(us => us.UserId, UserId)
                        .With(us => us.Id, ++surveyId)
                        .With(us => us.Completed, surveyComplete ? Fixture.Create<DateTime?>() : null)
                        .Create();

                    yield return userSurvey;
                }
            }

            IEnumerable<IUserSection> UserSectionBuilder(int sectionSurveyId, bool isTransition)
            {
                var userSurvey = UserSurveys.Single(us => us.Id == sectionSurveyId);
                var test = Surveys.Single(s => s.Id == userSurvey.Id).Tests.Single();

                var sectionCount = Fixture.Create<int>() % 8 + 3;
                var sectionCompleteCount = userSurvey.Completed.HasValue 
                    ? sectionCount 
                    : Fixture.Create<int>() % sectionCount;

                var userSections = Enumerable
                    .Range(0, sectionCount)
                    .Select(i =>
                    {
                        IUserSection section;
                        var isGroup = i % 5 == 2;

                        var started = i < sectionCompleteCount || (i == sectionCompleteCount - 1 && !isTransition) ? Fixture.Create<DateTime?>() : null;
                        var modified = i < sectionCompleteCount && Fixture.Create<bool>()
                            ? Fixture.Create<DateTime?>()
                            : null;
                        var completed = i < sectionCompleteCount ? Fixture.Create<DateTime?>() : null;

                        if (isGroup)
                        {
                            var subSectionCount = Fixture.Create<int>() % 5 + 2;
                            var subSections = Enumerable.Range(0, subSectionCount).Select(j =>
                            {
                                return Fixture
                                    .Build<UserSection>()
                                    .With(us => us.UserId, UserId)
                                    .With(us => us.SurveyId, sectionSurveyId)
                                    .With(us => us.TestId, test.Id)
                                    .With(us => us.Order, i)
                                    .With(us => us.Id, ++sectionId)
                                    .With(us => us.Started, started)
                                    .With(us => us.Modified, modified)
                                    .Without(us => us.Completed)
                                    .Create();
                            }).ToList();
                                
                            var finishSubsectionId = subSections.Shuffle().First().Id;

                            subSections = subSections.Select(s =>
                            {
                                s.Completed = finishSubsectionId == s.Id && completed.HasValue ? completed : null;
                                return s;
                            }).ToList();

                            section = new UserSectionGroup(subSections);
                        }
                        else
                        {
                            section = Fixture
                                .Build<UserSection>()
                                .With(us => us.UserId, UserId)
                                .With(us => us.SurveyId, sectionSurveyId)
                                .With(us => us.TestId, test.Id)
                                .With(us => us.Order, i)
                                .With(us => us.Id, ++sectionId)
                                .With(us => us.Started, started)
                                .With(us => us.Modified, modified)
                                .With(us => us.Completed, completed)
                                .Create();
                        }

                        return section;
                    }).ToList();

                return userSections;
            }

            UserSurveys = UserSurveyBuilder().Take(Fixture.Create<int>() % 8 + 3).ToList();
            var testSurveyId = UserSurveys.Shuffle().First().Id;

            Surveys = UserSurveys.Select(us =>
            {
                var tests = !us.Started.HasValue
                    ? new HashSet<Test>()
                    : new[]
                    {
                        Fixture
                            .Build<Test>()
                            .With(t => t.UserId, us.UserId)
                            .With(t => t.SurveyId, us.Id)
                            .With(t => t.Started, us.Started.GetValueOrDefault())
                            .With(t => t.Modified, us.Modified)
                            .With(t => t.Completed, us.Completed)
                            .Without(t => t.Survey)
                            .Without(t => t.TestResponses)
                            .Without(t => t.TestSectionMarkers)
                            .Create()
                    }.ToHashSet();

                var survey = Fixture
                    .Build<Survey>()
                    .With(s => s.Id, us.Id)
                    .With(s => s.Name, us.Name)
                    .With(s => s.Description, us.Description)
                    .With(s => s.Version, us.Version)
                    .With(s => s.IsActive, true)
                    .With(s => s.Created, us.Created)
                    .With(s => s.Modified, us.Modified)
                    .With(s => s.Tests, tests)
                    .Without(s => s.Deleted)
                    .Without(s => s.Sections)
                    .Create();

                if (!tests.Any())
                    return survey;

                survey.Tests = survey.Tests.Select(t =>
                {
                    t.Survey = survey;
                    return t;
                }).ToHashSet();


                return survey;
            }).ToList();

            UserSections = UserSectionBuilder(testSurveyId, false).ToList();

            Surveys = Surveys.Select(s =>
            {
                s.Sections = UserSections
                    .Select(uss =>
                        Fixture
                            .Build<Section>()
                            .With(sec => sec.Id, uss.Id)
                            .With(sec => sec.SurveyId, uss.SurveyId)
                            .With(sec => sec.Order, uss.Order)
                            .With(sec => sec.SelectorType, SelectorType.Random)
                            .With(sec => sec.Survey, s)
                            .Without(sec => sec.Pages)
                            .Without(sec => sec.TestSectionMarkers)
                            .Create())
                    .ToHashSet();

                s.Sections = s.Sections.Select(ss =>
                {
                    var testSectionMarker = s.Tests
                        .First()
                        .TestSectionMarkers
                        .Single(tsm => tsm.SectionId == s.Id);

                    ss.TestSectionMarkers.Add(testSectionMarker);

                    return ss;
                }).ToHashSet();

                if (testSurveyId == s.Id)
                {
                    s.Tests = s.Tests.Select(t =>
                    {
                        t.TestSectionMarkers = UserSections.Select(uss =>
                        {
                            return Fixture
                                .Build<TestSectionMarker>()
                                .With(tsm => tsm.TestId, t.Id)
                                .With(tsm => tsm.SectionId, uss.Id)
                                .With(tsm => tsm.Started, uss.Started)
                                .With(tsm => tsm.Completed, uss.Completed)
                                .With(tsm => tsm.Test, t)
                                .Without(tsm => tsm.Section)
                                .Create();
                        }).ToHashSet();
                        return t;
                    }).ToHashSet();
                }

                return s;
            }).ToList();

            Survey = Surveys.Single(s => s.Id == testSurveyId);
        }

        public IList<UserSurvey> UserSurveys { get; set; }
        public IList<IUserSection> UserSections { get; set; }

        public string UserId { get; set; }
        public Survey Survey { get; set; }
        public Section Section { get; set; }
        public Page Page { get; set; }
        public Test Test { get; set; }

        public IList<Survey> Surveys { get; set; }
        //public IList<Section> Sections => Surveys.SelectMany(s => s.Sections).ToList();
        //public IList<Page> Pages => Sections.SelectMany(s => s.Pages).ToList();
        public IList<Test> Tests => Surveys.SelectMany(s => s.Tests).ToList();
        //public IList<TestSectionMarker> TestSectionMarkers => Tests.SelectMany(t => t.TestSectionMarkers).ToList();
        //public IList<TestResponse> TestResponses => Tests.SelectMany(t => t.TestResponses).ToList();
        //public IList<string> UserIds => Tests.Select(t => t.UserId).ToList();

        public void PrepareServiceHelperCalls()
        {
            SurveySet.FakedObject.SetupData(Surveys, i => Surveys.SingleOrDefault(s => i.OfType<int>().Any(si => s.Id == si)));
            //SectionSet.FakedObject.SetupData(Sections, i => Sections.SingleOrDefault(s => i.OfType<int>().Any(si => s.Id == si)));
            //PageSet.FakedObject.SetupData(Pages, i => Pages.SingleOrDefault(p => i.OfType<int>().Any(pi => p.Id == pi)));
            TestSet.FakedObject.SetupData(Tests, i => Tests.SingleOrDefault(t => i.OfType<Guid>().Any(ti => t.Id == ti)));
            //TestSectionMarkerSet.FakedObject.SetupData(TestSectionMarkers, i => TestSectionMarkers.SingleOrDefault(t => i.OfType<Guid>().Any(tsi => t.Id == tsi)));
            //TestResponseSet.FakedObject.SetupData(TestResponses, i => TestResponses.SingleOrDefault(t => i.OfType<Guid>().Any(tri => t.Id == tri)));

            A.CallTo(() => DbContext.FakedObject.Set<Survey>()).Returns(SurveySet.FakedObject);
            A.CallTo(() => DbContext.FakedObject.Set<Section>()).Returns(SectionSet.FakedObject);
            A.CallTo(() => DbContext.FakedObject.Set<Page>()).Returns(PageSet.FakedObject);
            A.CallTo(() => DbContext.FakedObject.Set<Test>()).Returns(TestSet.FakedObject);
            A.CallTo(() => DbContext.FakedObject.Set<TestSectionMarker>()).Returns(TestSectionMarkerSet.FakedObject);
            A.CallTo(() => DbContext.FakedObject.Set<TestResponse>()).Returns(TestResponseSet.FakedObject);
        }
    }
}
