using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientCare.Models.CRM
{
    public partial class Netværk
    {
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
