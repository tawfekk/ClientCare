using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientCare.Models.CRM
{
    [Table("Netværk", Schema = "dbo")]
    public partial class Netværk
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string Name { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string UserId { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int MedlemId { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int StatusId { get; set; }

        public Medlem Medlem { get; set; }

        //public ICollection<Medlem> Medlem { get; set; }

    }
}
