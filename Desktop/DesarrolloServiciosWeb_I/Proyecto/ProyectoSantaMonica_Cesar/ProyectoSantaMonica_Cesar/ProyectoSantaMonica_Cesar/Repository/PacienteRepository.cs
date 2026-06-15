using Microsoft.Data.SqlClient;
using ProyectoSantaMonica_Cesar.Models;
using System.Data;

namespace ProyectoSantaMonica_Cesar.Repository
{
    public class PacienteRepository
    {
        private readonly string _connectionString;

        public PacienteRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("CibertecConnection");
        }

        public List<Paciente> Listar()
        {
            var lista = new List<Paciente>();

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ListarPacientes", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Paciente
                    {
                        Id_Paciente = (int)dr["Id_Paciente"],
                        Nombres = dr["Nombres"].ToString(),
                        Apellidos = dr["Apellidos"].ToString(),
                        Dni = dr["Dni"].ToString(),
                        Fecha_Nacimiento = dr["Fecha_Nacimiento"] as DateTime?,
                        Telefono = dr["Telefono"].ToString()
                    });
                }
            }

            return lista;
        }

        public void Insertar(Paciente p)
        {
            using SqlConnection cn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("sp_InsertarPaciente", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Nombres", p.Nombres);
            cmd.Parameters.AddWithValue("@Apellidos", p.Apellidos);
            cmd.Parameters.AddWithValue("@Dni", p.Dni);
            cmd.Parameters.AddWithValue("@Fecha_Nacimiento", (object?)p.Fecha_Nacimiento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Telefono", (object?)p.Telefono ?? DBNull.Value);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public Paciente BuscarPorId(int id)
        {
            Paciente p = null;

            using SqlConnection cn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("sp_BuscarPacientePorId", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            cn.Open();
            var dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                p = new Paciente
                {
                    Id_Paciente = (int)dr["Id_Paciente"],
                    Nombres = dr["Nombres"].ToString(),
                    Apellidos = dr["Apellidos"].ToString(),
                    Dni = dr["Dni"].ToString(),
                    Fecha_Nacimiento = dr["Fecha_Nacimiento"] as DateTime?,
                    Telefono = dr["Telefono"].ToString()
                };
            }

            return p;
        }

        public void Actualizar(Paciente p)
        {
            using SqlConnection cn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("sp_ActualizarPaciente", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", p.Id_Paciente);
            cmd.Parameters.AddWithValue("@Nombres", p.Nombres);
            cmd.Parameters.AddWithValue("@Apellidos", p.Apellidos);
            cmd.Parameters.AddWithValue("@Dni", p.Dni);
            cmd.Parameters.AddWithValue("@Fecha_Nacimiento", (object?)p.Fecha_Nacimiento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Telefono", (object?)p.Telefono ?? DBNull.Value);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using SqlConnection cn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("sp_EliminarPaciente", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            cn.Open();
            cmd.ExecuteNonQuery();
        }

        public List<Paciente> BuscarPorNombre(string nombre)
        {
            var lista = new List<Paciente>();

            using SqlConnection cn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand("sp_BuscarPacientePorNombre", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Nombre", nombre);

            cn.Open();
            var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new Paciente
                {
                    Id_Paciente = (int)dr["Id_Paciente"],
                    Nombres = dr["Nombres"].ToString(),
                    Apellidos = dr["Apellidos"].ToString(),
                    Dni = dr["Dni"].ToString(),
                    Fecha_Nacimiento = dr["Fecha_Nacimiento"] as DateTime?,
                    Telefono = dr["Telefono"].ToString()
                });
            }

            return lista;
        }

        public List<Paciente> BuscarGeneral(string texto)
        {
            List<Paciente> lista = new();

            using SqlConnection cn = new SqlConnection(_connectionString);

            SqlCommand cmd =
                new SqlCommand("sp_BuscarPacienteGeneral", cn);

            cmd.CommandType = CommandType.StoredProcedure;

            // ✅ EVITAR NULL
            cmd.Parameters.AddWithValue("@Texto", texto ?? "");

            cn.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new Paciente
                {
                    Id_Paciente = Convert.ToInt32(dr["Id_Paciente"]),
                    Nombres = dr["Nombres"].ToString(),
                    Apellidos = dr["Apellidos"].ToString(),
                    Dni = dr["Dni"].ToString(),
                    Telefono = dr["Telefono"].ToString()
                });
            }

            dr.Close();

            return lista;
        }

    }
}
