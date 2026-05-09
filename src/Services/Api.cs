/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Identity;

public static class Api
{
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(5) };

    public static bool IsConfigured => ConVars.Url.Value.Contains("{userId}");

    public static async Task<User?> FetchUserAsync(ulong steamId)
    {
        var url = ConVars.Url.Value.Replace("{userId}", steamId.ToString());
        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(jsonContent);
        }
        catch (Exception error)
        {
            Runtime.Core.Logger.LogError("GET {Url} failed: {Message}", url, error.Message);
            return null;
        }
    }
}
