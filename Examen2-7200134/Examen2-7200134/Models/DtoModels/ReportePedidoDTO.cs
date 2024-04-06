using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examen2_7200134.Models.DtoModels
{
    public class ReportePedidoDTO
    {
        public string NombreCliente { get; set; }
        public DateTime FechaPedido { get; set; }
        public string NombreProducto { get; set; }
        public decimal Subtotal { get; set; }
    }

}
