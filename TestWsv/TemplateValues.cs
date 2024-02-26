namespace TestProject1;

public static class TemplateValues
{
    public const string CorrectTsv =
        @"FirstName LastName Age PlaceOfBirth
Lucas     Brown
William   Smith    30  Boston
Lucy      Reynolds 27    
Olivia    Jones    -   ""San Francisco""
   
""Hello""""World"" ""New""/""Line"" """" -

" + "\n \ncat\n\n";

    public static readonly string?[][] CorrectValues =
    [
        ["FirstName", "LastName", "Age", "PlaceOfBirth"],
        ["Lucas", "Brown"],
        ["William", "Smith", "30", "Boston"],
        ["Lucy", "Reynolds", "27"],
        ["Olivia", "Jones", null, "San Francisco"],
        [],
        ["Hello\"World", "New\nLine", "", null],
        [], [], [],
        ["cat"],
        [], []
    ];


    public const string StringNotClosedTsv = @"FirstName LastName Age PlaceOfBirth
Lucas     Brown  ""50
William   Smith    30  Boston
Lucy      Reynolds 27    
Olivia    Jones    -   ""San Francisco""";

    public const string QuoteAfterValueTsv = @"FirstName LastName Age PlaceOfBirth
Lucas     Brown    50
William   Smith""  30  Boston
Lucy      Reynolds 27    
Olivia    Jones    -   ""San Francisco""";

    public const string CharacterAfterStringTsv = @"FirstName LastName Age PlaceOfBirth
Lucas     Brown    50  ""Cat""Town
William   Smith    30  Boston
Lucy      Reynolds 27    
Olivia    Jones    -   ""San Francisco""";

    public const string InvalidLineBreak1Tsv = @"FirstName LastName Age PlaceOfBirth
Lucas     Brown    50  ""Cat""/ Town
William   Smith    30  Boston
Lucy      Reynolds 27    
Olivia    Jones    -   ""San Francisco""";

    public const string InvalidLineBreak2Tsv = @"FirstName LastName Age PlaceOfBirth
Lucas     Brown    50  ""Cat""/Town
William   Smith    30  Boston
Lucy      Reynolds 27    
Olivia    Jones    -   ""San Francisco""";
}