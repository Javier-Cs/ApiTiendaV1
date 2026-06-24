using ApiTiendaV1.Data;
using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.DeudasPorPagarDto;
using ApiTiendaV1.DTOs.VentaDt;
using ApiTiendaV1.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiTiendaV1.Repositorios.PagoRop
{
    public class PagoRepo : IPagoRepo
    {
        private readonly ISqlConnectionFactory _sqlconnection;
        public PagoRepo(ISqlConnectionFactory sqlConnection)
        {
            _sqlconnection = sqlConnection;
        }
        
        
        public async Task<List<ValoresVentasDto>> ObtenerValoresVentasAsync(ValoresConsultVentasDto valores, CancellationToken ct = default) {  
            const string sql = @"
                select 
                v.id_venta,
                v.descripcion_venta,
                v.monto_total_Venta
	
                from ventas v
                INNER JOIN clientes c
	                ON c.id_cliente = v.id_cliente
                where v.id_venta  = @id_venta
	                AND v.estado_venta = @estado_venta;
            ";

            using var connection = _sqlconnection.CreateConnection();

            var resultados = await connection.QueryAsync<ValoresVentasDto>(
                new CommandDefinition(
                    sql, 
                    new
                    {
                        valores.id_venta,
                        valores.estado_venta
                    }, cancellationToken: ct)
            );

            return resultados.ToList();
        }


        public async Task CrearPagoAsync(ReporteClientePagoDto dto, CancellationToken ct = default)
        {

            var vuelto = dto.efectivo_recibido - dto.monto_total_Venta;
            var numeroVentas = dto.lista_id_vents.Count();

            if (vuelto < 0)
                throw new Exception("El efectivo recibido es insuficiente.");

            using var connection = _sqlconnection.CreateConnection();
            connection.Open();

            using var transaccion = connection.BeginTransaction();

            try
            {
                // primero guardamos los registros del el pago
                const string sqlPago = @"insert into registro_pago_Ventas (
                        id_clientef,
                        efectivo_recibido_del_pago,
                        valor_a_pagar,
                        vuelto_de_deudas_Totales,
                        numero_ventas)
                        values(
                        @id_cliente,
                        @efectivo_recibido,
                        @monto_total_Venta,
                        @vuelto,
                        @numeroVentas)
                    select cast(scope_identity() as int);";

                /*
                 * nombre_cliente,
                        nombre_vendedor,
                        descripcion_de_pago,
                        
                 * ,
                        @nombre_cliente,
                        @nombre_vendedor,
                        @descripcion_de_pago)
                 */

                var idPago = await connection.ExecuteScalarAsync<int>(
                    sqlPago,
                    new
                    {
                        id_cliente = dto.id_cliente,
                        numeroVentas,
                        efectivo_recibido = dto.efectivo_recibido,
                        monto_total_Venta = dto.monto_total_Venta,
                        vuelto
                        //nombre_cliente = dto.nombre_cliente,
                        //nombre_vendedor = dto.nombre_vendedor,
                        //descripcion_de_pago = dto.descripcion_de_pago   
                    },
                    transaccion
                    );


                const string sqlventas = @"
                insert into pago_ventas (id_pagof, id_ventaf)
                values(@id_pagof, @id_ventaf)";

                foreach (var idVentas in dto.lista_id_vents)
                {
                    await connection.ExecuteAsync(sqlventas,
                        new
                        { 
                            id_pagof = idPago, 
                            id_ventaf = idVentas 
                        },
                        transaccion
                        );
                }


                const string sqlupdateVentas = @"
                update ventas 
                set estado_venta = 'PAGADO'
                WHERE id_venta in @ids
                and is_deleted = 0
                and estado_venta = 'DEUDA'";

                await connection.ExecuteAsync(
                    new CommandDefinition(
                    sqlupdateVentas,
                    new
                    {
                        ids = dto.lista_id_vents
                    },
                    transaccion,
                    cancellationToken: ct
                    )
                );

                transaccion.Commit();

            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                throw new Exception("Error al crear el pago: " + ex.Message);

            }

        }


        public async Task PagarDeudas(
     VentasAPagarConDeudaDto dto,
     CancellationToken ct)
        {
            using var connection = _sqlconnection.CreateConnection();

            connection.Open();

            using var tx = connection.BeginTransaction();

            try
            {
                // Obtener ids enviados
                var idsVentas = dto.lista_ventas
                    .Select(x => x.id_venta)
                    .ToList();

                // Consultar las ventas reales en la BD
                const string sqlVentas = @"
                SELECT
                    id_venta,
                    id_cliente,
                    monto_total_Venta,
                    estado_venta
                FROM ventas
                WHERE id_venta IN @ids
                AND is_deleted = 0";

                var ventas = (
                    await connection.QueryAsync<dynamic>(
                        sqlVentas,
                        new { ids = idsVentas },
                        tx
                    )
                ).ToList();

                if (!ventas.Any())
                    throw new Exception("No se encontraron ventas.");

                // Validar que todas estén en deuda
                if (ventas.Any(v => v.estado_venta != "DEUDA"))
                    throw new Exception(
                        "Existen ventas que no están en estado DEUDA."
                    );

                // Validar que todas las ventas pertenezcan al mismo cliente
                var clientes = ventas
                    .Select(v => (int)v.id_cliente)
                    .Distinct()
                    .ToList();

                if (clientes.Count > 1)
                    throw new Exception(
                        "Las ventas pertenecen a diferentes clientes."
                    );

                var numeroDeVentas = dto.lista_ventas.Count;

                var valorTotalDeTodasLasventas =
                    dto.lista_ventas.Sum(x => x.monto_total_Venta);

                var vueltoTotal =
                    dto.efectivo_recibido -
                    valorTotalDeTodasLasventas;

                if (vueltoTotal < 0)
                    throw new Exception(
                        "El efectivo recibido es insuficiente."
                    );

                // Cabecera
                const string sqlCabecera = @"
                INSERT INTO reporte_pago_venta(
                    idcliente,
                    nombre_vendedor,
                    descripcion_pago,
                    numero_ventas,
                    monto_total,
                    efectivo_recibido,
                    vuelto,
                    fecha_pago
                )
                VALUES(
                    @idcliente,
                    @nombre_vendedor,
                    @descripcion,
                    @numeroDeVentas,
                    @montoTotal,
                    @efectivo,
                    @vuelto,
                    @fechaPago
                )

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var idReporte =
                    await connection.ExecuteScalarAsync<int>(
                        sqlCabecera,
                        new
                        {
                            idcliente = dto.cliente.id_cliente,

                            nombre_vendedor =
                                dto.nombre_vendedor,

                            descripcion =
                                dto.descripcion_de_pago,

                            numeroDeVentas,

                            montoTotal =
                                valorTotalDeTodasLasventas,

                            efectivo =
                                dto.efectivo_recibido,

                            vuelto =
                                vueltoTotal,

                            fechaPago =
                                dto.fechaPago ?? DateTime.Now
                        },
                        tx
                    );

                // Detalle
                const string sqlDetalle = @"
                INSERT INTO detalle_reporte_pago_venta(
                    id_reporte_pago,
                    id_venta,
                    monto_venta
                )
                VALUES(
                    @idReporte,
                    @idVenta,
                    @monto
                )";

                foreach (var venta in dto.lista_ventas)
                {
                    await connection.ExecuteAsync(
                        sqlDetalle,
                        new
                        {
                            idReporte,
                            idVenta = venta.id_venta,
                            monto = venta.monto_total_Venta
                        },
                        tx
                    );
                }

                // Actualizar ventas
                const string sqlActualizar = @"
                UPDATE ventas
                SET estado_venta = 'PAGADO'
                WHERE id_venta IN @ids
                AND estado_venta = 'DEUDA'
                AND is_deleted = 0";

                await connection.ExecuteAsync(
                    sqlActualizar,
                    new
                    {
                        ids = idsVentas
                    },
                    tx
                );

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }

}
