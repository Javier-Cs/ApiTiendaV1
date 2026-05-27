using System.Text.Json.Serialization;

namespace ApiTiendaV1.DTOs.DataPeopleDt
{
    public class licenciasDeConducir
    {
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Celular { get; set; }
        public string? TipoLicencia { get; set; }
        public string LicenciaFechaDesde { get; set; }
        public string LicenciaFechaHasta { get; set; }
        public string? TipoSangre { get; set; }
        public string? FechaDefuncion { get; set; }
        public string? LugarDefuncion { get; set; }
        public string? MotivoDefuncion { get; set; }
    }
}
