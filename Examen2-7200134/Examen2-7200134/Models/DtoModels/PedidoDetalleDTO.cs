using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examen2_7200134.Models.DtoModels
{
    public class PedidoDetalleDTO
    {
        public Pedido pedido { get; set; }
        public Detalle detalle { get; set; }
    }
}
