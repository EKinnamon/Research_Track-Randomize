using EKSurvey.Core.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EKSurvey.Data
{
    public class MembershipDbContext : IdentityDbContext<ApplicationUser>
    {
        public MembershipDbContext()
            : base("MembershipConnection", throwIfV1Schema: false)
        {
        }

        public static MembershipDbContext Create()
        {
            return new MembershipDbContext();
        }
    }
}