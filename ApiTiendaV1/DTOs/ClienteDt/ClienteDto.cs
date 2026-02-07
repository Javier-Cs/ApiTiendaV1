namespace ApiTiendaV1.DTOs.ClienteDt
{
    public class ClienteDto
    {
        public int id_cliente { get; set; }
        public string nombre { get; set; }
        public string tipo { get; set; }
        public bool estado { get; set; }
        public DateTime fecha_creacion { get; set; }

    }

    public class ClienteSearchDto { 
        public int id_cliente { get; set; } = 0;
        public string nombre { get; set; } = string.Empty;
    }
}