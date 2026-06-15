using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Repository;

namespace ProyectoSantaMonica_Cesar.Service.Auth
{
    public class CustomUserService : ICustomUserService
    {
        private readonly UsuarioRepository _usuarioRepository;

        public CustomUserService(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario?> GetByUsernameAsync(string username)
        {
            Console.WriteLine($"Intentando login con username: {username}");

           
            var usuario = await _usuarioRepository.GetByUsernameAsync(username);

            Console.WriteLine($"Usuario encontrado en DB: {usuario}");

            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            var role = usuario.Rol switch
            {
                Roles.ADMINISTRADOR => Roles.ADMINISTRADOR,
                Roles.RECEPCIONISTA => Roles.RECEPCIONISTA,
                Roles.CAJERO => Roles.CAJERO,
                _ => throw new Exception("Rol desconocido")
            };

            Console.WriteLine($"Rol asignado: {role}");

            return usuario;
        }
    }
}
