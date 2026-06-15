using Microsoft.AspNetCore.Identity;
using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Repository;

namespace ProyectoSantaMonica_Cesar.Service.UsuarioService
{
    public class UsuarioService :IUsuarioService
    {
        private readonly UsuarioRepository repo;
        private readonly PasswordHasher<Usuario> hasher;

        public UsuarioService(UsuarioRepository repo)
        {
            this.repo = repo;
            this.hasher = new PasswordHasher<Usuario>();
        }

        public string RegistrarUsuario(Usuario u)
        {
            u.Contrasenia = hasher.HashPassword(u, u.Contrasenia);
            return repo.Insertar(u);
        }

        public void ActualizarUsuario(Usuario nuevo, Usuario actual)
        {
            var existente = repo.BuscarPorId(nuevo.Id_Usuario);

            if (existente == null)
                throw new Exception("Usuario no encontrado");

            existente.Username = nuevo.Username;
            existente.Nombres = nuevo.Nombres;
            existente.Apellidos = nuevo.Apellidos;
            existente.Dni = nuevo.Dni;
            existente.Telefono = nuevo.Telefono;
            existente.Correo = nuevo.Correo;
            existente.Img_Perfil = nuevo.Img_Perfil;

            if (nuevo.Id_Usuario != actual.Id_Usuario)
                existente.Rol = nuevo.Rol;

            if (!string.IsNullOrEmpty(nuevo.Contrasenia))
            {
                existente.Contrasenia = hasher.HashPassword(existente, nuevo.Contrasenia);
            }

            repo.Actualizar(existente);
        }

        public Usuario BuscarPorNombre(string nombre)
        {
            var u = repo.BuscarPorNombre(nombre);
            if (u == null) throw new Exception("Usuario no encontrado");
            return u;
        }

        public Usuario BuscarPorID(long id)
        {
            var u = repo.BuscarPorId(id);
            if (u == null) throw new Exception("Usuario no encontrado");
            return u;
        }

        public Usuario BuscarPorUserName(string username)
        {
            var u = repo.BuscarPorUsername(username);

            if (u == null)
                throw new Exception("Usuario no encontrado");

            return u;
        }

        public Usuario? Login(string username, string password)
        {
            var usuario = repo.BuscarPorUsername(username);

            if (usuario == null)
                return null;

            var resultado = hasher.VerifyHashedPassword(
                usuario,
                usuario.Contrasenia,
                password
            );

            if (resultado == PasswordVerificationResult.Success)
                return usuario;

            return null;
        }
    }
}
