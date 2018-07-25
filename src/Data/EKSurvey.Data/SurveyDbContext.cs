using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
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

        public DbSet<Page> Pages { get; set; }

        public DbSet<Test> Tests { get; set; }

        public DbSet<TestResponse> TestResponses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new SurveyMap());
            modelBuilder.Configurations.Add(new SectionMap());
            modelBuilder.Configurations.Add(new PageMap());
            modelBuilder.Configurations.Add(new TestMap());
            modelBuilder.Configurations.Add(new TestResponseMap());
        }
    }

    public class TestResponseMap : EntityTypeConfiguration<TestResponse>
    {
    }

    public class TestMap : EntityTypeConfiguration<Test>
    {
    }

    public class PageMap : EntityTypeConfiguration<Page>
    {
    }

    public class SectionMap : EntityTypeConfiguration<Section>
    {
        public SectionMap()
        {
            this.Property(t => t.Name)
                .HasMaxLength(256);
        }
    }

    public class SurveyMap : EntityTypeConfiguration<Survey>
    {
        public SurveyMap()
        {
            this.Property(t => t.Name)
                .HasMaxLength(256);
        }
    }
}
