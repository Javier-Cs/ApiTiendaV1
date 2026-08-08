using ApiTiendaV1.DTOs.Stream;

namespace ApiTiendaV1.Servicios.Streams
{
    public class StreamService
    {
        private readonly HttpClient _httpClient;

        public StreamService(HttpClient httpClient) { 
            _httpClient = httpClient;
        }

        public async Task<StreamResponseDto> GetStream(string url) {
     
            if (string.IsNullOrEmpty(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute)) {
                throw new Exception("Error");
            }
            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Icy-MetaData", "1");

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            Console.WriteLine(response.StatusCode);

            if (!response.IsSuccessStatusCode) {
                throw new Exception("No se pudo conectar con el servidor de streaming.");
            }

            response.Headers.TryGetValues("icy-br", out var icyBrvalues);
            int.TryParse(icyBrvalues?.FirstOrDefault(), out int bitrate);

            response.Headers.TryGetValues("Server", out var serverValues);
            response.Headers.TryGetValues("icy-genre", out var genreValues);

            var streamData = new StreamResponseDto {
                Content_Type = response.Content.Headers.ContentType?.ToString() ?? "unknown",
                icy_br = bitrate,
                Server = serverValues?.FirstOrDefault() ?? "unknown",
                icy_genre = genreValues?.FirstOrDefault() ?? "unknown",

            };

            return streamData;

        }
    }
}
