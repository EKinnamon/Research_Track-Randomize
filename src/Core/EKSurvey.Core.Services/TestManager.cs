using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EKSurvey.Core.Models.Entities;

namespace EKSurvey.Core.Services
{
    public class TestManager : ITestManager
    {
        private readonly DbContext _dbContext;
        private readonly IMapper _mapper;

        public TestManager(DbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Test Create(int surveyId, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Test> CreateAsync(int surveyId, string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
