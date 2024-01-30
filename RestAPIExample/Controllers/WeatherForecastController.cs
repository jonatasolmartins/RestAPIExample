using Microsoft.AspNetCore.Mvc;

namespace RestAPIExample.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    private WeatherForecast[] Summaries;

    public WeatherForecastController()
    {
        Summaries = new[]
        {
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 25, "Hot", 1),
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 15, "Cold", 2),
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 5, "Freezing", 3),
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 35, "Scorching", 4),
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 45, "Burning", 5),
        };
    }
    [HttpGet]
    public IActionResult GetWeatherForecast()
    {
        return Ok(Summaries);
    }
    
    [HttpPatch("{id:int}/{summary}")]
    public IActionResult PatchWeatherForecast(int id, string summary, [FromBody] PatchModel patch)
    {
        var forecast = Summaries.FirstOrDefault(s => s.Summary?.Equals(summary, StringComparison.OrdinalIgnoreCase) == true);

        if (forecast == null)
        {
            return NotFound();
        }

        //forecast.TemperatureC = 12;
        forecast = forecast with { TemperatureC = patch.TemperatureC };

        return NoContent();
    }
    
    [HttpPut("{id}")]
    public IActionResult PutWeatherForecast(int id, [FromBody] PutModel putModel)
    {
        var forecast = Summaries.FirstOrDefault(f => f.id == id);

        if (forecast == null)
        {
            return NotFound();
        }

        var index = Array.IndexOf(Summaries, forecast);
        if (index != -1)
        {
            Summaries[index] = putModel.WeatherForecast;
        }

        return NoContent();
    }
    
    [HttpPost]
    public IActionResult PostWeatherForecast([FromBody] PostModel postModel)
    {
        var newForecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), postModel.TemperatureC, postModel.Summary, Summaries.Length + 1);
        Array.Resize(ref Summaries, Summaries.Length + 1);
        Summaries[Summaries.Length - 1] = newForecast;

        return CreatedAtAction(nameof(GetWeatherForecast), new { id = newForecast.id }, newForecast);
    }
    
    [HttpHead]
    public IActionResult HeadWeatherForecast()
    {
        if (Summaries == null || Summaries.Length == 0)
        {
            return NotFound();
        }

        Response.Headers.Add("X-Total-Count", Summaries.Length.ToString());
        return NoContent();
    }
}


public class PatchModel
{
    public int TemperatureC { get; set; }
}

public class PutModel
{
    public WeatherForecast WeatherForecast { get; set; }
}

public class PostModel
{
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, int id)
{
    public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
}