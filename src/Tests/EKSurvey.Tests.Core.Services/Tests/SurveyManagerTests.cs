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
            var service = new SurveyManager(_context.DbContext.FakedObject, _context.RNG.FakedObject, _context.Mapper);

            service.Should().NotBeNull();
            service.Should().BeOfType<SurveyManager>();
        }
    }
}
