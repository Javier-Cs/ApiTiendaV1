namespace ApiTiendaV1.Validation
{
    public class CsrfMiddleware
    {
        private readonly RequestDelegate _next;

        public CsrfMiddleware(RequestDelegate next) { 
            _next = next;
        }

        public async Task Invoke(HttpContext context) {
            if (
                HttpMethods.IsPost(context.Request.Method) ||
                HttpMethods.IsPut(context.Request.Method) ||
                HttpMethods.IsDelete(context.Request.Method)
                ) {

                var origin = context.Request.Headers["Origin"].ToString();

                if (
                    !string.IsNullOrEmpty(origin) &&
                    origin != "https://legumfrutsa.com" &&
                    origin != "https://legumfrutsa.com" &&
                    origin != "https://apioper.legumfrutsa.com" &&
                    origin != "http://localhost:4321" &&
                    origin != "https://localhost:44313"&&
                    origin != "http://localhost:4200" &&
                    origin != "https://radiosys.legumfrutsa.com" &&
                    origin != "https://borrador.cedesystem.com"
                    ) {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("No compa, su peticion de este origen no esta permitido");
                    return;
                }
            }
            await _next(context);
        }
    }
}
