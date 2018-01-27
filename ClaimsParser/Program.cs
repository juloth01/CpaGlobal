namespace ClaimsParser
{
    using System;
    using System.IO;

    public class Program
    {
        private static void Main(string[] args)
        {
            string preParseClaim = File.ReadAllText(@"c:\claims.txt");

            Parser claimsParser = new Parser(preParseClaim);

            claimsParser.ParseClaims();
            
            Console.WriteLine(claimsParser.JsonClaim);

            Console.ReadKey();
        }
    }
}
