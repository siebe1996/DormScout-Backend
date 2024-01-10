using Models.ReviewImages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Models.Reviews
{
    public class GetReviewModel : BaseReviewModel
    {
        [Required]
        public Guid Id { get; set; }

        public Guid? AssessmentId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public List<GetReviewImageModel> Images { get; set; }
    }
}
