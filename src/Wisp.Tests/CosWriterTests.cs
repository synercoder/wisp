using System.Text;

namespace Wisp.Tests;

public sealed class CosWriterTests
{
    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void Should_Write_Booleans_Correctly(bool value, string expected)
    {
        // Given
        var fixture = new CosWriterFixture();

        // When
        fixture.Write(new CosBoolean(value));

        // Then
        fixture.Result.ShouldBe(expected);
    }

    [Fact]
    public void Should_Write_Dates_Correctly()
    {
        // Given
        var fixture = new CosWriterFixture();

        // When
        fixture.Write(new CosDate(new DateTimeOffset(2024, 3, 7, 23, 31, 12, TimeSpan.FromHours(1))));

        // Then
        fixture.Result.ShouldBe("(D:20240307233112+01'00)");
    }

    [Fact]
    public void Should_Write_Arrays_Correctly()
    {
        // Given
        var fixture = new CosWriterFixture();

        // When
        fixture.Write(new CosArray(
            new[]
            {
                new CosBoolean(true),
                new CosBoolean(false),
            }));

        // Then
        fixture.Result.ShouldBe("[true false]");
    }

    [Fact]
    public void Should_Write_Dictionaries_Correctly()
    {
        // Given
        var fixture = new CosWriterFixture();

        // When
        fixture.Write(new CosDictionary
        {
            { new CosName("Size"), new CosInteger(40) },
            { new CosName("Root"), new CosObjectReference(25, 0) },
        });

        // Then
        fixture.Result.ShouldBe(
            """
            <<
            /Size 40
            /Root 25 0 R
            >>
            """);
    }

    [Fact]
    public void Should_Write_Integers_Correctly()
    {
        // Given
        var fixture = new CosWriterFixture();

        // When
        fixture.Write(new CosInteger(32));

        // Then
        fixture.Result.ShouldBe("32");
    }

    [Theory]
    [InlineData("Hello", "/Hello")]
    [InlineData("/Hello", "/Hello")]
    public void Should_Write_Names_Correctly(string name, string expected)
    {
        // Given
        var fixture = new CosWriterFixture();

        // When
        fixture.Write(new CosName(name));

        // Then
        fixture.Result.ShouldBe(expected);
    }

    [Fact]
    public void Should_Write_Null_Correctly()
    {
        // Given
        var fixture = new CosWriterFixture();

        // When
        fixture.Write(new CosNull());

        // Then
        fixture.Result.ShouldBe("null");
    }

    [Fact]
    public void Should_Write_Real_Numbers_Correctly()
    {
        // Given
        var fixture = new CosWriterFixture();

        // When
        fixture.Write(new CosReal(32.33));

        // Then
        fixture.Result.ShouldBe("32.33");
    }

    [Fact]
    public void Should_Write_Hex_Strings_Correctly()
    {
        // Given
        var fixture = new CosWriterFixture();

        // When
        fixture.Write(new CosHexString([80, 97, 116, 114, 105, 107]));

        // Then
        fixture.Result.ShouldBe("<50617472696B>");
    }

    [Fact]
    public void Should_Write_Object_References_Correctly()
    {
        // Given
        var fixture = new CosWriterFixture();

        // When
        fixture.Write(new CosObjectReference(32, 2));

        // Then
        fixture.Result.ShouldBe("32 2 R");
    }

    [Theory]
    [MemberData(nameof(Should_Write_Strings_Correctly_Data))]
    public void Should_Write_Strings_Correctly(string input, byte[] expected)
    {
        // Given
        var fixture = new CosWriterFixture();

        // When
        fixture.Write(new CosString(input));

        // Then
        fixture.RawResult.ShouldBe(expected);
    }

    public static IEnumerable<object[]> Should_Write_Strings_Correctly_Data()
    {
        yield return new object[]
        {
            "Hello World",
            Encoding.ASCII.GetBytes("(Hello World)"),
        };
        yield return new object[]
        {
            "Parenthesis like ( and ) should be escaped.",
            Encoding.ASCII.GetBytes("(Parenthesis like \\( and \\) should be escaped.)"),
        };

        var unicodeChars = "ĀĆĎĒĨĩŏŊ";
        yield return new object[]
        {
            unicodeChars,
            new byte[] { 40, 254, 255, 1, 0, 1, 6, 1, 14, 1, 18, 1, 92, 40, 1, 92, 41, 1, 79, 1, 74, 41 },
        };

        var unicodeAndParenthesis = "ĀĆĎĒĨĩŏŊ and () mixed.";
        yield return new object[]
        {
            unicodeAndParenthesis,
            new byte[]
            {
                40, 254, 255, 1, 0, 1, 6, 1, 14, 1, 18, 1, 92, 40, 1, 92, 41, 1,
                79, 1, 74, 0, 32, 0, 97, 0, 110, 0, 100, 0, 32, 0, 92, 40, 0, 92,
                41, 0, 32, 0, 109, 0, 105, 0, 120, 0, 101, 0, 100, 0, 46, 41,
            },
        };
    }

    [Fact]
    public void Should_Write_Objects_Correctly()
    {
        // Given
        var fixture = new CosWriterFixture();

        // When
        fixture.Write(new CosObject(
            new CosObjectId(12, 0),
            new CosString("Hello World")));

        // Then
        fixture.Result.ShouldBe(
            """
            12 0 obj
            (Hello World)
            endobj
            """);
    }
}