using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "WeatherForecast API", 
        Version = "v1",
        Description = "A simple example ASP.NET Core Web API",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "John Doe",
            Email = string.Empty,
            Url = new Uri("https://twitter.com/johndoe"),
        },
        License = new OpenApiLicense
        {
            Name = "Use under MIT",
            Url = new Uri("https://example.com/license"),
        }
    });
   
    var filePath = Path.Combine(System.AppContext.BaseDirectory, "RestAPIExample.xml");
    c.IncludeXmlComments(filePath);
});

var app = builder.Build();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.InjectStylesheet("/swagger-ui/custom.css");
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

