using System.Text.RegularExpressions;
using WsvNet;
using static TestProject1.TemplateValues;

namespace TestProject1;

public class TestWriter
{
    [Test]
    public void TestWrite()
    {
        var expected = Regex.Replace(CorrectTsv, @" +", " ").Split('\n').Select(line => line.Trim()).ToArray();
        var result = WsvWriter.WriteTsv(CorrectValues).Split('\n');

        Assert.That(result, Is.EquivalentTo(expected));
        Assert.Pass();
    }
}