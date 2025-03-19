using System.ComponentModel.DataAnnotations;

namespace ConsultorioNet.Models
{
    public class PatientMedicalInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(5)]
        public string BloodType { get; set; }

        public short? Height { get; set; }

        public short? Weight { get; set; }
    }
}