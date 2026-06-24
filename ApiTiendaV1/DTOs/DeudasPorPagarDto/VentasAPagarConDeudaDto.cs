using ApiTiendaV1.DTOs.ClienteDt;
using ApiTiendaV1.DTOs.VentaDt;

namespace ApiTiendaV1.DTOs.DeudasPorPagarDto
{
    public class VentasAPagarConDeudaDto
    {
        public ClienteDatosrDto? cliente { get; set; }
        //public int numeroDeVentas { get; set; }
        public string? nombre_vendedor { get; set; } = null;
        public string? descripcion_de_pago { get; set; } = null;
        public DateTime? fechaPago { get; set;} = null;
        public List<ValoresVentasDto>? lista_ventas { get; set; }
        //public decimal monto_total_Venta { get; set; }
        public decimal efectivo_recibido { get; set; }
        //public decimal vuelto {  get; set; }
    }
}
