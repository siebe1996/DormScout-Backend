using Globals.Entities;
using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Authentication
{
    public class ClientSecretModel
    {
        [Required]
        public string ClientSecret { get; set; }
    }
}
