using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Reviews
{
    public class BaseReviewModel
    {
        [Required]
        public Guid PlaceId { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
