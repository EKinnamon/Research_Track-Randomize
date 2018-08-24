namespace EKSurvey.Core.Services
{
    public interface IDtoBuilder
    {
        TDto Build<T, TDto>(T entity);
    }
}