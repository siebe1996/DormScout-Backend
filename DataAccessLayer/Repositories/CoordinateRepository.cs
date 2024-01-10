using DataAccessLayer.Repositories.interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Coordinates;
using Models.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class CoordinateRepository : ICoordinateRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;
        private readonly Backend_DormScoutContext _context;

        public CoordinateRepository(Backend_DormScoutContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<GetCoordinateModel> GetCoordinate(Guid id)
        {
            GetCoordinateModel coordinate = await _context.Coordinates
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new GetCoordinateModel
                {
                    Id = x.Id,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).FirstOrDefaultAsync();
            if (coordinate == null)
            {
                throw new NotFoundException("Coordinate not Found");
            }
            return coordinate;
        }

        public async Task<List<GetCoordinateModel>> GetCoordinates()
        {
            Guid userId = new Guid(_user.Identity.Name);
            List<GetCoordinateModel> coordinates = await _context.Coordinates
                .AsNoTracking()
                .Select(x => new GetCoordinateModel
                {
                    Id = x.Id,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                })
            .ToListAsync();

            return coordinates;
        }

        public async Task<GetCoordinateModel> PostCoordinate(PostCoordinateModel postCoordinateModel)
        {
            Guid userId = new Guid(_user.Identity.Name);
            Coordinate coordinate = new Coordinate
            {
                Latitude = postCoordinateModel.Latitude,
                Longitude = postCoordinateModel.Longitude,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _context.Coordinates.Add(coordinate);
            await _context.SaveChangesAsync();

            GetCoordinateModel getCoordinateModel = new GetCoordinateModel
            {
                Id = coordinate.Id,
                Latitude = coordinate.Latitude,
                Longitude = coordinate.Longitude,
                CreatedAt = coordinate.CreatedAt,
                UpdatedAt = coordinate.UpdatedAt,
            };
            return getCoordinateModel;
        }
    }
}
