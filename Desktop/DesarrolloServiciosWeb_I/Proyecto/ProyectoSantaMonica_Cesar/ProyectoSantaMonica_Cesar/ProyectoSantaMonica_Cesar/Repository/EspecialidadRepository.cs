using Microsoft.Data.SqlClient;
using ProyectoSantaMonica_Cesar.Models;
using System.Data;

namespace ProyectoSantaMonica_Cesar.Repository
{
    public class EspecialidadRepository
    {
        private readonly string _connection;

        public EspecialidadRepository(IConfiguration config)
        {
            _connection = config.GetConnectionString("CibertecConnection");
        }

        private Especialidad Mapear(SqlDataReader dr)
        {
            return new Especialidad
            {
                Id_Especialidad = dr.GetInt32(0),
                Nombre = dr.GetString(1)
            };
        }

        public async Task<List<Especialidad>> Listar()
        {
            var lista = new List<Especialidad>();

            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_listarEspecialidades", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            await cn.OpenAsync();
            var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
                lista.Add(Mapear(dr));

            return lista;
        }

        public async Task<Especialidad?> ObtenerPorId(int id)
        {
            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_obtenerEspecialidadPorId", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id_Especialidad", id);

            await cn.OpenAsync();
            var dr = await cmd.ExecuteReaderAsync();

            if (await dr.ReadAsync())
                return Mapear(dr);

            return null;
        }

        public async Task<string> Registrar(Especialidad e)
        {
            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_insertarEspecialidad", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Nombre", e.Nombre);

            await cn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString();
        }

        public async Task<string> Actualizar(Especialidad e)
        {
            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_actualizarEspecialidad", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id_Especialidad", e.Id_Especialidad);
            cmd.Parameters.AddWithValue("@Nombre", e.Nombre);

            await cn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString();
        }

        public async Task<string> Eliminar(int id)
        {
            using var cn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_eliminarEspecialidad", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id_Especialidad", id);

            await cn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString();
        }
    }
}