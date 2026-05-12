namespace ApiTiendaV1.DTOs.ClienteDt
{
    public class ClienteVentDeudor
    {
        public int id_venta { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string nombre_vendedor { get; set; } = string.Empty;
        public string tipo_venta { get; set; } = string.Empty;
        public string estado_venta { get; set; } = string.Empty;
        public int cantidadDeDeudas { get; set; } = 0;
        public decimal monto_total_Venta { get; set; } = 0;
        public DateTime fecha_venta { get; set; } = DateTime.MinValue;
    }
}
