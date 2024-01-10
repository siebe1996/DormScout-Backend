using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.PlaceImages
{
    public class PatchPlaceImageModel : BasePlaceImageModel
    {
         public Guid? Id { get; set; }

        public Guid? PlaceId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
