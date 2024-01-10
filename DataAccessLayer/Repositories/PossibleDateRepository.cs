using DataAccessLayer.Repositories.interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Notes;
using Models.PossibleDates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class PossibleDateRepository : IPossibleDateRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;
        private readonly Backend_DormScoutContext _context;

        public PossibleDateRepository(Backend_DormScoutContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<GetPossibleDateModel> GetPossibleDate(Guid id)
        {
            GetPossibleDateModel date = await _context.PossibleDates
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new GetPossibleDateModel
                {
                    Id = x.Id,
                    PlaceId = x.PlaceId,
                    Date = x.Date,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).FirstOrDefaultAsync();
            if (date == null)
            {
                throw new NotFoundException("Possible Date Note Found");
            }
            return date;
        }

        public async Task<List<GetPossibleDateModel>> GetPossibleDates()
        {
            Guid userId = new Guid(_user.Identity.Name);
            List<GetPossibleDateModel> dates = await _context.PossibleDates
                .AsNoTracking()
                .Select(x => new GetPossibleDateModel
                {
                    Id = x.Id,
                    PlaceId = x.PlaceId,
                    Date = x.Date,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                })
            .ToListAsync();

            return dates;
        }

        public async Task<GetPossibleDateModel> PostPossibleDate(PostPossibleDateModel postPossibleDateModel)
        {
            Guid userId = new Guid(_user.Identity.Name);
            PossibleDate date = new PossibleDate
            {
                Date = postPossibleDateModel.Date,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _context.PossibleDates.Add(date);
            await _context.SaveChangesAsync();

            GetPossibleDateModel getPossibleDateModel = new GetPossibleDateModel
            {
                Id = date.Id,
                PlaceId = date.PlaceId,
                Date = date.Date,
                CreatedAt = date.CreatedAt,
                UpdatedAt = date.UpdatedAt,
            };
            return getPossibleDateModel;
        }
    }
}
