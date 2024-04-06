using Examen2_7200134.Contratos;
using Examen2_7200134.Models;
using Examen2_7200134.Models.DtoModels;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examen2_7200134.Implementacion
{
    public class ReportsLogic: IReportsLogic
    {
        private readonly Contexto contexto;

        public ReportsLogic(Contexto contexto)
        {
            this.contexto = contexto;
        }
        public async Task<bool> CreatePedidoDetalle(Pedido pedido, Detalle detalle)
        {
            bool sw = false;
            contexto.Pedidos.Add(pedido);
            int response = await contexto.SaveChangesAsync();
            if (response == 1)
            {
                sw = true;
            }
            contexto.Detalles.Add(detalle);
            int response2 = await contexto.SaveChangesAsync();
            if (response == 1)
            {
                sw = true;
            }
            return sw;
        }
        public async Task<List<ReportePedidoDTO>> ObtenerReportePedidos()
        {
            var reporte = await (from pedido in contexto.Pedidos
                                 join cliente in contexto.Clientes on pedido.IdCliente equals cliente.Id
                                 join detalle in contexto.Detalles on pedido.IdPedido equals detalle.IdPedido
                                 join producto in contexto.Productos on detalle.IdProducto equals producto.IdProducto
                                 select new ReportePedidoDTO
                                 {
                                     NombreCliente = cliente.Nombre + " " + cliente.Apellido,
                                     FechaPedido = pedido.Fecha,
                                     NombreProducto = producto.Nombre,
                                     Subtotal = detalle.SubTotal
                                 }).ToListAsync();

            return reporte;
        }
        public async Task<List<ProductoMasPedidoDTO>> ObtenerProductosMasPedidos()
        {
            var productosMasPedidos = await contexto.Detalles
                .GroupBy(x => x.IdProducto)
                .Select(y => new
                {
                    IdProducto = y.Key,
                    CantidadTotal = y.Sum(d => d.Cantidad)
                })
                .OrderByDescending(p => p.CantidadTotal)
                .Take(3)
                .ToListAsync();

            var productosMasPedidosDTO = new List<ProductoMasPedidoDTO>();
            foreach (var productoMasPedido in productosMasPedidos)
            {
                var producto = await contexto.Productos.FindAsync(productoMasPedido.IdProducto);
                if (producto != null)
                {
                    productosMasPedidosDTO.Add(new ProductoMasPedidoDTO
                    {
                        IdProducto = producto.IdProducto,
                        Nombre = producto.Nombre,
                        CantidadTotal = productoMasPedido.CantidadTotal
                    });
                }
            }

            return productosMasPedidosDTO;
        }
        public async Task<bool> EliminarClienteCascada(int id)
        {
            var cliente = await contexto.Clientes.FindAsync(id);
            if (cliente == null)
            {
                throw new Exception($"No se encontró ninguna persona con el ID {id}");
            }

            var pedidos = contexto.Pedidos.Where(p => p.IdCliente == id);
            foreach (var pedido in pedidos)
            {
                contexto.Pedidos.Remove(pedido);
            }

            contexto.Clientes.Remove(cliente);

            await contexto.SaveChangesAsync();

            return true;
        }
        public async Task<List<ProductoMasVendidoDTO>> ObtenerProductosMasVendidos(DateTime fechaInicio, DateTime fechaFin)
        {
            var productosMasVendidos = await contexto.Detalles
                .Where(d => d.Pedido.Fecha >= fechaInicio && d.Pedido.Fecha <= fechaFin)
                .GroupBy(d => d.IdProducto)
                .Select(g => new
                {
                    IdProducto = g.Key,
                    CantidadTotal = g.Sum(d => d.Cantidad)
                })
                .OrderByDescending(p => p.CantidadTotal)
                .Take(3) 
                .ToListAsync();

            var productosMasVendidosDTO = new List<ProductoMasVendidoDTO>();
            foreach (var productoMasVendido in productosMasVendidos)
            {
                var producto = await contexto.Productos.FindAsync(productoMasVendido.IdProducto);
                if (producto != null)
                {
                    productosMasVendidosDTO.Add(new ProductoMasVendidoDTO
                    {
                        IdProducto = producto.IdProducto,
                        Nombre = producto.Nombre,
                        CantidadTotal = productoMasVendido.CantidadTotal
                    });
                }
            }

            return productosMasVendidosDTO;
        }

        
    }
}
