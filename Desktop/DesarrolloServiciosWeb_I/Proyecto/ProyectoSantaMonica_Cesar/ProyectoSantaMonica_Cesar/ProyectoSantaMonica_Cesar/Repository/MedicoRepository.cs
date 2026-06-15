using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Models.enums;
using System.Data;

namespace ProyectoSantaMonica_Cesar.Repository
{
    public class MedicoRepository
    {
        private readonly string _connectionString;

        public MedicoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CibertecConnection");
        }

        public async Task<List<Medico>> ListarAsync()
        {
            var lista = new List<Medico>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                SqlCommand cmd = new SqlCommand("sp_ListarMedicos", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader dr = await cmd.ExecuteReaderAsync();

                while (await dr.ReadAsync())
                {
                    lista.Add(new Medico
                    {
                        Id_Medico = (int)dr["Id_Medico"],
                        Nombres = dr["Nombres"].ToString(),
                        Apellidos = dr["Apellidos"].ToString(),
                        Dni = dr["Dni"].ToString(),
                        Nro_Colegiatura = dr["Nro_Colegiatura"].ToString(),
                        Telefono = dr["Telefono"].ToString(),
                        Especialidad = dr["Especialidad"].ToString()
                    });
                }
            }
            return lista;
        }

        public async Task InsertarAsync(Medico m)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            SqlCommand cmd = new SqlCommand("sp_InsertarMedico", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Nombres", m.Nombres);
            cmd.Parameters.AddWithValue("@Apellidos", m.Apellidos);
            cmd.Parameters.AddWithValue("@Dni", m.Dni);
            cmd.Parameters.AddWithValue("@Nro_Colegiatura", m.Nro_Colegiatura);
            cmd.Parameters.AddWithValue("@Telefono", (object?)m.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id_Especialidad", m.Id_Especialidad);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task ActualizarAsync(Medico m)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            SqlCommand cmd = new SqlCommand("sp_ActualizarMedico", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id_Medico", m.Id_Medico);
            cmd.Parameters.AddWithValue("@Nombres", m.Nombres);
            cmd.Parameters.AddWithValue("@Apellidos", m.Apellidos);
            cmd.Parameters.AddWithValue("@Dni", m.Dni);
            cmd.Parameters.AddWithValue("@Nro_Colegiatura", m.Nro_Colegiatura);
            cmd.Parameters.AddWithValue("@Telefono", (object?)m.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id_Especialidad", m.Id_Especialidad);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsync(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            SqlCommand cmd = new SqlCommand("sp_EliminarMedico", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id_Medico", id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<Medico?> BuscarPorIdAsync(int id)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            SqlCommand cmd = new SqlCommand("sp_BuscarMedicoPorId", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id_Medico", id);

            SqlDataReader dr = await cmd.ExecuteReaderAsync();

            if (await dr.ReadAsync())
            {
                return new Medico
                {
                    Id_Medico = (int)dr["Id_Medico"],
                    Nombres = dr["Nombres"].ToString(),
                    Apellidos = dr["Apellidos"].ToString(),
                    Dni = dr["Dni"].ToString(),
                    Nro_Colegiatura = dr["Nro_Colegiatura"].ToString(),
                    Telefono = dr["Telefono"].ToString(),
                    Id_Especialidad = (int)dr["Id_Especialidad"],
                    Especialidad = dr["Especialidad"].ToString()
                };
            }
            return null;
        }

        public async Task<List<Medico>> BuscarPorNombreAsync(string nombre)
        {
            var lista = new List<Medico>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            SqlCommand cmd = new SqlCommand("sp_BuscarMedicoPorNombre", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Nombre", nombre);

            SqlDataReader dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                lista.Add(new Medico
                {
                    Id_Medico = (int)dr["Id_Medico"],
                    Nombres = dr["Nombres"].ToString(),
                    Apellidos = dr["Apellidos"].ToString(),
                    Especialidad = dr["Especialidad"].ToString()
                });
            }

            return lista;
        }

        public async Task<List<Horario>> ObtenerHorariosPorMedicoAsync(int idMedico)
        {
            var lista = new List<Horario>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            SqlCommand cmd = new SqlCommand("sp_ListarHorariosPorMedico1", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@idMedico", idMedico);

            SqlDataReader dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                lista.Add(new Horario
                {
                    Dia_Semana = Enum.Parse<DiaSemana>(dr["Dia_Semana"].ToString()),
                    Horario_Entrada = dr["Horario_Entrada"] != DBNull.Value
                        ? TimeSpan.Parse(dr["Horario_Entrada"].ToString())
                        : null,
                    Horario_Salida = dr["Horario_Salida"] != DBNull.Value
                        ? TimeSpan.Parse(dr["Horario_Salida"].ToString())
                        : null
                });
            }

            return lista;
        }

        public async Task<List<Medico>> BuscarGeneralAsync(string texto)
        {
            var lista = new List<Medico>();

            using var cn = new SqlConnection(_connectionString);

            using var cmd = new SqlCommand("sp_BuscarMedicoGeneral", cn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Texto", texto);

            await cn.OpenAsync();

            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                lista.Add(new Medico
                {
                    Id_Medico = Convert.ToInt32(dr["Id_Medico"]),
                    Nombres = dr["Nombres"].ToString(),
                    Apellidos = dr["Apellidos"].ToString(),
                    Especialidad = dr["Especialidad"].ToString(),
                    Dni = dr["Dni"].ToString(),
                    Nro_Colegiatura = dr["Nro_Colegiatura"].ToString(),
                    Telefono = dr["Telefono"].ToString()
                });
            }

            return lista;
        }


    }
}