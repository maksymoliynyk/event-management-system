﻿using System.Text;

using API.Models;

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

    public static async Task<(HttpStatusCode Code, string Response)> InviteUser(this HttpClient client, Guid eventId,
        string inviteeEmail)
    {
        var route = $"events/{eventId}/invites";
        var model = new SendRSVPRequestModel(inviteeEmail);

        var response = await client.PostAsync(route,
            new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }
    
    public static async Task<(HttpStatusCode Code, string Response)> GetRsvpsForEvent(this HttpClient client, Guid eventId)
    {
        var route = $"events/{eventId}/invites";

        var response = await client.GetAsync(route);

        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }
    
    public static async Task<(HttpStatusCode Code, string Response)> GetAttendeesForEvent(this HttpClient client, Guid eventId)
    {
        var route = $"events/{eventId}/attendees";

        var response = await client.GetAsync(route);

        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }
    
    public static async Task<(HttpStatusCode Code, string Response)> RespondToRSVP(this HttpClient client, Guid eventId, bool isAccepted)
    {
        var route = $"events/{eventId}/invites";
        var model = new RespondToRSVPRequestModel(isAccepted);

        var response = await client.PatchAsync(route,
            new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }
    
    public static async Task<(HttpStatusCode Code, string Response)> GetRsvpsForUser(this HttpClient client)
    {
        const string route = "invites";

        var response = await client.GetAsync(route);

        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }

    #endregion
}