using Microsoft.Data.SqlClient;
using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Models.enums;
using System.Data;

namespace ProyectoSantaMonica_Cesar.Repository
{
    public class HorarioRepository
    {
        private readonly string _connectionString;

        public HorarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CibertecConnection");
        }

        //  Mapper
        private Horario Mapear(SqlDataReader dr)
        {
            return new Horario
            {
                Id_Horario = dr.GetInt32(0),
                Id_Medico = dr.GetInt32(1),
                Dia_Semana = Enum.Parse<DiaSemana>(dr.GetString(2)),
                Horario_Entrada = dr.GetTimeSpan(3),
                Horario_Salida = dr.GetTimeSpan(4)
            };
        }

        //  Listar por médico
        public async Task<List<Horario>> ListarPorMedico(int idMedico)
        {
            var lista = new List<Horario>();

            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_listarHorariosPorMedico1", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@idMedico", SqlDbType.Int).Value = idMedico;

            await cn.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
                lista.Add(Mapear(dr));

            return lista;
        }

        //  Listar por médico + día
        public async Task<List<Horario>> ListarPorMedicoDia(int idMedico, DiaSemana dia)
        {
            var lista = new List<Horario>();

            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_listarHorariosPorMedicoDia1", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id_Medico", idMedico);
            cmd.Parameters.AddWithValue("@Dia_Semana", dia.ToString());

            await cn.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();

            while (await dr.ReadAsync())
                lista.Add(Mapear(dr));

            return lista;
        }

        //  Registrar
        public async Task<string> Registrar(Horario h)
        {
            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_insertarHorario1", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id_Medico", h.Id_Medico);
            cmd.Parameters.AddWithValue("@Dia_Semana", h.Dia_Semana.ToString());
            cmd.Parameters.AddWithValue("@Horario_Entrada", h.Horario_Entrada);
            cmd.Parameters.AddWithValue("@Horario_Salida", h.Horario_Salida);

            await cn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString();
        }

        //  Actualizar
        public async Task<string> Actualizar(Horario h)
        {
            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_actualizarHorario1", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id_Horario", h.Id_Horario);
            cmd.Parameters.AddWithValue("@Id_Medico", h.Id_Medico);
            cmd.Parameters.AddWithValue("@Dia_Semana", h.Dia_Semana.ToString());
            cmd.Parameters.AddWithValue("@Horario_Entrada", h.Horario_Entrada);
            cmd.Parameters.AddWithValue("@Horario_Salida", h.Horario_Salida);

            await cn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString();
        }

        //  Eliminar
        public async Task<string> Eliminar(int id)
        {
            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_eliminarHorario1", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id_Horario", id);

            await cn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return result?.ToString();
        }

        //Obtener por Id

        // 🔹 Obtener por ID
        public async Task<Horario?> ObtenerPorId(int id)
        {
            using var cn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_obtenerHorarioPorId1", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id_Horario", id);

            await cn.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();

            if (await dr.ReadAsync())
                return Mapear(dr);

            return null;
        }

      

    }
}
