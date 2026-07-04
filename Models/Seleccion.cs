using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTNGolCoin.API.Models
{
    public class Seleccion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(3)]
        public string CodigoFifa { get; set; }

        [Required]
        [MaxLength(20)]
        public string Confederacion { get; set; }

        [MaxLength(500)]
        public string? BanderaUrl { get; set; }

        [Required]
        public int GrupoId { get; set; }

        [ForeignKey("GrupoId")]
        public Grupo? Grupo { get; set; }
    }
}
