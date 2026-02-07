namespace ApiTiendaV1.DTOs
{
    public class ReporteClienteVentaDto
    {
        public int id_pago_venta { get; set; }
        public string nombre_cliente { get; set; }
        public int cantidad_de_Venta { get; set; }
        public decimal efectivo_recibido { get; set; }
        public decimal monto_total_Venta { get; set; }
        public decimal monto_Vuelto {  get; set; }
        public DateTime fecha_de_pago { get; set; }

    }
}
