using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class Review
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid PlaceId { get; set; }

        public Guid AssessmentId { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }


        public virtual Assessment Assessment { get; set; }
        public virtual Place Place { get; set; }
        public virtual ICollection<ReviewImage> Images { get; set; }
    }
}
