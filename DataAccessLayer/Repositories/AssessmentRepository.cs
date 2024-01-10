using DataAccessLayer.Repositories.interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Assessments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class AssessmentRepository : IAssessmentRepository
    {
        private readonly Backend_DormScoutContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        public AssessmentRepository(Backend_DormScoutContext backend_DormScoutContext, IHttpContextAccessor httpContextAccessor)
        {
            _context = backend_DormScoutContext;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<List<GetAssessmentModel>> GetAssessments()
        {
            List<GetAssessmentModel> assessments = await _context.Assessments
                .AsNoTracking()
                .Select(x => new GetAssessmentModel
                {
                    Id = x.Id,
                    ReviewId = x.ReviewId,
                    Score = x.Score,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).ToListAsync();

            return assessments;
        }

        public async Task<GetAssessmentModel> GetAssessment(Guid id)
        {
            GetAssessmentModel assessment =  await _context.Assessments
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new GetAssessmentModel
                {
                    Id = x.Id,
                    ReviewId = x.ReviewId,
                    Score = x.Score,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).FirstOrDefaultAsync();

            if (assessment == null)
            {
                throw new NotFoundException("Assessment Not Found");
            }

            return assessment;
        }

        public async Task<GetAssessmentModel> PostAssessment(PostAssessmentModel postAssessmentModel)
        {
            Assessment assessment = new Assessment 
            {
                ReviewId = postAssessmentModel.ReviewId,
                Score = postAssessmentModel.Score,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _context.Assessments.Add(assessment);
            await _context.SaveChangesAsync();

            var review = await _context.Reviews.FindAsync(assessment.ReviewId);
            Guid? reviewerId = assessment.Review.Place.ReviewerId;
            if (review != null)
            {
                review.AssessmentId = assessment.Id;
                await _context.SaveChangesAsync();
            }

            List<GetAssessmentModel> assessmentsReviewer = await _context.Assessments
                .AsNoTracking()
                .Where(x => x.Review.Place.ReviewerId == reviewerId)
                .Select(x => new GetAssessmentModel
                {
                    Id = x.Id,
                    ReviewId = x.ReviewId,
                    Score = x.Score,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).ToListAsync();

            double averageScore = assessmentsReviewer.Count > 0 ? assessmentsReviewer.Average(a => a.Score) : 0;

            User user = await _context.Users.FirstOrDefaultAsync(x => x.Id == reviewerId);
            
            if (user == null)
            {
                throw new NotFoundException("Reviewer not found");
            }

            user.Score = averageScore;

            await _context.SaveChangesAsync();

            GetAssessmentModel assessmentModel = new GetAssessmentModel
            {
                Id = assessment.Id,
                ReviewId = assessment.ReviewId,
                Score = assessment.Score,
                CreatedAt = assessment.CreatedAt,
                UpdatedAt = assessment.UpdatedAt,
            };

            return assessmentModel;
        }
    }
}
