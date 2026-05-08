using System;
using Audacia.Auth.OpenIddict.Common.Configuration;
using Shouldly;
using Xunit;

namespace Audacia.Auth.OpenIddict.Tests.Common.Configuration;

public class ConfigurableTimespanTests
{
    [Fact]
    public void Lifetime_is_timespan_from_days_when_type_is_days()
    {
        const int expectedDays = 4;
        var configurableTimespan = new ConfigurableTimespan
        {
            Type = ConfigurableTimespanType.Days,
            Value = expectedDays
        };

        var lifetime = configurableTimespan.GetLifetime();

        lifetime.TotalDays.ShouldBe(expectedDays);
    }

    [Fact]
    public void Lifetime_is_timespan_from_hours_when_type_is_hours()
    {
        const int expectedHours = 7;
        var configurableTimespan = new ConfigurableTimespan
        {
            Type = ConfigurableTimespanType.Hours,
            Value = expectedHours
        };

        var lifetime = configurableTimespan.GetLifetime();

        lifetime.TotalHours.ShouldBe(expectedHours);
    }

    [Theory]
    [InlineData(ConfigurableTimespanType.Minutes)]
    [InlineData(ConfigurableTimespanType.Mins)]
    public void Lifetime_is_timespan_from_minutes_when_type_is_minutes(ConfigurableTimespanType type)
    {
        const int expectedMinutes = 17;
        var configurableTimespan = new ConfigurableTimespan
        {
            Type = type,
            Value = expectedMinutes
        };

        var lifetime = configurableTimespan.GetLifetime();

        lifetime.TotalMinutes.ShouldBe(expectedMinutes);
    }

    [Theory]
    [InlineData(ConfigurableTimespanType.Seconds)]
    [InlineData(ConfigurableTimespanType.Secs)]
    public void Lifetime_is_timespan_from_seconds_when_type_is_seconds(ConfigurableTimespanType type)
    {
        const int expectedSeconds = 42;
        var configurableTimespan = new ConfigurableTimespan
        {
            Type = type,
            Value = expectedSeconds
        };

        var lifetime = configurableTimespan.GetLifetime();

        lifetime.TotalSeconds.ShouldBe(expectedSeconds);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Exception_when_value_is_less_than_or_equal_to_zero(int value)
    {
        const string expectedMessage = "The value cannot be less than or equal to zero.";

        var configurableTimespan = new ConfigurableTimespan
        {
            Type = ConfigurableTimespanType.Hours,
            Value = value
        };

        Action action = () => configurableTimespan.GetLifetime();

        action.ShouldThrow<ArgumentException>(expectedMessage);
    }
}
