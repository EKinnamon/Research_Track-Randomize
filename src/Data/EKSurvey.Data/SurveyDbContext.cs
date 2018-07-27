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

        public virtual DbSet<Survey> Surveys { get; set; }

        public virtual DbSet<Section> Sections { get; set; }

        public virtual DbSet<Page> Pages { get; set; }

        public virtual DbSet<Test> Tests { get; set; }

        public virtual DbSet<TestResponse> TestResponses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new SurveyMap());
            modelBuilder.Configurations.Add(new SectionMap());
            modelBuilder.Configurations.Add(new PageMap());
            modelBuilder.Configurations.Add(new TestMap());
            modelBuilder.Configurations.Add(new TestResponseMap());
        }
    }

}
