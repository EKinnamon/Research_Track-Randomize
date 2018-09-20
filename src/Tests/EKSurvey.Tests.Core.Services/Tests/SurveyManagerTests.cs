using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EKSurvey.Core.Models.DataTransfer;
using EKSurvey.Core.Models.Entities;
using EKSurvey.Core.Services;
using EKSurvey.Tests.Core.Services.Contexts;

using FluentAssertions;

using Xunit;

namespace EKSurvey.Tests.Core.Services.Tests
{
    public class SurveyManagerTests
    {
        private readonly SurveyManagerTestContext _context = new SurveyManagerTestContext();

        public SurveyManagerTests()
        {
            _context.Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Fact]
        public void Constructor_with_valid_arguments_instantiates_service()
        {
            _context.PrepareServiceConfiguration();
            var service = new SurveyManager(_context.DbContext.FakedObject, _context.Rng.FakedObject, _context.Mapper);

            service.Should().NotBeNull();
            service.Should().BeOfType<SurveyManager>();
        }

        [Fact]
        public void Constructor_with_null_dbcontext_throws_exception()
        {
            _context.PrepareServiceConfiguration();

            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SurveyManager(null,
                    _context.Rng.FakedObject,
                    _context.Mapper));

            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_with_null_random_throws_exception()
        {
            _context.PrepareServiceConfiguration();
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SurveyManager(_context.DbContext.FakedObject,
                    null,
                    _context.Mapper));

            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_with_null_mapper_throws_exception()
        {
            _context.PrepareServiceConfiguration();
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SurveyManager(_context.DbContext.FakedObject,
                    _context.Rng.FakedObject,
                    null));

            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GetActiveSurveys_will_return_all_active_surveys()
        {
            _context.PrepareServiceConfiguration();
            _context.PrepareTestEntities();
            _context.PrepareServiceHelperCalls();
            _context.PrepareService();

            var surveys = _context.Service.GetActiveSurveys();

            surveys.Should().NotBeNull();
            surveys.Should().NotContainNulls();
            surveys.Should().BeEquivalentTo(_context.Surveys.Where(s => s.IsActive && !s.Deleted.HasValue));
        }

