using System.ComponentModel.DataAnnotations;

namespace Papeleria.Core.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;

        public int? ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public string NombreClienteMostrador { get; set; } = "Cliente General";

        [Required]
        public string MetodoPago { get; set; } = "Efectivo";

        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();

        public decimal TotalVenta { get; set; }

        public void CalcularTotal()
        {
            decimal suma = 0;
            foreach (var item in Detalles)
            {
                suma += item.Subtotal;
            }
            TotalVenta = suma;
        }
    }

    public class DetalleVenta
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
