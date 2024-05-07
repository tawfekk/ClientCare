using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientCare.Models.CRM
{
    [Table("RelationsAnsvarlige", Schema = "dbo")]
    public partial class RelationsAnsvarlig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ConcurrencyCheck]
        public string Name { get; set; }

    }
}
