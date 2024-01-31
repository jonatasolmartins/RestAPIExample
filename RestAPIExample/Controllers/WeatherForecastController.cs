using Microsoft.AspNetCore.Mvc;

namespace RestAPIExample.Controllers;

[ApiController]
[Route("[controller]")]
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
    [ResponseCache(Duration = 60)] // Cache for 60 seconds
    public IActionResult GetWeatherForecast()
    {
        return Ok(Summaries);
    }

    [HttpGet("{id}")]
    //[ResponseCache(Duration = 60)] // Cache for 60 seconds
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)] // Disable caching
    public IActionResult GetWeatherForecast(int id)
    {
        var forecast = Summaries.FirstOrDefault(s => s.id == id);

        if (forecast == null)
        {
            return NotFound();
        }

        var resource = new WeatherForecastResource
        {
            WeatherForecast = forecast,
            Links = new List<Link>
            {
                new Link { Href = Url.Action(nameof(GetWeatherForecast), new { id = forecast.id }), Rel = "self", Method = "GET" },
                new Link { Href = Url.Action(nameof(PutWeatherForecast), new { id = forecast.id }), Rel = "update", Method = "PUT" },
                new Link { Href = Url.Action(nameof(DeleteWeatherForecast), new { id = forecast.id }), Rel = "delete", Method = "DELETE" },
                new Link { Href = Url.Action(nameof(GetCode)), Rel = "code", Method = "GET" }
            }
        };

        return Ok(resource);
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
    public IActionResult PutWeatherForecast([FromRoute]int id, [FromBody] PutModel putModel)
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
    
    [HttpDelete("{id}")]
    public IActionResult DeleteWeatherForecast(int id)
    {
        var enummm = WeatherForecastSummary.Cold;
        switch (enummm)
        {
            case WeatherForecastSummary.Burning:
                return BadRequest();
            case WeatherForecastSummary.Cold:
                return NotFound();
        }
        return NoContent();
    }
    
    [HttpGet("code")]
    public ContentResult GetCode()
    {
        var script = @"
    function calculateTemperatureF(temperatureC) {
        return 32 + (temperatureC / 0.5556);
    }";

        return Content(script, "application/javascript");
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

public enum WeatherForecastSummary
{
    Hot,
    Cold,
    Freezing,
    Scorching,
    Burning
}

public class Link
{
    public string Href { get; set; }
    public string Rel { get; set; }
    public string Method { get; set; }
}

public class WeatherForecastResource
{
    public WeatherForecast WeatherForecast { get; set; }
    public List<Link> Links { get; set; }
}