using Globals.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.PlaceImages
{
    public class BasePlaceImageModel
    {
        [Required]
        public byte[] ImageData { get; set; }
    }
}
