using ProyectoSantaMonica_Cesar.Models;

namespace ProyectoSantaMonica_Cesar.Service.Auth
{
    public interface ICustomUserService
    {
        Task<Usuario?> GetByUsernameAsync(string username);
    }
}
