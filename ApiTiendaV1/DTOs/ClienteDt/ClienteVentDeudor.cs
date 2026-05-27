namespace ApiTiendaV1.DTOs.ClienteDt
{
    public class ClienteVentDeudor
    {
        public int id_cliente { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string nombre_vendedor { get; set; } = string.Empty;
        public int cantidadDeDeudas { get; set; } = 0;
        public decimal monto_total_Venta { get; set; } = 0;
        public DateTime fecha_ultima_venta { get; set; }
    }
}
