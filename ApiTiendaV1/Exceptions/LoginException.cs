namespace ApiTiendaV1.Exceptions
{
    public class LoginException : Exception
    {
        public int IntentosRestantes { get; }
        public bool Bloqueado { get; }

        public LoginException( string mensaje, int intentosRestantes, bool bloqueados = false) : base(mensaje) {
            IntentosRestantes = intentosRestantes;
            Bloqueado = bloqueados;
        }
    }
}
