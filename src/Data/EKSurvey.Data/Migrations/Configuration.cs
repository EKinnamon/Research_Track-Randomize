using System.Data.Entity.Migrations;

namespace EKSurvey.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<MembershipDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MembershipDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
