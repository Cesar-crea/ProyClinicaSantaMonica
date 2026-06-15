using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using ProyectoSantaMonica_Cesar.Data;
using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Repository;
using ProyectoSantaMonica_Cesar.Service.MedicoService;
using ProyectoSantaMonica_Cesar.Service.UsuarioService;
using ProyectoSantaMonica_Cesar.Service.EspecialidadService;
using ProyectoSantaMonica_Cesar.Service.HorarioService;
using ProyectoSantaMonica_Cesar.Service.PacienteService;
using ProyectoSantaMonica_Cesar.Service.CitaService;
using ProyectoSantaMonica_Cesar.Service.ComprobanteDePagoService;


var builder = WebApplication.CreateBuilder(args);

/// AUTENTICACIÓN
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuario/Login";
        options.LogoutPath = "/Usuario/Logout";
        options.AccessDeniedPath = "/Usuario/AccesoDenegado";
    });

///  AUTORIZACIÓN
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ADMINISTRADOR", p => p.RequireRole("ADMINISTRADOR"));
    options.AddPolicy("RECEPCIONISTA", p => p.RequireRole("RECEPCIONISTA"));
    options.AddPolicy("CAJERO", p => p.RequireRole("CAJERO"));
});

///  DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CibertecConnection")));

/// REPOSITORIOS 
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<EspecialidadRepository>();
builder.Services.AddScoped<PacienteRepository>();
builder.Services.AddScoped<MedicoRepository>();
builder.Services.AddScoped<HorarioRepository>();
builder.Services.AddScoped<CitaRepository>();
builder.Services.AddScoped<ComprobanteDePagoRepository>();

///  SERVICIOS 
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IMedicoService,MedicoService>();
builder.Services.AddScoped<IEspecialidadService,EspecialidadService>();
builder.Services.AddScoped<IHorarioService, HorarioServiceImpl>();
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<ICitaService, CitaService>();
builder.Services.AddScoped<IComprobanteDePagoService, ComprobanteDePagoService>();

///  MVC
builder.Services.AddControllersWithViews();

///  SESSION
builder.Services.AddSession();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<UsuarioRepository>();
    var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<ProyectoSantaMonica_Cesar.Models.Usuario>();

    var existe = repo.BuscarPorUsername("admin");

    if (existe == null)
    {
        var admin = new Usuario
        {
            Username = "admin1",
            Nombres = "Juan",
            Apellidos = "Perez Torres",
            Dni = "45852020",
            Telefono = "014578921",
            Correo = "juanpt@admin.com",
            Img_Perfil = "/images/default-user.jpg",
            Rol = Roles.ADMINISTRADOR
        };

        // 🔥 IMPORTANTE
        admin.Contrasenia = hasher.HashPassword(admin, "1234");

        repo.Insertar(admin);
    }
}


///  PIPELINE
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

///  STATIC FILES
app.UseStaticFiles();

///  UPLOADS
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuario}/{action=Login}/{id?}");

app.Run();