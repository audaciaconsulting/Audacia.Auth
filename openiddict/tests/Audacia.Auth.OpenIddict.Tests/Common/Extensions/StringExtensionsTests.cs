using System;
using System.Security.Cryptography.X509Certificates;
using Audacia.Auth.OpenIddict.Common.Extensions;
using Audacia.Auth.OpenIddict.DependencyInjection;
using FluentAssertions;
using Xunit;

namespace Audacia.Auth.OpenIddict.Tests.Common.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null, StoreLocation.CurrentUser)]
    [InlineData("", StoreLocation.CurrentUser)]
    [InlineData("CurrentUser", StoreLocation.CurrentUser)]
    [InlineData("LocalMachine", StoreLocation.LocalMachine)]
    public void Raw_strings_parsed_into_correct_store_locations(string? valuetoParse, StoreLocation expectedResult)
    {
        var actualResult = valuetoParse.ParseStoreLocation();

        actualResult.Should().Be(expectedResult);
    }

    [Fact]
    public void Invalid_store_location_string_throws_exception()
    {
        Action action = () => "not valid".ParseStoreLocation();

        action.Should().ThrowExactly<OpenIddictConfigurationException>();
    }
}
