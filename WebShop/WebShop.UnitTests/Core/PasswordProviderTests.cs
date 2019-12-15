using System;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using WebShop.Api.Core;
using Xunit;

namespace WebShop.UnitTests.Core
{
    public class PasswordProviderTests
    {
        [Fact]
        public void ComputeHash_returns_params()
        {
            var fixture = new Fixture();
            var password = fixture.Create<string>();
            var sut = fixture.Create<PasswordProvider>();

            var result = sut.ComputeHashParams(password);

            result.Should().NotBeNull();
            result.Salt.Should().NotBeNullOrEmpty();
            result.PasswordHash.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void ComputeHash_fails_if_passwords_empty()
        {
            var sut = new PasswordProvider();

            FluentActions.Invoking(() => sut.ComputeHashParams(string.Empty))
                .Should().Throw<ArgumentException>();
            FluentActions.Invoking(() => sut.ComputeHashParams(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [AutoMoqData]
        public void IsValid_succeeds([Frozen] PasswordProvider sut, 
            string password)
        {
            var (salt, passwordHash) = sut.ComputeHashParams(password);
            sut.IsValid(password, passwordHash, salt).Should().BeTrue();
        }

    }
}