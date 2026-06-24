namespace ApiTiendaV1.DTOs.VentaDt
{
    public class ValoresVentasDto
    {
        public int? id_venta { get; set; }
        public decimal? monto_total_Venta { get; set; }
        public string? descripcion_venta { get; set; } = null;
        public DateTime? date_venta { get; set; }
    }


    public class ValoresConsultVentasDto
    {
        public int id_venta { get; set; }
        public string? estado_venta { get; set; }

    }
}
