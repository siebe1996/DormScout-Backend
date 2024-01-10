using Models.Assessments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.interfaces
{
    public interface IAssessmentRepository
    {
        Task<List<GetAssessmentModel>> GetAssessments();
        Task<GetAssessmentModel> GetAssessment(Guid id);
        Task<GetAssessmentModel> PostAssessment(PostAssessmentModel postAssessmentModel);
    }
}
