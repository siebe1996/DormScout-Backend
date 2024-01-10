using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Places
{
    public class PutPlaceModel
    {
        public DateTime? ChosenDate { get; set; }

        public Guid? ReviewerId { get; set; }

        public Guid? ReviewId { get; set; }
    }
}
