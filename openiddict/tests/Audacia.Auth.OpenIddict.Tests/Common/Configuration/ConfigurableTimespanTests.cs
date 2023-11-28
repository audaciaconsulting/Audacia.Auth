using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Audacia.Auth.OpenIddict.Common.Configuration;
using FluentAssertions;
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

        lifetime.TotalDays.Should().Be(expectedDays);
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

        lifetime.TotalHours.Should().Be(expectedHours);
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

        lifetime.TotalMinutes.Should().Be(expectedMinutes);
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

        lifetime.TotalSeconds.Should().Be(expectedSeconds);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Exception_when_value_is_less_than_or_equal_to_zero(int value)
    {
        var configurableTimespan = new ConfigurableTimespan
        {
            Type = ConfigurableTimespanType.Hours,
            Value = value
        };

        Action action = () => configurableTimespan.GetLifetime();

        action.Should().Throw<ArgumentException>().WithMessage("The value cannot be less than or equal to zero.");
    }
}
