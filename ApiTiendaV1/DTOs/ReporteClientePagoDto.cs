namespace ApiTiendaV1.DTOs
{
    public class ReporteClientePagoDto
    {
        public int id_cliente { get; set; }
        public List<int> lista_id_vents { get; set; }
        public decimal efectivo_recibido { get; set; }
        public decimal monto_total_Venta { get; set; }

    }
}
