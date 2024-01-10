using Models.Notes;
using Models.PossibleDates;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Places
{
    public class BasePlaceModel
    {
        [Required]
        public string Address { get; set; }

        [Required]
        public string HomeownerTelephone { get; set; }

        [Required]
        public string HomeownerEmail { get; set; }

        public string Link { get; set; }
    }
}
