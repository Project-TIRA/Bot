using System;
using System.ComponentModel.DataAnnotations;

namespace EntityModel
{
    public class OrganizationSnapshot
    {
        [Key]
        public int Id { get; set; }

        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public DateTime Date { get; set; }
        public int OpenBeds { get; set; }
    }
}
