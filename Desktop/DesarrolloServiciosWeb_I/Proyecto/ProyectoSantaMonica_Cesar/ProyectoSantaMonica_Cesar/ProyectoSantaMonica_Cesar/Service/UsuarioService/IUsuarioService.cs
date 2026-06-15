using ProyectoSantaMonica_Cesar.Models;

namespace ProyectoSantaMonica_Cesar.Service.UsuarioService
{
    public interface IUsuarioService
    {
        Usuario? Login(string username, string password);
        string RegistrarUsuario(Usuario u);
        void ActualizarUsuario(Usuario nuevo, Usuario actual);
        Usuario BuscarPorNombre(string username);
        Usuario BuscarPorID(long id);
        Usuario BuscarPorUserName(string username);

    }
}
