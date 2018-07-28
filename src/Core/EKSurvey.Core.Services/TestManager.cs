using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EKSurvey.Core.Models.Entities;
using EKSurvey.Core.Services.Exceptions;

namespace EKSurvey.Core.Services
{
    public class TestManager : ITestManager
    {
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;

        public DbSet<Survey> Surveys => _dbContext.Set<Survey>();
        public DbSet<Test> Tests => _dbContext.Set<Test>();

        public TestManager(DbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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

        //public Test Get(int surveyId, string userId)
        //{
        //    var test = Tests.SingleOrDefault(t => t.SurveyId == surveyId && t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));
        //    return test;
        //}

        //public async Task<Test> GetAsync(int surveyId, string userId, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    var test = await Tests.SingleOrDefaultAsync( t => t.SurveyId == surveyId && t.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase), cancellationToken);
        //    return test;
        //}

    }
}
