using ApiTiendaV1.Servicios.PeopleSrv;
using Microsoft.AspNetCore.Mvc;

namespace ApiTiendaV1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleDataController : ControllerBase
    {
        /*
        public readonly PeopleService _peopleService;
        public PeopleDataController(PeopleService peopleService)
        {
            _peopleService = peopleService;
        }

        [HttpGet("{cedula}")]
        public async Task<IActionResult> GetDataPeople(string cedula)
        {
            try
            {
                var dataPeople = await _peopleService.GetDataPeople(cedula);
                return Ok(dataPeople);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }¨*/
       
    }
}
