using Globals.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Authentication
{
    public class PaymentIntentCreateRequestModel
    {
        [Required]
        public Item[] Items { get; set; }
    }
}
