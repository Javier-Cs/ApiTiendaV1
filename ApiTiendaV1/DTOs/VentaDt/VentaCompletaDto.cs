namespace ApiTiendaV1.DTOs
{
    public class VentaCompletaDto
    {
        public int id_venta { get; set; }
        public int id_cliente { get; set; }        
        public string? nombre_vendedor { get; set; }
        public string? descripcion_venta { get; set; }
        public string? tipo_venta { get; set; }
        public string? estado_venta { get; set; }
        public decimal efectivo_recibido { get; set; }
        public decimal monto_total_Venta { get; set; }
        public decimal monto_vuelto { get; set; }
        public DateTime fecha_venta { get; set; }
    }
}