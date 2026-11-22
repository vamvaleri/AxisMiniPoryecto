using System.ComponentModel.DataAnnotations;

namespace Papeleria.Core.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es obligatorio (ej. CU-001)")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Categoria { get; set; } = "General";

        [Range(0.01, 10000000, ErrorMessage = "El precio de venta debe ser mayor a 0")]
        public decimal PrecioVenta { get; set; }

        [Range(0, 100000, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        public int StockMinimo { get; set; } = 5;

        public bool TieneStockDisponible(int cantidadSolicitada)
        {
            return Stock >= cantidadSolicitada && cantidadSolicitada > 0;
        }

        public void DescontarStock(int cantidad)
        {
            if (!TieneStockDisponible(cantidad))
                throw new InvalidOperationException($"Stock insuficiente para {Nombre}. Disponible: {Stock}");
            Stock -= cantidad;
        }

        public void ReponerStock(int cantidad)
        {
            if (cantidad > 0)
                Stock += cantidad;
        }

        public virtual string EstadoInventario()
        {
            if (Stock == 0) return "Agotado";
            if (Stock <= StockMinimo) return "Stock Bajo";
            return "Disponible";
        }
    }

    public class ProductoEscolar : Producto
    {
        public override string EstadoInventario()
        {
            if (Stock == 0) return "Agotado (Urgente)";
            if (Stock <= StockMinimo * 2) return "Reponer Pronto";
            return "Disponible";
        }
    }
}
