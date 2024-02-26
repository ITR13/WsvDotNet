using WsvNet;
using static TestProject1.TemplateValues;

namespace TestProject1;

public class TestReader
{
    [Test]
    public void TestRead()
    {
        var span = CorrectTsv.AsSpan();
        var result = WsvReader.Read(span);

        Assert.That(result, Has.Length.EqualTo(CorrectValues.Length));
        for (var i = 0; i < result.Length; i++)
        {
            Assert.That(result[i], Is.EquivalentTo(CorrectValues[i]));
        }

        Assert.Pass();
    }

    [Test]
    public void TestException_StringNotClosed()
    {
        Assert.Throws<StringNotClosedException>(
            () =>
            {
                var span = StringNotClosedTsv.AsSpan();
                WsvReader.Read(span);
            }
        );
        Assert.Pass();
    }

    [Test]
    public void TestException_QuoteAfterValue()
    {
        Assert.Throws<InvalidDoubleQuoteAfterValueException>(
            () =>
            {
                var span = QuoteAfterValueTsv.AsSpan();
                WsvReader.Read(span);
            }
        );
        Assert.Pass();
    }

    [Test]
    public void TestException_CharacterAfterString()
    {
        Assert.Throws<InvalidCharacterAfterStringException>(
            () =>
            {
                var span = CharacterAfterStringTsv.AsSpan();
                WsvReader.Read(span);
            }
        );
        Assert.Pass();
    }

    [Test]
    public void TestException_InvalidLineBreak1()
    {
        Assert.Throws<InvalidStringLineBreakException>(
            () =>
            {
                var span = InvalidLineBreak1Tsv.AsSpan();
                WsvReader.Read(span);
            }
        );
        Assert.Pass();
    }

    [Test]
    public void TestException_InvalidLineBreak2()
    {
        Assert.Throws<InvalidStringLineBreakException>(
            () =>
            {
                var span = InvalidLineBreak2Tsv.AsSpan();
                WsvReader.Read(span);
            }
        );
        Assert.Pass();
    }
}