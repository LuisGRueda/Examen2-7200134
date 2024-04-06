using Examen2_7200134.Models;
using Examen2_7200134.Models.DtoModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examen2_7200134.Contratos
{
    public interface IReportsLogic
    {
        public Task<bool> CreatePedidoDetalle(Pedido pedido, Detalle detalle);
        public Task<List<ReportePedidoDTO>> ObtenerReportePedidos();
        public Task<List<ProductoMasPedidoDTO>> ObtenerProductosMasPedidos();
        public Task<bool> EliminarClienteCascada(int id);
        public Task<List<ProductoMasVendidoDTO>> ObtenerProductosMasVendidos(DateTime fechaInicio, DateTime fechaFin);
    }
}
