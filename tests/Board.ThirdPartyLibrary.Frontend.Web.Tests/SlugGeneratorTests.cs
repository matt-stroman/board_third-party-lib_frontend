using Board.ThirdPartyLibrary.Frontend.Web.Services;

namespace Board.ThirdPartyLibrary.Frontend.Web.Tests;

public sealed class SlugGeneratorTests
{
    [Theory]
    [InlineData(null, "item")]
    [InlineData("", "item")]
    [InlineData("   ", "item")]
    public void ToKebabCase_WithEmptyInput_UsesDefaultFallback(string? input, string expected)
    {
        var slug = SlugGenerator.ToKebabCase(input);

        Assert.Equal(expected, slug);
    }

    [Fact]
    public void ToKebabCase_WithCustomFallback_UsesFallbackForSymbolOnlyInput()
    {
        var slug = SlugGenerator.ToKebabCase("!@#$%^&*()", "new-title");

        Assert.Equal("new-title", slug);
    }

    [Theory]
    [InlineData("Crème Brûlée", "creme-brulee")]
    [InlineData("  Star   Blasters___Deluxe!!! ", "star-blasters-deluxe")]
    [InlineData("Board 2D VR", "board-2d-vr")]
    [InlineData("Already-kebab-case", "already-kebab-case")]
    public void ToKebabCase_NormalizesUnexpectedCharacters(string input, string expected)
    {
        var slug = SlugGenerator.ToKebabCase(input);

        Assert.Equal(expected, slug);
    }
}
