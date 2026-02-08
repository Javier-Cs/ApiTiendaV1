namespace ApiTiendaV1.DTOs.VentaDt
{
    public class VentaUpDto
    {
        public string descripcion_venta { get; set; }
        public string tipo_venta { get; set; }
        public decimal? efectivo_recibido { get; set; }
        public decimal? monto_total_Venta { get; set; }
        //public decimal? monto_vuelto { get; set; }

    }
}
