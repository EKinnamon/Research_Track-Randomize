using System.Data.Entity.ModelConfiguration;
using EKSurvey.Core.Models.Entities;

namespace EKSurvey.Data
{
    public class TestResponseMap : EntityTypeConfiguration<TestResponse> { }

    public class TestMap : EntityTypeConfiguration<Test> { }

    public class TestSectionMarkerMap : EntityTypeConfiguration<TestSectionMarker> { }

    public class PageMap : EntityTypeConfiguration<Page>
    {
        public PageMap()
        {
            this.HasMany(p => p.TestResponses)
                .WithRequired(tr => tr.Page)
                .WillCascadeOnDelete(false);
        }
    }

    public class SectionMap : EntityTypeConfiguration<Section>
    {
        public SectionMap()
        {
            this.HasMany(s => s.TestSectionMarkers)
                .WithRequired(tsm => tsm.Section)
                .WillCascadeOnDelete(false);
        }
    }

    public class SurveyMap : EntityTypeConfiguration<Survey>
    {
        public SurveyMap()
        {
            this.HasMany(s => s.Tests)
                .WithRequired(t => t.Survey)
                .WillCascadeOnDelete(false);
        }
    }
}
