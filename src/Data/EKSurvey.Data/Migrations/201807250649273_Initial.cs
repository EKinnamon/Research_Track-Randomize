namespace EKSurvey.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SectionId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        Range = c.Int(),
                        Text = c.String(),
                        True = c.String(),
                        False = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sections", t => t.SectionId, cascadeDelete: true)
                .Index(t => t.SectionId);
            
            CreateTable(
                "dbo.Sections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SurveyId = c.Int(nullable: false),
                        Name = c.String(),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Surveys", t => t.SurveyId, cascadeDelete: true)
                .Index(t => t.SurveyId);
            
            CreateTable(
                "dbo.Surveys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Version = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TestResponses",
                c => new
                    {
                        TestId = c.Guid(nullable: false),
                        SectionId = c.Int(nullable: false),
                        PageId = c.Int(nullable: false),
                        Response = c.String(),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.TestId, t.SectionId, t.PageId })
                .ForeignKey("dbo.Pages", t => t.PageId, cascadeDelete: true)
                .ForeignKey("dbo.Sections", t => t.SectionId, cascadeDelete: true)
                .ForeignKey("dbo.Tests", t => t.TestId, cascadeDelete: true)
                .Index(t => t.TestId)
                .Index(t => t.SectionId)
                .Index(t => t.PageId);
            
            CreateTable(
                "dbo.Tests",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.String(),
                        SurveyId = c.Int(nullable: false),
                        Started = c.DateTime(nullable: false),
                        Modified = c.DateTime(),
                        Completed = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Surveys", t => t.SurveyId, cascadeDelete: true)
                .Index(t => t.SurveyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestResponses", "TestId", "dbo.Tests");
            DropForeignKey("dbo.Tests", "SurveyId", "dbo.Surveys");
            DropForeignKey("dbo.TestResponses", "SectionId", "dbo.Sections");
            DropForeignKey("dbo.TestResponses", "PageId", "dbo.Pages");
            DropForeignKey("dbo.Sections", "SurveyId", "dbo.Surveys");
            DropForeignKey("dbo.Pages", "SectionId", "dbo.Sections");
            DropIndex("dbo.Tests", new[] { "SurveyId" });
            DropIndex("dbo.TestResponses", new[] { "PageId" });
            DropIndex("dbo.TestResponses", new[] { "SectionId" });
            DropIndex("dbo.TestResponses", new[] { "TestId" });
            DropIndex("dbo.Sections", new[] { "SurveyId" });
            DropIndex("dbo.Pages", new[] { "SectionId" });
            DropTable("dbo.Tests");
            DropTable("dbo.TestResponses");
            DropTable("dbo.Surveys");
            DropTable("dbo.Sections");
            DropTable("dbo.Pages");
        }
    }
}
