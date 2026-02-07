namespace ApiTiendaV1.Modelos
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Tipo { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }

        public ICollection<Venta> Ventas { get; set; }
    }
}
