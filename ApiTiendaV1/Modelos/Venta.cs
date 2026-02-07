namespace ApiTiendaV1.Modelos
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public int IdCliente { get; set; }
        public string Nombre_vendedor { get; set; } 
        public string Descripcion_venta { get; set; }
        public decimal EfectivoRecibido { get; set; }
        public decimal Monto { get; set; }
        public decimal Vuelto { get; set; }
        public DateTime FechaVenta { get; set; }

        public Cliente Cliente { get; set; }
    }
}
