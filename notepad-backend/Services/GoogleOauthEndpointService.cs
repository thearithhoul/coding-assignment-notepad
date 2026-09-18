using System.Text.Json.Serialization;

namespace notepad_backend.Services;


public class GoogleOauthEndpointService
{
    private const string DiscoveryUrl = "https://accounts.google.com/.well-known/openid-configuration";

    public Discovery Discovery { get; }

    public GoogleOauthEndpointService(IHttpClientFactory factory)
    {
        var client = factory.CreateClient();

        var response = client.GetAsync(DiscoveryUrl).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();

        Discovery = response.Content.ReadFromJsonAsync<Discovery>().GetAwaiter().GetResult()
            ?? throw new InvalidOperationException("Failed to read Google OpenID discovery document.");
    }
}


public record Discovery
{
    [JsonPropertyName("issuer")]
    public string Issuer { get; init; } = string.Empty;

    [JsonPropertyName("authorization_endpoint")]
    public string AuthorizationEndpoint { get; init; } = string.Empty;

    [JsonPropertyName("token_endpoint")]
    public string TokenEndpoint { get; init; } = string.Empty;

    [JsonPropertyName("userinfo_endpoint")]
    public string UserinfoEndpoint { get; init; } = string.Empty;

    [JsonPropertyName("revocation_endpoint")]
    public string RevocationEndpoint { get; init; } = string.Empty;

    [JsonPropertyName("jwks_uri")]
    public string JwksUri { get; init; } = string.Empty;

    [JsonPropertyName("scopes_supported")]
    public List<string> ScopesSupported { get; init; } = [];

    [JsonPropertyName("response_types_supported")]
    public List<string> ResponseTypesSupported { get; init; } = [];

    [JsonPropertyName("grant_types_supported")]
    public List<string> GrantTypesSupported { get; init; } = [];
}
