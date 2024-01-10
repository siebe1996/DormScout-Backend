using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Assessments
{
    public class BaseAssessmentModel
    {
        [Required]
        public Guid ReviewId { get; set; }

        [Required]
        public double Score { get; set; }
    }
}
