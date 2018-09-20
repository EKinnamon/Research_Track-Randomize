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

        private static UserSection GenerateUserSection(UserSection userSection)
        {
            userSection = Fixture
                .Build<UserSection>()
                .With(us => us.UserId, userSection.UserId)
                .With(us => us.SurveyId, userSection.SurveyId)
                .With(us => us.TestId, userSection.TestId)
                .With(us => us.TestSectionMarkerId, userSection.TestSectionMarkerId)
                .With(us => us.Order, userSection.Order)
                .With(us => us.Started, userSection.Started)
                .With(us => us.Modified, userSection.Modified)
                .With(us => us.Completed, userSection.Completed)
                .With(us => us.Id, userSection.Id)
                .Create();

            return userSection;
        }

        private static UserSectionGroup GenerateSectionGroup(string userId, 
            int surveyId, 
            int sectionId, 
            Guid testId, 
            int order, 
            DateTime? modified)
        {
            const int subSectionCount = 5;
            var subSections = Enumerable.Range(0, subSectionCount).Select(i =>
            {
                return Fixture
                    .Build<UserSection>()
                    .With(us => us.UserId, userId)
                    .With(us => us.SurveyId, surveyId)
                    .With(us => us.TestId, testId)
                    .With(us => us.Order, order)
                    .With(us => us.Id, ++sectionId)
                    .With(us => us.Modified, modified)
                    .Without(us => us.TestSectionMarkerId)
                    .Without(us => us.Started)
                    .Without(us => us.Completed)
                    .Create();
            });
            //var activeSubSection = i < sectionCompleteCount ? null : subSections.Shuffle().First();
            //subSections = subSections.Select(ss =>
            //{
            //    ss.TestSectionMarkerId = started.HasValue ? Fixture.Create<Guid?>() : null;
            //    ss.Started = activeSubSection?.Id == ss.Id ? started : null;
            //    ss.Completed = activeSubSection?.Id == ss.Id ? completed : null;
            //    return ss;
            //}).ToList();

            //userSection = new UserSectionGroup(subSections);

            return new UserSectionGroup(subSections);
        }

        public void PrepareTestEntities(bool includeCompleted = false, bool userSurveyComplete = false)
        {
            UserId = Guid.NewGuid().ToString();
            var surveyId = 0;
            var sectionId = 0;

            IEnumerable<UserSurvey> UserSurveyBuilder()
            {
                while (true)
                {
                    var surveyComplete = ++surveyId % 4 == 0;
                    var userSurvey = Fixture
                        .Build<UserSurvey>()
                        .With(us => us.UserId, UserId)
                        .With(us => us.Id, surveyId)
                        .With(us => us.Completed, surveyComplete ? Fixture.Create<DateTime?>() : null)
                        .Create();

                    yield return userSurvey;
                }
            }

            IEnumerable<IUserSection> UserSectionBuilder(int sectionSurveyId, bool isTransition)
            {
                var userSurvey = UserSurveys.Single(us => us.Id == sectionSurveyId);
                var testId = userSurvey.TestId;

                const int sectionCount = 10;
                var sectionCompleteCount = userSurvey.Completed.HasValue
                    ? sectionCount
                    : Fixture.Create<int>() % sectionCount;

                var userSections = Enumerable
                    .Range(0, sectionCount)
                    .Select(i =>
                    {
                        IUserSection userSection;

                        var isSectionGroup = i % 5 == 2;
                        var started = i < sectionCompleteCount || (i == sectionCompleteCount - 1 && !isTransition)
                            ? Fixture.Create<DateTime?>()
                            : null;
                        var modified = i < sectionCompleteCount || Fixture.Create<bool>()
                            ? Fixture.Create<DateTime?>()
                            : null;
                        var completed = i < sectionCompleteCount 
                            ? Fixture.Create<DateTime?>() 
                            : null;

                        if (isSectionGroup)
                        {
                            userSection = GenerateSectionGroup(UserId, sectionSurveyId, ++sectionId, testId, i, modified);
                        }
                        else
                        {
                            userSection = GenerateUserSection(new UserSection
                            {
                                UserId = UserId,
                                SurveyId = sectionSurveyId,
                                TestId = testId,
                                Order = i,
                                Id = ++sectionId,
                                Started = started,
                                Modified = modified,
                                Completed = completed
                            });
                        }

                        return userSection;
                    }).ToList();

                return userSections;
            }

            UserSurveys = UserSurveyBuilder().Take(20).ToList();
            var testSurveyId = UserSurveys.Shuffle().First(s => userSurveyComplete && s.Completed.HasValue || !userSurveyComplete).Id;
            UserSections = UserSectionBuilder(testSurveyId, false).ToList();

            Surveys = UserSurveys.Select(us =>
            {
                var tests = !us.IsStarted
                    ? new HashSet<Test>()
                    : new[]
                    {
                        Fixture
                            .Build<Test>()
                            .With(t => t.Id, us.TestId)
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

                if (us.Id == testSurveyId)
                {
                    survey.Sections = UserSections.SelectMany(ss =>
                    {
                        IEnumerable<Section> sections = null;
                        switch (ss)
                        {
                            case UserSection userSection:
                                var section = Fixture
                                    .Build<Section>()
                                    .With(s => s.Id, userSection.Id)
                                    .With(s => s.SurveyId, userSection.SurveyId)
                                    .With(s => s.Name, userSection.Name)
                                    .With(s => s.Order, userSection.Order)
                                    .With(s => s.SelectorType, SelectorType.Random)
                                    .With(s => s.Survey, survey)
                                    .Without(s => s.Pages)
                                    .Without(s => s.TestSectionMarkers)
                                    .Create();

                                section.TestSectionMarkers = new[]
                                {
                                    Fixture
                                        .Build<TestSectionMarker>()
                                        .With(tsm => tsm.TestId, ss.TestId)
                                        .With(tsm => tsm.SectionId, ss.Id)
                                        .With(tsm => tsm.Started, ss.Started)
                                        .With(tsm => tsm.Completed, ss.Completed)
                                        .With(tsm => tsm.Section, section)
                                        .With(tsm => tsm.Test, survey.Tests.Single(t => t.Id == ss.TestId))
                                        .Create()
                                };

                                sections = new [] { section };
                                break;

                            case UserSectionGroup userSectionGroup:
                                sections = userSectionGroup.Select(sss =>
                                {
                                    var subSection = Fixture
                                        .Build<Section>()
                                        .With(s => s.Id, sss.Id)
                                        .With(s => s.SurveyId, sss.SurveyId)
                                        .With(s => s.Name, sss.Name)
                                        .With(s => s.Order, sss.Order)
                                        .With(s => s.SelectorType, SelectorType.Random)
                                        .With(s => s.Survey, survey)
                                        .Without(s => s.Pages)
                                        .Without(s => s.TestSectionMarkers)
                                        .Create();

                                    subSection.TestSectionMarkers = new[]
                                    {
                                        Fixture
                                            .Build<TestSectionMarker>()
                                            .With(tsm => tsm.TestId, ss.TestId)
                                            .With(tsm => tsm.SectionId, ss.Id)
                                            .With(tsm => tsm.Started, ss.Started)
                                            .With(tsm => tsm.Completed, ss.Completed)
                                            .With(tsm => tsm.Section, subSection)
                                            .With(tsm => tsm.Test, survey.Tests.Single(t => t.Id == ss.TestId))
                                            .Create()
                                    };

                                    return subSection;
                                });
                                break;
                        }

                        return sections;
                    }).ToHashSet();
                }

                return survey;
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
        public IList<Section> Sections => Surveys.SelectMany(s => s.Sections).ToList();
        //public IList<Page> Pages => Sections.SelectMany(s => s.Pages).ToList();
        public IList<Test> Tests => Surveys.SelectMany(s => s.Tests).ToList();
        //public IList<TestSectionMarker> TestSectionMarkers => Tests.SelectMany(t => t.TestSectionMarkers).ToList();
        //public IList<TestResponse> TestResponses => Tests.SelectMany(t => t.TestResponses).ToList();
        //public IList<string> UserIds => Tests.Select(t => t.UserId).ToList();

        public void PrepareServiceHelperCalls()
        {
            SurveySet.FakedObject.SetupData(Surveys, i => Surveys.SingleOrDefault(s => i.OfType<int>().Any(si => s.Id == si)));
            SectionSet.FakedObject.SetupData(Sections, i => Sections.SingleOrDefault(s => i.OfType<int>().Any(si => s.Id == si)));
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
