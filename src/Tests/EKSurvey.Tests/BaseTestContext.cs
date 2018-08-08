using System;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoMapper;

namespace EKSurvey.Tests
{
    public abstract class BaseTestContext : IDisposable
    {
        protected IFixture Fixture { get; set; }
        public IMapper Mapper { get; set; }

        protected BaseTestContext()
        {
            Fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Fixture = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
