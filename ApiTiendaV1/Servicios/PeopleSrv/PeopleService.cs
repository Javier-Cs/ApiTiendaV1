using ApiTiendaV1.DTOs.DataPeopleDt;
using System.Text.Json;

namespace ApiTiendaV1.Servicios.PeopleSrv
{
    public class PeopleService
    {
        private readonly string _urlBase;
        private readonly HttpClient _httpClient;

        public PeopleService(HttpClient httpClient ,IConfiguration configuration)
        {
            _urlBase = configuration.GetValue<string>("urlApiDatos");
            _httpClient = httpClient;
        }


        public async Task<DataPeopleResponseDto> GetDataPeople(string cedula)
        {
            var response = await _httpClient.GetAsync($"{_urlBase}/{cedula}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<DataPeopleResponseDto>(
                    content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );
            }
            else
            {
                throw new Exception($"Error al obtener los datos: {response.ReasonPhrase}");
            }
        }


    }
}
