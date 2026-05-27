using ApiTiendaV1.Data;
using ApiTiendaV1.DTOs;
using ApiTiendaV1.DTOs.VentaDt;
using ApiTiendaV1.Modelos;
using Dapper;
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
        
        
        public async Task<List<ValoresVentasDto>> ObtenerValoresVentasAsync(
            ValoresConsultVentasDto valores, 
            CancellationToken ct = default) 
        {
            
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



        


    }

}
