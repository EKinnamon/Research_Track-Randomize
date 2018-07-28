using System.Data.Entity.ModelConfiguration;
using EKSurvey.Core.Models.Entities;

namespace EKSurvey.Data
{
    public class TestResponseMap : EntityTypeConfiguration<TestResponse>
    {
        public TestResponseMap()
        {
            this.HasRequired(tr => tr.Section)
                .WithMany(s => s.TestResponses)
                .WillCascadeOnDelete(false);

            this.HasRequired(tr => tr.Page)
                .WithMany(p => p.TestResponses)
                .WillCascadeOnDelete(false);
        }
    }

    public class TestMap : EntityTypeConfiguration<Test> { }

    public class TestSectionMarkerMap : EntityTypeConfiguration<TestSectionMarker>
    {
        public TestSectionMarkerMap()
        {
            this.HasRequired(tsm => tsm.Section)
                .WithMany(s => s.TestSectionMarkers)
                .WillCascadeOnDelete(false);

            this.HasRequired(tsm => tsm.Test)
                .WithMany(t => t.TestSectionMarkers)
                .WillCascadeOnDelete(true);
        }
    }


    public class PageMap : EntityTypeConfiguration<Page> { }

    public class SectionMap : EntityTypeConfiguration<Section> { }

    public class SurveyMap : EntityTypeConfiguration<Survey> { }
}
