using Audacia.Auth.OpenIddict.DependencyInjection;
using Audacia.Auth.OpenIddict.UserInfo;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace Audacia.Auth.OpenIddict.Tests.DependencyInjection
{
    public class TypeResolutionTests
    {
        private readonly IServiceCollection _services;

        public TypeResolutionTests()
        {
            var mockHostingEnvironment = new Mock<IWebHostEnvironment>();
            mockHostingEnvironment.SetupGet(env => env.EnvironmentName).Returns(Environments.Development);

            _services = new ServiceCollection();
            _services
                .AddTransient(_ => new Mock<IRoleStore<DummyRole>>().Object)
                .AddTransient(_ => new Mock<IUserStore<DummyUser>>().Object)
                .AddIdentity<DummyUser, DummyRole>()
                .AddUserManager<UserManager<DummyUser>>();
            _services.AddOpenIddict<DummyUser, int>(
                _ => { },
                user => user.Id,
                new Common.Configuration.OpenIdConnectConfig(),
                mockHostingEnvironment.Object);
        }

        [Fact]
        public void Can_resolve_user_info_handler()
        {
            var serviceProvider = _services.BuildServiceProvider();

            var userInfoHandler = serviceProvider.GetRequiredService<IUserInfoHandler<DummyUser, int>>();

            userInfoHandler.Should().NotBeNull();
        }

        public class DummyUser : IdentityUser<int>
        {
        }

        public class DummyRole : IdentityRole<int>
        {
        }
    }
}
