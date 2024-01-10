using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.PossibleDates
{
    public class BasePossibleDateModel
    {
        [Required]
        public DateTime Date { get; set; }
    }
}
