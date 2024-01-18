using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ModulAR.Models
{
    public class Escaparate
    {

        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }
        public int ProductoId { get; set; }
        public virtual Producto? Producto { get; set; }
    }
}
