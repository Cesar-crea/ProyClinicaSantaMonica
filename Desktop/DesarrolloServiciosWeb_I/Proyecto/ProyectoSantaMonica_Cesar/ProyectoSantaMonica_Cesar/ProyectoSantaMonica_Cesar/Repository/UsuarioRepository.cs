using Microsoft.Data.SqlClient;
using ProyectoSantaMonica_Cesar.Models;
using System.Data;
using ProyectoSantaMonica_Cesar.Security;

namespace ProyectoSantaMonica_Cesar.Repository
{
    public class UsuarioRepository
    {
        private readonly string cadena;

        public UsuarioRepository(IConfiguration config)
        {
            cadena = config.GetConnectionString("CibertecConnection");
        }

        public string Insertar(Usuario u)
        {
       
            try
            {
                using SqlConnection cn = new SqlConnection(cadena);
                using SqlCommand cmd = new SqlCommand("sp_InsertarUsuario", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Username", u.Username);
                cmd.Parameters.AddWithValue("@Contrasenia", u.Contrasenia);
                cmd.Parameters.AddWithValue("@Nombres", u.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", u.Apellidos);
                cmd.Parameters.AddWithValue("@Dni", u.Dni);
                cmd.Parameters.AddWithValue("@Telefono", (object?)u.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Img_Perfil", (object?)u.Img_Perfil ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Correo", (object?)u.Correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Rol", u.Rol.ToString());

                cn.Open();
                cmd.ExecuteNonQuery();
                return "OK";
            }
            catch (SqlException ex) {

                return ex.Message;
            }
        }

        public Usuario BuscarPorUsername(string username)
        {
            Usuario u = null;

            using SqlConnection cn = new SqlConnection(cadena);
            using SqlCommand cmd = new SqlCommand("sp_BuscarUsuarioPorUsername", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Username", username);

            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                u = Mapear(dr);
            }

            return u;
        }

        public Usuario BuscarPorId(long id)
        {
            Usuario u = null;

            using SqlConnection cn = new SqlConnection(cadena);
            using SqlCommand cmd = new SqlCommand("sp_BuscarUsuarioPorId", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            cn.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                u = Mapear(dr);
            }

            return u;
        }

        public void Actualizar(Usuario u)
        {
            using SqlConnection cn = new SqlConnection(cadena);
            using SqlCommand cmd = new SqlCommand("sp_ActualizarUsuario", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id_Usuario", u.Id_Usuario);
            cmd.Parameters.AddWithValue("@Username", u.Username);
            cmd.Parameters.AddWithValue("@Contrasenia", u.Contrasenia);
            cmd.Parameters.AddWithValue("@Nombres", u.Nombres);
            cmd.Parameters.AddWithValue("@Apellidos", u.Apellidos);
            cmd.Parameters.AddWithValue("@Dni", u.Dni);
            cmd.Parameters.AddWithValue("@Telefono", (object?)u.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Img_Perfil", (object?)u.Img_Perfil ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Correo", (object?)u.Correo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Rol", u.Rol.ToString());

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        private Usuario Mapear(SqlDataReader dr)
        {
            return new Usuario
            {
                Id_Usuario = (long)dr["Id_Usuario"],
                Username = dr["Username"].ToString(),
                Contrasenia = dr["Contrasenia"].ToString(),
                Nombres = dr["Nombres"].ToString(),
                Apellidos = dr["Apellidos"].ToString(),
                Dni = dr["Dni"].ToString(),
                Telefono = dr["Telefono"]?.ToString(),
                Img_Perfil = dr["Img_Perfil"]?.ToString(),
                Correo = dr["Correo"]?.ToString(),
                Rol = Enum.Parse<Roles>(dr["Rol"].ToString())
            };
        }

        public async Task<Usuario?> GetByUsernameAsync(string username)
        {
            Usuario? u = null;

            using SqlConnection cn = new SqlConnection(cadena);
            using SqlCommand cmd = new SqlCommand("sp_BuscarUsuarioPorUsername", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Username", username);

            await cn.OpenAsync();

            using SqlDataReader dr = await cmd.ExecuteReaderAsync();

            if (await dr.ReadAsync())
            {
                u = Mapear(dr);
            }

            return u;
        }

        public Usuario BuscarPorNombre(string nombre)
        {
            Usuario u = null;

            using SqlConnection cn = new SqlConnection(cadena);
            using SqlCommand cmd = new SqlCommand("sp_BuscarUsuarioPorNombre", cn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Nombre", nombre);

            cn.Open();

            using SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                u = new Usuario
                {
                    Id_Usuario = Convert.ToInt64(dr["Id_Usuario"]),
                    Username = dr["Username"].ToString(),
                    Nombres = dr["Nombres"].ToString(),
                    Apellidos = dr["Apellidos"].ToString(),
                    Dni = dr["Dni"].ToString(),
                    Telefono = dr["Telefono"]?.ToString(),
                    Correo = dr["Correo"]?.ToString(),
                    Img_Perfil = dr["Img_Perfil"]?.ToString(),
                    Rol = Enum.Parse<Roles>(dr["Rol"].ToString())
                };
            }

            return u;
        }

    }
}