using DataAccessLayer.Repositories.interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Places;
using Models.ReviewImages;
using Models.Reviews;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly Backend_DormScoutContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        public ReviewRepository(Backend_DormScoutContext backend_DormScoutContext, IHttpContextAccessor httpContextAccessor)
        {
            _context = backend_DormScoutContext;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<List<GetReviewModel>> GetReviews()
        {
            List<GetReviewModel> reviews = await _context.Reviews.AsNoTracking()
                .Select(x => new GetReviewModel
            { 
                Id = x.Id,
                PlaceId = x.PlaceId,
                AssessmentId = x.AssessmentId != Guid.Empty ? x.AssessmentId : null,
                Text = x.Text,
                    Images = x.Images.Select(x => new GetReviewImageModel
                    {
                        Id = x.Id,
                        ReviewId = x.ReviewId,
                        ImageData = x.ImageData,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            }).ToListAsync();

            return reviews;
        }

        public async Task<GetReviewModel> GetReview(Guid id)
        {
            GetReviewModel review = await _context.Reviews.AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new GetReviewModel
                {
                    Id = x.Id,
                    PlaceId = x.PlaceId,
                    AssessmentId = x.AssessmentId != Guid.Empty ? x.AssessmentId : null,
                    Text = x.Text,
                    Images = x.Images.Select(x => new GetReviewImageModel
                    {
                        Id = x.Id,
                        ReviewId = x.ReviewId,
                        ImageData = x.ImageData,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                    }).ToList(),
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).FirstOrDefaultAsync();

            if (review == null)
            {
                throw new NotFoundException("Review Not Found");
            }

            return review;
        }

        public async Task<GetReviewModel> PostReview(PostReviewModel postReviewModel)
        {
            Globals.Entities.Review review = new Globals.Entities.Review
            {
                PlaceId = postReviewModel.PlaceId,
                Text = postReviewModel.Text,
                Images = postReviewModel.Images?.Select(x => new ReviewImage
                {
                    ImageData = x.ImageData,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                }).ToList(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var place = await _context.Places.FindAsync(review.PlaceId);
            if (place != null)
            {
                place.ReviewId = review.Id;
                await _context.SaveChangesAsync();
            }

            //toDO not finshed stripe
            //this is code to create a stripe apayment
            /*string stripeAccountId = place.Reviewer.StripeAccountId;
            //toDo handle errors
            int amount = CalculateAmount(place.Reviewer.Score);

            var optionsTransfer = new TransferCreateOptions
            {
                Amount = amount,
                Currency = "eur",
                Destination = stripeAccountId,
            };
            var serviceTransfer = new TransferService();
            serviceTransfer.Create(optionsTransfer);*/

            //this doesnt work cause i need card or bank id
            /*var options = new PayoutCreateOptions
            {
                Amount = amount,  // amount in cents
                Currency = "eur",
                Method = "instant",  // payout method
                Destination = stripeAccountId, //this needs to be card id
            };

            var service = new PayoutService();
            Payout payout = service.Create(options);*/

            //end of the stripe code

            GetReviewModel getReviewModel = new GetReviewModel
            {
                Id = review.Id,
                PlaceId = review.PlaceId,
                AssessmentId = review.AssessmentId != Guid.Empty ? review.AssessmentId : null,
                Text = review.Text,
                Images = review.Images.Select(x => new GetReviewImageModel
                {
                    Id = x.Id,
                    ReviewId = x.ReviewId,
                    ImageData = x.ImageData,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).ToList(),
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
            };

            return getReviewModel;
        }

        public int CalculateAmount(double userRating)
        {
            double platformFeePercentage = 0.1;
            int totalAmount = 10;
            int userReceives = (int)((1 - platformFeePercentage) * (userRating / 5) * totalAmount * 100);
            return userReceives;

        }
    }
}
