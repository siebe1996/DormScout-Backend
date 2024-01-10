using Models.Places;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.interfaces
{
    public interface IPlaceRepository
    {
        Task<List<GetPlaceModel>> GetPlacesNotYours();
        Task<List<GetPlaceModel>> GetPlacesYours();
        Task<List<GetPlaceModel>> GetPlacesNearby(double lat, double lon);
        Task<List<GetPlaceModel>> GetPlacesReviewer();
        Task<GetPlaceModel> GetPlace(Guid id);
        Task<GetPlaceModel> PostPlace(PostPlaceModel postPlaceModel);
        Task<GetPlaceModel> PutPlace(Guid id, PutPlaceModel putPlaceModel);
        Task<GetPlaceModel> PatchPlace(Guid id, PatchPlaceModel patchPlaceModel);
    }
}
