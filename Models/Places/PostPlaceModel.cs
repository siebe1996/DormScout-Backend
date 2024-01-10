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
    public class PostPlaceModel : BasePlaceModel
    {
        [Required]
        public PostCoordinateModel Coordinate { get; set; }

        [Required]
        public List<PostPossibleDateModel> Dates { get; set; }

        public List<PostPlaceImageModel>? Images { get; set; }

        public List<PostNoteModel>? Notes { get; set; }
    }
}
