using FluentAssertions;
using System;
using Xunit;

namespace CodeSample.Tests.Validators;

public class PersonCreateValidatorTests
{
    [Fact]
    public void Valid_payload_should_pass()
    {
        var dto = new PersonCreateDto
        {
            FirstName = "Ali",
            LastName = "Taheri",
            NationalCode = "1234567890",
            BirthDate = "1996-05-20"
        };

        var validator = new PersonCreateValidator();
        var result = validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1")]
    public void FirstName_invalid_should_fail(string? name)
    {
        var dto = new PersonCreateDto
        {
            FirstName = name!,
            LastName = "Taheri",
            NationalCode = "1234567890",
            BirthDate = "1990-01-01"
        };

        var validator = new PersonCreateValidator();
        var result = validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }

    [Theory]
    [InlineData("a23456789")]   // 9 digits
    [InlineData("12345678901")] // 11 digits
    [InlineData("abcdefghij")]  // non-digits
    public void NationalCode_invalid_should_fail(string nc)
    {
        var dto = new PersonCreateDto
        {
            FirstName = "Ali",
            LastName = "Taheri",
            NationalCode = nc,
            BirthDate = "1990-01-01"
        };

        var validator = new PersonCreateValidator();
        var result = validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NationalCode");
    }

    [Fact]
    public void BirthDate_in_future_should_fail()
    {
        var future = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)).ToString("yyyy-MM-dd");
        var dto = new PersonCreateDto
        {
            FirstName = "Ali",
            LastName = "Taheri",
            NationalCode = "1234567890",
            BirthDate = future
        };

        var validator = new PersonCreateValidator();
        var result = validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BirthDate");
    }
}
