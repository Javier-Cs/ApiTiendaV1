namespace ApiTiendaV1.DTOs.VentaDt
{
    public class BuscarVenta
    {
        public DateTime fecha_venta { get; set; }
        public int id_cliente { get; set; }
        public string tipo_venta { get; set; }
    }
}
