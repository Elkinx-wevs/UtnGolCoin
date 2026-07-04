using System.ComponentModel.DataAnnotations;

namespace UTNGolCoin.API.Models
{
    public class Grupo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Nombre { get; set; }

        public List<Seleccion>? Selecciones { get; set; }
    }
}
