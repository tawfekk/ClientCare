using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientCare.Models
{
     public partial class ApplicationUser
     {
         public string FirstName { get; set; }
         public string LastName { get; set; }
         public string Picture { get; set; }
     }
}