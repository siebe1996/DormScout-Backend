using DataAccessLayer.Repositories.interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Notes;
using Models.Places;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;
        private readonly Backend_DormScoutContext _context;

        public NoteRepository(Backend_DormScoutContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<GetNoteModel> GetNote(Guid id)
        {
            GetNoteModel note = await _context.Notes
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new GetNoteModel
                {
                    Id = x.Id,
                    PlaceId = x.PlaceId,
                    Content = x.Content,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                }).FirstOrDefaultAsync();
            if (note == null)
            {
                throw new NotFoundException("Note Not Found");
            }
            return note;
        }

        public async Task<List<GetNoteModel>> GetNotes()
        {
            Guid userId = new Guid(_user.Identity.Name);
            List<GetNoteModel> notes = await _context.Notes
                .AsNoTracking()
                .Select(x => new GetNoteModel
                {
                    Id = x.Id,
                    PlaceId = x.PlaceId,
                    Content = x.Content,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                })
            .ToListAsync();

            return notes;
        }

        public async Task<GetNoteModel> PostNote(PostNoteModel postNoteModel)
        {
            Guid userId = new Guid(_user.Identity.Name);
            Note note = new Note
            {
                Content = postNoteModel.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            GetNoteModel getNoteModel = new GetNoteModel
            {
                Id = note.Id,
                PlaceId = note.PlaceId,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
            };
            return getNoteModel;
        }
    }
}
