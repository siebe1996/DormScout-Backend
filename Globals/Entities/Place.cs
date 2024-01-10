using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class Place
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Address {  get; set; }

        [Required]
        public Guid CoordinateId { get; set; }

        public DateTime? ChosenDate { get; set; }

        [Required]
        public Guid RequesterId { get; set; }

        public Guid? ReviewerId { get; set; }

        public Guid? ReviewId { get; set; }

        [Required]
        public string HomeownerTelephone { get; set; }

        [Required]
        public string HomeownerEmail { get; set; }

        [Required]
        public string Link { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public virtual User Requester { get; set; }
        public virtual User Reviewer { get; set; }
        public virtual Review Review { get; set; }
        public virtual Coordinate Coordinate { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<PossibleDate> Dates { get; set; }
        public virtual ICollection<PlaceImage> Images { get; set; }
    }
}
