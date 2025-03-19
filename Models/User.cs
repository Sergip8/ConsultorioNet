using System.ComponentModel.DataAnnotations;

namespace ApiConsultorio.Models
{
    public class User
    {
    [Key]
    public long Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Email { get; set; }

    [Required]
    public int IsActive { get; set; }

    [Required]
    [StringLength(255)]
    public string Password { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    }

}
