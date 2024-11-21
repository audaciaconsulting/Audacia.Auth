using System;
using Audacia.Auth.OpenIddict.Common.Configuration;
using Audacia.Auth.OpenIddict.QuartzCleanup;
using Audacia.Auth.OpenIddict.QuartzCleanup.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using OpenIddict.Quartz;
using Xunit;

namespace Audacia.Auth.OpenIddict.Tests.QuartzCleanup;

public class ServiceCollectionExtensionsTests
{
    private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;
    private readonly OpenIdConnectCleanupConfig _openIdConnectConfig;

    public ServiceCollectionExtensionsTests()
    {
        _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
        _webHostEnvironmentMock.SetupGet(e => e.EnvironmentName).Returns(Environments.Development);

        _openIdConnectConfig = new OpenIdConnectCleanupConfig
        {
            SigningCertificateThumbprint = "abc",
            EncryptionCertificateThumbprint = "xyz"
        };
    }

    [Fact]
    public void Adding_OpenIddict_with_cleanup_uses_6_hour_token_lifespan_default()
    {
        var services = new ServiceCollection();
        services.AddOpenIddictWithCleanup<DummyUser, string>(
            _ => { },
            user => user.Id,
            _openIdConnectConfig as OpenIdConnectConfig,
            _webHostEnvironmentMock.Object);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictQuartzOptions>>();

        options.CurrentValue.MinimumTokenLifespan.Should().Be(TimeSpan.FromHours(6));
    }

    [Fact]
    public void Adding_OpenIddict_with_cleanup_uses_6_hour_authorization_lifespan_default()
    {
        var services = new ServiceCollection();
        services.AddOpenIddictWithCleanup<DummyUser, string>(
            _ => { },
            user => user.Id,
            _openIdConnectConfig as OpenIdConnectConfig,
            _webHostEnvironmentMock.Object);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictQuartzOptions>>();

        options.CurrentValue.MinimumAuthorizationLifespan.Should().Be(TimeSpan.FromHours(6));
    }

    [Fact]
    public void Adding_OpenIddict_with_cleanup_uses_configured_token_lifespan_if_set()
    {
        _openIdConnectConfig.MinimumAgeToCleanup = new ConfigurableTimespan
        {
            Type = ConfigurableTimespanType.Hours,
            Value = 3
        };
        var services = new ServiceCollection();
        services.AddOpenIddictWithCleanup<DummyUser, string>(
            _ => { },
            user => user.Id,
            _openIdConnectConfig,
            _webHostEnvironmentMock.Object);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictQuartzOptions>>();

        options.CurrentValue.MinimumTokenLifespan.Should().Be(TimeSpan.FromHours(3));
    }

    [Fact]
    public void Adding_OpenIddict_with_cleanup_uses_configured_authorization_lifespan_if_set()
    {
        _openIdConnectConfig.MinimumAgeToCleanup = new ConfigurableTimespan
        {
            Type = ConfigurableTimespanType.Hours,
            Value = 3
        };
        var services = new ServiceCollection();
        services.AddOpenIddictWithCleanup<DummyUser, string>(
            _ => { },
            user => user.Id,
            _openIdConnectConfig,
            _webHostEnvironmentMock.Object);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictQuartzOptions>>();

        options.CurrentValue.MinimumTokenLifespan.Should().Be(TimeSpan.FromHours(3));
    }

    [Fact]
    public void Adding_OpenIddict_with_cleanup_uses_default_token_lifespan_if_not_set()
    {
        var services = new ServiceCollection();
        services.AddOpenIddictWithCleanup<DummyUser, string>(
            _ => { },
            user => user.Id,
            _openIdConnectConfig,
            _webHostEnvironmentMock.Object);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictQuartzOptions>>();

        options.CurrentValue.MinimumTokenLifespan.Should().Be(TimeSpan.FromHours(6));
    }

    [Fact]
    public void Adding_OpenIddict_with_cleanup_uses_default_authorization_lifespan_if_not_set()
    {
        var services = new ServiceCollection();
        services.AddOpenIddictWithCleanup<DummyUser, string>(
            _ => { },
            user => user.Id,
            _openIdConnectConfig,
            _webHostEnvironmentMock.Object);

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictQuartzOptions>>();

        options.CurrentValue.MinimumTokenLifespan.Should().Be(TimeSpan.FromHours(6));
    }
}

public class DummyUser
{
    public string Id { get; set; } = string.Empty;
}
