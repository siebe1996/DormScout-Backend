using Models.Notes;
using Models.PossibleDates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.interfaces
{
    public interface IPossibleDateRepository
    {
        Task<List<GetPossibleDateModel>> GetPossibleDates();
        Task<GetPossibleDateModel> GetPossibleDate(Guid id);
        Task<GetPossibleDateModel> PostPossibleDate(PostPossibleDateModel postPossibleDateModel);
    }
}
