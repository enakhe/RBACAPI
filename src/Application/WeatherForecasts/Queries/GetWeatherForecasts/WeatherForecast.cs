<<<<<<< HEAD
﻿namespace RBACAPI.Application.WeatherForecasts.Queries.GetWeatherForecasts;
=======
﻿namespace EcommerceAPI.Application.WeatherForecasts.Queries.GetWeatherForecasts;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public class WeatherForecast
{
    public DateTime Date { get; init; }

    public int TemperatureC { get; init; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; init; }
}
