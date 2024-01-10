using Globals.Entities;
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
    public class GetPlaceModel : BasePlaceModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime? ChosenDate { get; set; }

        [Required]
        public Guid RequesterId { get; set; }

        [Required]
        public Guid? ReviewerId { get; set; }
        [Required]
        public Guid? ReviewId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public GetCoordinateModel Coordinate { get; set; }

        [Required]
        public List<GetPossibleDateModel> Dates { get; set; }

        public List<GetPlaceImageModel> Images { get; set; }

        public List<GetNoteModel> Notes { get; set; }
    }
}
