using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTNGolCoin.API.Models
{
    public class Partido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        [Required]
        [MaxLength(30)]
        public string Fase { get; set; }

        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "Programado";

        public int? GolesLocal { get; set; }

        public int? GolesVisitante { get; set; }

        [Required]
        public int SeleccionLocalId { get; set; }

        [ForeignKey("SeleccionLocalId")]
        public Seleccion? SeleccionLocal { get; set; }

        [Required]
        public int SeleccionVisitanteId { get; set; }

        [ForeignKey("SeleccionVisitanteId")]
        public Seleccion? SeleccionVisitante { get; set; }

        [Required]
        public int SedeId { get; set; }

        [ForeignKey("SedeId")]
        public Sede? Sede { get; set; }
    }
}
