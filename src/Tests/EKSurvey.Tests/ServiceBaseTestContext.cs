using AutoFixture;

namespace EKSurvey.Tests
{
    public abstract class ServiceBaseTestContext<T> : BaseTestContext
    {
        public T Service { get; set; }

        public void PrepareService()
        {
            Service = Fixture.Create<T>();
        }
    }
}
