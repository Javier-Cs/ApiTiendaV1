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

                var idPago = await connection.ExecuteScalarAsync<int>(
                    sqlPago,
                    new
                    {
                        id_cliente = dto.id_cliente,
                        numeroVentas,
                        efectivo_recibido = dto.efectivo_recibido,
                        monto_total_Venta = dto.monto_total_Venta,
                        vuelto
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

        }}
}
