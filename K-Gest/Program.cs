using K_Gest.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Ativa o serviço de gerenciamento de autenticação por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index"; // Redireciona para cá se tentar acessar sem login
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

// Registra o serviço de segundo plano
builder.Services.AddHostedService<VerificadorValidadeService>();

var app = builder.Build();

// Configura a cultura de forma global e correta para Português do Brasil (pt-BR)
var ptBR = new CultureInfo("pt-BR");
var supportedCultures = new[] { ptBR };

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(ptBR),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

// Ordem estrita e obrigatória de segurança do ASP.NET Core
app.UseAuthentication();
app.UseAuthorization();

// MAPEAMENTO DE ROTAS: Apenas uma rota padrão iniciando no Login/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();