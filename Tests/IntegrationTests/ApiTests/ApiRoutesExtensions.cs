using System.Text;

using Newtonsoft.Json;

namespace IntegrationTests.ApiTests;

public static class ApiRoutesExtensions
{
    #region Authentication

    public static async Task<(HttpStatusCode Code, string Response)> RegisterUser(this HttpClient client,
        object model)
    {
        const string route = "auth/register";

        var response = await client.PostAsync(route,
            new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }

    public static async Task<(HttpStatusCode Code, string Token)> LoginUser(this HttpClient client,
        object model)
    {
        const string route = "auth/login";

        var response = await client.PostAsync(route,
            new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }

    public static async Task<HttpStatusCode> IsCorrectUser(this HttpClient client)
    {
        const string route = "auth/is-correct";

        var response = await client.GetAsync(route);

        return response.StatusCode;
    }

    #endregion

    #region Events

    public static async Task<(HttpStatusCode Code, string response)> CreateEvent(this HttpClient client,
        object model)
    {
        const string route = "events";

        var response = await client.PostAsync(route,
            new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }

    public static async Task<(HttpStatusCode Code, string Response)> GetEventById(this HttpClient client, Guid eventId)
    {
        var route = $"events/{eventId}";

        var response = await client.GetAsync(route);

        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }

    public static async Task<(HttpStatusCode Code, string Response)> GetEventForUser(this HttpClient client,
        bool isOwner = false)
    {
        var ownerQuery = isOwner ? "?owner=true" : "";
        var route = $"events{ownerQuery}";

        var response = await client.GetAsync(route);

        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }

    public static async Task<(HttpStatusCode Code, string Response)> DeleteEvent(this HttpClient client, Guid eventId)
    {
        var route = $"events/{eventId}";

        var response = await client.DeleteAsync(route);

        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }
    
    public static async Task<(HttpStatusCode Code, string Response)> CancelEvent(this HttpClient client, Guid eventId)
    {
        var route = $"events/{eventId}/cancel";

        var response = await client.PatchAsync(route, null);

        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }

    #endregion
}