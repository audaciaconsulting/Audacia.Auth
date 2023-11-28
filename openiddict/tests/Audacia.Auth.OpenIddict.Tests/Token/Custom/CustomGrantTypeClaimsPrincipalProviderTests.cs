using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Common;
using Audacia.Auth.OpenIddict.Token;
using Audacia.Auth.OpenIddict.Token.Custom;
using FluentAssertions;
using Moq;
using OpenIddict.Abstractions;
using Xunit;

namespace Audacia.Auth.OpenIddict.Tests.Token.Custom;

public class CustomGrantTypeClaimsPrincipalProviderTests
{
    private readonly Mock<IProfileService<DummyUser>> _profileServiceMock;

    public CustomGrantTypeClaimsPrincipalProviderTests()
    {
        _profileServiceMock = new Mock<IProfileService<DummyUser>>();
    }

    [Fact]
    public async Task Exception_when_no_grant_type()
    {
        var request = new OpenIddictRequest();
        request.GrantType = null;
        var factory = new CustomGrantTypeClaimsPrincipalProvider<DummyUser>(Enumerable.Empty<ICustomGrantTypeValidator<DummyUser>>(), _profileServiceMock.Object);

        var action = async () => await factory.GetPrincipalAsync(request);

        (await action.Should().ThrowAsync<InvalidGrantException>())
            .WithMessage("No grant type is specified.");
    }

    [Fact]
    public async Task Exception_when_no_provider_for_grant_type()
    {
        var request = new OpenIddictRequest();
        request.GrantType = "saml";
        var factory = new CustomGrantTypeClaimsPrincipalProvider<DummyUser>(Enumerable.Empty<ICustomGrantTypeValidator<DummyUser>>(), _profileServiceMock.Object);

        var action = async () => await factory.GetPrincipalAsync(request);

        (await action.Should().ThrowAsync<InvalidGrantException>())
            .WithMessage("The grant type 'saml' is not supported.");
    }

    [Fact]
    public async Task Correct_provider_used_when_matching_provider_for_grant_type()
    {
        var request = new OpenIddictRequest();
        request.GrantType = "saml";

        var expectedPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        var mockProvider = new Mock<ICustomGrantTypeValidator<DummyUser>>();
        mockProvider.SetupGet(provider => provider.GrantType).Returns(request.GrantType);
        mockProvider.Setup(provider => provider.ValidateAsync(request)).ReturnsAsync(new CustomGrantTypeValidationResponse<DummyUser>(expectedPrincipal, new DummyUser()));
        var factory = new CustomGrantTypeClaimsPrincipalProvider<DummyUser>(new[] { mockProvider.Object }, _profileServiceMock.Object);

        var actualPrincipal = await factory.GetPrincipalAsync(request);

        actualPrincipal.Should().Be(expectedPrincipal);
    }

    public class DummyUser
    {
    }
}
