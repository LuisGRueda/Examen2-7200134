using Examen2_7200134.Contratos;
using Examen2_7200134.Models;
using Examen2_7200134.Models.DtoModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;

namespace Examen2_7200134.Endpoints
{
    public class Function
    {
        private readonly ILogger<Function> _logger;
        private readonly IReportsLogic reportsLogic;

        public Function(ILogger<Function> logger, IReportsLogic reportsLogic)
        {
            _logger = logger;
            this.reportsLogic = reportsLogic;
        }
        //1.-realizar un endpoint para que registre un pedido y su respectivo detalle
        [Function("RegistrarPedidoDetalle")]
        [OpenApiOperation("Reportsspec", "InsertarPedido", Description = "Sirve para insertar")]
        [OpenApiRequestBody("application/json", typeof(PedidoDetalleDTO),
           Description = "Institucion modelo")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
            bodyType: typeof(PedidoDetalleDTO),
            Description = "Mostrara objeto creado")]
        public async Task<HttpResponseData> RegistrarPedidoDetalle([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            try
            {
                var pedidoDetalleDTO = await req.ReadFromJsonAsync<PedidoDetalleDTO>() ?? throw new Exception("Debe ingresar un pedido");

                Pedido pedido = pedidoDetalleDTO.pedido;

                Detalle detalle = pedidoDetalleDTO.detalle;

                bool seGuardo = await reportsLogic.CreatePedidoDetalle(pedido, detalle);

                if (!seGuardo) return req.CreateResponse(HttpStatusCode.BadRequest);

                var respuesta = req.CreateResponse(HttpStatusCode.OK);

                return respuesta;
            }
            catch (Exception ex)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(ex.Message);
                return error;
            }
        }
        //2.- Listar el siguiente reporte: nombre de cliente, fecha de pedido, nombre de producto, subtotal
        [Function("ListarReportesPedidos")]
        [OpenApiOperation("Reportsspec", "ListarReporte", Description = "Sirve para listar ")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
            bodyType: typeof(List<ReportePedidoDTO>),
            Description = "Mostrara una lista ")]
        public async Task<HttpResponseData> ListarReportesPedidos([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ListarReportesPedidos")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para listar Reporte.");
            try
            {
                var listaReporte = reportsLogic.ObtenerReportePedidos();
                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                await respuesta.WriteAsJsonAsync(listaReporte.Result);
                return respuesta;
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }

        }
        //3.- Identificar los 3 productos mas pedidos
        [Function("ListarProductosMasPedidos")]
        [OpenApiOperation("Reportsspec", "ListarProductos", Description = "Sirve para listar tos")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json",
            bodyType: typeof(List<ProductoMasPedidoDTO>),
            Description = "Mostrara una lista ")]
        public async Task<HttpResponseData> ListarProductosMasPedidos([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ListarProductosMasPedidos")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para listar Reporte.");
            try
            {
                var listaReporte = reportsLogic.ObtenerProductosMasPedidos();
                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                await respuesta.WriteAsJsonAsync(listaReporte.Result);
                return respuesta;
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }

        }

        // realizar un endpoint para actualizar un pedido y su respectivo detalle
        //Realizar un endpoint para realizar un eliminado en cascada de la tabla cliente
        [Function("EliminarClienteCascada")]
        [OpenApiOperation("Reportsspec", "EliminarClienteCascada", Description = "Sirve para Eliminar")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string))]
        public async Task<HttpResponseData> EliminarClienteCascada([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "eliminarclientecascada/{id}")] HttpRequestData req, int id)
        {
            _logger.LogInformation($"Ejecutando azure function para eliminar persona con ID {id}");
            try
            {
                bool seElimino = await reportsLogic.EliminarClienteCascada(id);
                if (seElimino)
                {
                    var respuesta = req.CreateResponse(HttpStatusCode.OK);
                    return respuesta;
                }
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }
        //6.- Realizar un endpoiunt que determine cuales son los productos mas vendidos segun un rango de fechas como parametro
        [Function("ListarProductosMasVendido")]
        [OpenApiOperation("Reportsspec", "ListarProductos", Description = "Sirve para listar tos")]
        [OpenApiParameter(name: "fechaInicio", In = ParameterLocation.Path, Required = true, Type = typeof(DateTime), Description = "Fecha de inicio")]
        [OpenApiParameter(name: "fechaFin", In = ParameterLocation.Path, Required = true, Type = typeof(DateTime), Description = "Fecha de fin")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<ProductoMasVendidoDTO>), Description = "Mostrara una lista")]
        public async Task<HttpResponseData> ListarProductosMasVendido(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ListarProductosMasVendido/{fechaInicio}/{fechaFin}")] HttpRequestData req,
          DateTime fechaInicio,
          DateTime fechaFin)
        {
            _logger.LogInformation($"Ejecutando azure function para listar Reporte entre {fechaInicio} y {fechaFin}.");
            try
            {
                var listaReporte = await reportsLogic.ObtenerProductosMasVendidos(fechaInicio, fechaFin);
                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                await respuesta.WriteAsJsonAsync(listaReporte);
                return respuesta;
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }


    }
}
