using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientCare.Models.CRM
{
    [Table("Medlemmer", Schema = "dbo")]
    public partial class Medlem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string Email { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string Name { get; set; }

        [ConcurrencyCheck]
        public string Direktør { get; set; }

        [ConcurrencyCheck]
        public int AntalAnsatte { get; set; }

        [ConcurrencyCheck]
        public int CVR { get; set; }

        [ConcurrencyCheck]
        public decimal Kontigent { get; set; }

        [ConcurrencyCheck]
        public DateTime Indmeldelsesdato { get; set; }

        [ConcurrencyCheck]
        public int RelationsansvarligId { get; set; }

        [ConcurrencyCheck]
        public int BrancheId { get; set; }

        public Branche Branche { get; set; }

        public RelationsAnsvarlig RelationsAnsvarlig { get; set; }

        public ICollection<Netværk> Netværk { get; set; }

    }
}
