using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultorioNet.Models
{
    public class Consultorio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [MaxLength(50)]
        public string Number { get; set; }

        [Required]
        public short Type { get; set; }

        [Required]
        public int MedicalCenterId { get; set; }
    }
}