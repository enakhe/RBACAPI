<<<<<<< HEAD
﻿using RBACAPI.Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace RBACAPI.Web.Endpoints;
=======
﻿using EcommerceAPI.Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace EcommerceAPI.Web.Endpoints;
>>>>>>> f328d42b2352a899f713f43892f8f4a1a23a6498

public class WeatherForecasts : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetWeatherForecasts);
    }

    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecasts(ISender sender)
    {
        return await sender.Send(new GetWeatherForecastsQuery());
    }
}
