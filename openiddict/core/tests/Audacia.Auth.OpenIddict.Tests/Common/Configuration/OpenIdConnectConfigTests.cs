using Audacia.Auth.OpenIddict.Common.Configuration;
using FluentAssertions;
using Xunit;

namespace Audacia.Auth.OpenIddict.Tests
{
    public class OpenIdConnectConfigTests
    {
        private readonly OpenIdConnectConfig _config = new OpenIdConnectConfig();

        [Fact]
        public void All_clients_contains_api_clients()
        {
            const string clientId = "api-client";
            _config.ApiClients = new[]
            {
                new ApiClient { ClientId = clientId }
            };

            var allClients = _config.AllClients;

            allClients.Should().Contain(client => client.ClientId == clientId);
        }

        [Fact]
        public void All_clients_contains_ui_clients()
        {
            const string clientId = "ui-client";
            _config.UiClients = new[]
            {
                new UiClient { ClientId = clientId }
            };

            var allClients = _config.AllClients;

            allClients.Should().Contain(client => client.ClientId == clientId);
        }

        [Fact]
        public void All_clients_contains_test_automation_clients()
        {
            const string clientId = "automation-client";
            _config.TestAutomationClients = new[]
            {
                new TestAutomationClient { ClientId = clientId }
            };

            var allClients = _config.AllClients;

            allClients.Should().Contain(client => client.ClientId == clientId);
        }
    }
}