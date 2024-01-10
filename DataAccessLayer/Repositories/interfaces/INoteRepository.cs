using Models.Assessments;
using Models.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.interfaces
{
    public interface INoteRepository
    {
        Task<List<GetNoteModel>> GetNotes();
        Task<GetNoteModel> GetNote(Guid id);
        Task<GetNoteModel> PostNote(PostNoteModel postNoteModel);
    }
}