        [Fact]
        public async Task GetActiveSurveysAsync_will_return_all_active_surveys()
        {
            _context.PrepareServiceConfiguration();
            _context.PrepareTestEntities();
            _context.PrepareServiceHelperCalls();
            _context.PrepareService();

            var surveys = await _context.Service.GetActiveSurveysAsync();

            surveys.Should().NotBeNull();
            surveys.Should().NotContainNulls();
            surveys.Should().BeAssignableTo<IQueryable<Survey>>();
            surveys.Should().BeEquivalentTo(_context.Surveys.Where(s => s.IsActive && !s.Deleted.HasValue));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GetUserSurveys_will_return_all_surveys_for_user(bool includeCompleted)
        {
            _context.PrepareServiceConfiguration();
            _context.PrepareTestEntities(includeCompleted);
            _context.PrepareServiceHelperCalls();
            _context.PrepareService();

            var userSurveys = _context.Service.GetUserSurveys(_context.UserId, includeCompleted);

            userSurveys.Should().NotBeNull();
            userSurveys.Should().NotContainNulls();
            userSurveys.Should().BeAssignableTo<ICollection<UserSurvey>>();
            userSurveys.Should().BeEquivalentTo(_context.UserSurveys.Where(us => !us.Completed.HasValue || includeCompleted));

            if (!includeCompleted)
            {
                userSurveys.All(us => !us.Completed.HasValue).Should().BeTrue();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetUserSurveysAsync_will_return_all_surveys_for_user(bool includeCompleted)
        {
            _context.PrepareServiceConfiguration();
            _context.PrepareTestEntities(includeCompleted);
            _context.PrepareServiceHelperCalls();
            _context.PrepareService();

            var userSurveys = await _context.Service.GetUserSurveysAsync(_context.UserId, includeCompleted);

            userSurveys.Should().NotBeNull();
            userSurveys.Should().NotContainNulls();
            userSurveys.Should().BeAssignableTo<ICollection<UserSurvey>>();
            userSurveys.Should().BeEquivalentTo(_context.UserSurveys.Where(us => !us.Completed.HasValue || includeCompleted));

            if (!includeCompleted)
            {
                userSurveys.All(us => !us.Completed.HasValue).Should().BeTrue();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GetUserSections_will_return_all_sections_for_survey_and_user(bool surveyComplete)
        {
            _context.PrepareServiceConfiguration();
            _context.PrepareTestEntities(userSurveyComplete: surveyComplete);
            _context.PrepareServiceHelperCalls();
            _context.PrepareService();

            var userSections = _context.Service.GetUserSections(_context.UserId, _context.Survey.Id);

            userSections.Should().NotBeNull();
            userSections.Should().NotContainNulls();
            userSections.Should().BeAssignableTo<ICollection<IUserSection>>();
            userSections.Should().BeEquivalentTo(_context.UserSections);
        }

        //[Fact]
        //public async Task GetUserSectionsAsync_will_return_all_sections_for_survey_and_user()
        //{
        //    _context.PrepareServiceConfiguration();
        //    _context.PrepareTestEntities();
        //    _context.PrepareServiceHelperCalls();
        //    _context.PrepareService();

        //    var userSections = await _context.Service.GetUserSectionsAsync(_context.UserId, _context.SurveyId);

        //    userSections.Should().NotBeNull();
        //    userSections.Should().NotContainNulls();
        //    userSections.Should().BeAssignableTo<ICollection<IUserSection>>();
        //    userSections.Select(i => i.UserId).Distinct().Single().Should().BeEquivalentTo(_context.UserId);
        //}

        //[Fact]
        //public void GetUserSections_for_invalid_survey_will_throw_exception()
        //{
        //    _context.PrepareServiceConfiguration();
        //    _context.PrepareTestEntities(invalidSurvey: true);
        //    _context.PrepareServiceHelperCalls();
        //    _context.PrepareService();

        //    Action action = () => _context.Service.GetUserSections(_context.UserId, _context.SurveyId);
        //    action.Should()
        //        .Throw<SurveyNotFoundException>()
        //        .WithMessage($"Survey `{_context.SurveyId}` could not be found.");
        //}

        //[Fact]
        //public void GetUserSectionsAsync_for_invalid_survey_will_throw_exception()
        //{
        //    _context.PrepareServiceConfiguration();
        //    _context.PrepareTestEntities(invalidSurvey: true);
        //    _context.PrepareServiceHelperCalls();
        //    _context.PrepareService();

        //    Func<Task> action = async () => { await _context.Service.GetUserSectionsAsync(_context.UserId, _context.SurveyId); };

        //    action.Should()
        //        .Throw<SurveyNotFoundException>()
        //        .WithMessage($"Survey `{_context.SurveyId}` could not be found.");
        //}

        //[Fact]
        //public void GetCurrentUserSection_gets_current_active_section_for_user()
        //{
        //    _context.PrepareServiceConfiguration();
        //    _context.PrepareTestEntities();
        //    _context.PrepareServiceHelperCalls();
        //    _context.PrepareService();

        //    var userSection = _context.Service.GetCurrentUserSection(_context.UserId, _context.SurveyId);

        //    userSection.Should().NotBeNull();
        //    userSection.UserId.Should().BeEquivalentTo(_context.UserId);
        //    userSection.SurveyId.Should().Be(_context.SurveyId);
        //    userSection.Started.Should().NotBeNull();
        //    userSection.Completed.Should().BeNull();
        //}

        //[Fact]
        //public async Task GetCurrentUserSectionAsync_gets_current_active_section_for_user()
        //{
        //    _context.PrepareServiceConfiguration();
        //    _context.PrepareTestEntities();
        //    _context.PrepareServiceHelperCalls();
        //    _context.PrepareService();

        //    var userSection = await _context.Service.GetCurrentUserSectionAsync(_context.UserId, _context.SurveyId);

        //    userSection.Should().NotBeNull();
        //    userSection.UserId.Should().BeEquivalentTo(_context.UserId);
        //    userSection.SurveyId.Should().Be(_context.SurveyId);
        //    userSection.Started.Should().NotBeNull();
        //    userSection.Completed.Should().BeNull();
        //}

        //[Fact]
        //public void GetCurrentUserPage_gets_current_acitve_page_for_user()
        //{
        //    _context.PrepareServiceConfiguration();
        //    _context.PrepareTestEntities();
        //    _context.PrepareServiceHelperCalls();
        //    _context.PrepareService();

        //    var userPage = _context.Service.GetCurrentUserPage(_context.UserId, _context.SurveyId);

        //    userPage.Should().NotBeNull();
        //    userPage.UserId.Should().BeEquivalentTo(_context.UserId);
        //    userPage.SurveyId.Should().Be(_context.SurveyId);
        //    userPage.Page.SectionId.Should().Be(_context.SectionId);
        //    userPage.Page.Id.Should().Be(_context.PageId);
        //}

        //[Fact]
        //public async Task GetCurrentUserPageAsync_gets_current_acitve_page_for_user()
        //{
        //    _context.PrepareServiceConfiguration();
        //    _context.PrepareTestEntities();
        //    _context.PrepareServiceHelperCalls();
        //    _context.PrepareService();

        //    var userPage = await _context.Service.GetCurrentUserPageAsync(_context.UserId, _context.SurveyId);

        //    userPage.Should().NotBeNull();
        //    userPage.UserId.Should().BeEquivalentTo(_context.UserId);
        //    userPage.SurveyId.Should().Be(_context.SurveyId);
        //    userPage.Page.SectionId.Should().Be(_context.SectionId);
        //    userPage.Page.Id.Should().Be(_context.PageId);
        //}

        //[Theory]
        //[InlineData(false)]
        //[InlineData(true)]
        //public void GetSectionResponses_gets_section_responses_for_user(bool userHasResponses)
        //{
        //    _context.PrepareServiceConfiguration();
        //    _context.PrepareTestEntities(userHasResponses: userHasResponses);
        //    _context.PrepareServiceHelperCalls();
        //    _context.PrepareService();

        //    var userResponses = _context.Service.GetSectionResponses(_context.UserId, _context.SectionId);

        //    userResponses.Should().NotBeNull();
        //    userResponses.Should().NotContainNulls();

        //    if (!userHasResponses)
        //        return;

        //    userResponses.Should().NotBeEmpty();
        //    userResponses.Select(ur => ur.UserId).Distinct().Single().Should().Be(_context.UserId);
        //    userResponses.Select(ur => ur.SurveyId).Distinct().Single().Should().Be(_context.SurveyId);
        //    userResponses.Select(ur => ur.SectionId).Distinct().Single().Should().Be(_context.SectionId);
        //}

        //[Theory]
        //[InlineData(false)]
        //[InlineData(true)]
        //public async Task GetSectionResponsesAsync_gets_section_responses_for_user(bool userHasResponses)
        //{
        //    _context.PrepareServiceConfiguration();
        //    _context.PrepareTestEntities(userHasResponses: userHasResponses);
        //    _context.PrepareServiceHelperCalls();
        //    _context.PrepareService();

        //    var userResponses = await _context.Service.GetSectionResponsesAsync(_context.UserId, _context.SectionId);

        //    userResponses.Should().NotBeNull();
        //    userResponses.Should().NotContainNulls();

        //    if (!userHasResponses)
        //        return;

        //    userResponses.Should().NotBeEmpty();
        //    userResponses.Select(ur => ur.UserId).Distinct().Single().Should().Be(_context.UserId);
        //    userResponses.Select(ur => ur.SurveyId).Distinct().Single().Should().Be(_context.SurveyId);
        //    userResponses.Select(ur => ur.SectionId).Distinct().Single().Should().Be(_context.SectionId);
        //}

        //[Fact]
        //public void GetUserSurvey_gets_survey_for_user()
        //{
        //    _context.PrepareServiceConfiguration();
        //    _context.PrepareTestEntities();
        //    _context.PrepareServiceHelperCalls();
        //    _context.PrepareService();

        //    var userSurvey = _context.Service.GetUserSurvey(_context.UserId, _context.SurveyId);

        //    userSurvey.Should().NotBeNull();
        //    userSurvey.UserId.Should().BeEquivalentTo(_context.UserId);
        //    userSurvey.Id.Should().Be(_context.SurveyId);
        //}

        //[Fact]
        //public async Task GetUserSurveyAsync_gets_survey_for_user()
        //{
        //    _context.PrepareServiceConfiguration();
        //    _context.PrepareTestEntities();
        //    _context.PrepareServiceHelperCalls();
        //    _context.PrepareService();

        //    var userSurvey = await _context.Service.GetUserSurveyAsync(_context.UserId, _context.SurveyId);

        //    userSurvey.Should().NotBeNull();
        //    userSurvey.UserId.Should().BeEquivalentTo(_context.UserId);
        //    userSurvey.Id.Should().Be(_context.SurveyId);
        //}

    }
}
