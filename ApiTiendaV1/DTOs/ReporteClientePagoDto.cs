using ApiTiendaV1.DTOs.VentaDt;

namespace ApiTiendaV1.DTOs
{
    public class ReporteClientePagoDto
    {
        public int id_cliente { get; set; }
        //public List<ValoresVentasDto>? lista_ventas { get; set; }
        public List<int>? lista_id_vents { get; set; }
        public decimal efectivo_recibido { get; set; }
        public decimal monto_total_Venta { get; set; }

        // datos nuevos
        /*
        public string? nombre_cliente { get; set; } = null;
        public string? nombre_vendedor { get; set; } = null;
        public string? descripcion_de_pago { get; set; } = null;*/
    }
}
