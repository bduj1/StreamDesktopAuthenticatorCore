﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SteamAuthCore.Abstractions;
using SteamAuthCore.Extensions;
using SteamAuthCore.Models.Internal;

namespace SteamAuthCore.Services;

internal sealed class SteamApi : ISteamApi
{
    public SteamApi(HttpClient client)
    {
        _client = client;

        _client.BaseAddress = new Uri(ApiEndpoints.SteamApiBase);
        _client.AddDefaultHeaders();
    }

    private readonly HttpClient _client;

    public async ValueTask<string> GetSteamTime()
    {
        using var responseMessage = await _client.PostAsJsonAsync(ApiEndpoints.TwoFactorTimeQuery, "steamid=0");
        responseMessage.EnsureSuccessStatusCode();
        var timeQuery = (await responseMessage.Content.ReadFromJsonAsync<TimeQuery>())!;
        
        return timeQuery.Response.ServerTime;
    }

    public async ValueTask<RefreshSessionDataInternalResponse?> MobileauthGetwgtoken(string token)
    {
        var pair = new KeyValuePair<string, string>("access_token", token);

        var message = new HttpRequestMessage(HttpMethod.Post, ApiEndpoints.MobileauthGetwgtoken);
        message.Content = new FormUrlEncodedContent(new[] {pair});

        var responseMessage = await _client.SendAsync(message);
        responseMessage.EnsureSuccessStatusCode();

        var response = await responseMessage.Content.ReadFromJsonAsync<RefreshSessionDataResponse>();
        return response!.Response;
    }
}
