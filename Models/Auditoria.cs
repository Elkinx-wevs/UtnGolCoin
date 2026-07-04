using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTNGolCoin.API.Models
{
    public class Auditoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Accion { get; set; }

        [Required]
        [MaxLength(100)]
        public string Entidad { get; set; }

        public int EntidadId { get; set; }

        public DateTime FechaHora { get; set; }

        public string? DetallesJson { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }
    }
}
