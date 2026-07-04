using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTNGolCoin.API.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(150)]
        public string Correo { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime FechaRegistro { get; set; }

        [Required]
        public int RolId { get; set; }

        [ForeignKey("RolId")]
        public Rol? Rol { get; set; }
    }
}
