using Models.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.interfaces
{
    public interface IReviewRepository
    {
        Task<List<GetReviewModel>> GetReviews();
        Task<GetReviewModel> GetReview(Guid id);
        Task<GetReviewModel> PostReview(PostReviewModel postReviewModel);
    }
}
