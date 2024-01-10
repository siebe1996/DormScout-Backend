using Models.ReviewImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Reviews
{
    public class PostReviewModel : BaseReviewModel
    {
        public List<PostReviewImageModel> Images { get; set; }
    }
}
