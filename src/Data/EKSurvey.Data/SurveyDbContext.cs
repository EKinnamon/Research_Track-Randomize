using System.Data.Entity;
using EKSurvey.Core.Models.Entities;

namespace EKSurvey.Data
{
    public class SurveyDbContext : DbContext
    {
        static SurveyDbContext()
        {
            Database.SetInitializer<SurveyDbContext>(null);
        }

        public SurveyDbContext() : base("name=EKSurveyConnection") { }

        public DbSet<Survey> Surveys { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<Question> Questions { get; set; }

    }
}
