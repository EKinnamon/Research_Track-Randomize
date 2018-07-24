using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EKSurvey.Core.Models.Entities
{
    [Table("TestResponses")]
    public class TestResponse
    {
        [Key, Column(Order=0)]
        public int TestId { get; set; }

        [Key, Column(Order=1)]
        public int SectionId { get; set; }


    }
}