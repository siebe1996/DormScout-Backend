
using Models.Coordinates;
using Models.Notes;
using Models.PlaceImages;
using Models.PossibleDates;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Places
{
    public class PatchPlaceModel : BasePlaceModel
    {
        public Guid? Id { get; set; }

        public DateTime? ChosenDate { get; set; }

        public Guid? RequesterId { get; set; }

        public Guid? ReviewerId { get; set; }

        public Guid? ReviewId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public GetCoordinateModel? Coordinate { get; set; }

        public List<GetPossibleDateModel>? Dates { get; set; }

        public List<GetPlaceImageModel>? Images { get; set; }

        public List<GetNoteModel>? Notes { get; set; }
    }
}
