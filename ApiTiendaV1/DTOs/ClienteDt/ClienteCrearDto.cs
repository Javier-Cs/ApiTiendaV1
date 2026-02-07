using ApiTiendaV1.DTOs.VentaDt;

namespace ApiTiendaV1.DTOs.ClienteDt
{
    public class ClienteCrearDto
    {
        public string nombre { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public string tipo { get; set; }
        public bool estado { get; set; }
        public DateTime fecha_creacion { get; set; }
    }
}

