using Audacia.Auth.OpenIddict.Common.Configuration;
using FluentAssertions;
using Xunit;

namespace Audacia.Auth.OpenIddict.Tests;

public class OpenIdConnectConfigTests
{
    private readonly OpenIdConnectConfig _config = new OpenIdConnectConfig();

    [Fact]
    {
        const string clientId = "ui-client";
        _config.AuthorizationCodeClients = new[]
        {
            new AuthorizationCodeClient { ClientId = clientId }
        };

        var allClients = _config.AllClients;

        allClients.Should().Contain(client => client.ClientId == clientId);
    }

    [Fact]
    public void All_clients_contains_resource_owner_password_clients()
    {
        const string clientId = "automation-client";
        _config.ResourceOwnerPasswordClients = new[]
        {
            new ResourceOwnerPasswordClient { ClientId = clientId }
        };

        var allClients = _config.AllClients;

        allClients.Should().Contain(client => client.ClientId == clientId);
    }
}