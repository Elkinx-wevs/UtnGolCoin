using System.ComponentModel.DataAnnotations;

namespace UTNGolCoin.API.Models
{
    public class Sede
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string NombreEstadio { get; set; }

        [Required]
        [MaxLength(100)]
        public string Ciudad { get; set; }

        [Required]
        [MaxLength(50)]
        public string Pais { get; set; }

        public int Capacidad { get; set; }

        public List<Partido>? Partidos { get; set; }
    }
}
