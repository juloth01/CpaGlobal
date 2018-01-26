/* ------------------------------------------------------------------------------------------
Assumptions made in this iteration for Claims Parsing:
    Claims always start out with digits followed by . at the beginning of a line
    Claims that are subclaims will always follow it's parent claim
    All claim lines end in . 
    I will get claification before refactoring - this is brute force method
------------------------------------------------------------------------------------------ */

namespace ClaimsParser
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Newtonsoft.Json;

    #endregion

    public class Parser
    {
        private Regex claimIdRegex = new Regex(@"^(\d+)\.", RegexOptions.Singleline);

        public Parser(string claims)
        {
            if (string.IsNullOrEmpty(claims))
            {
                throw new ArgumentException("Claims cannot be empty", claims);
            }

            this.RawClaim = claims;
        }

        public string RawClaim { get; set; }

        public string JsonClaim { get; set; }

        public void ParseClaims()
        {
            List<string> claimLines = new List<string>();
            claimLines = this.RawClaim.Split(Environment.NewLine.ToCharArray()).ToList();
            Dictionary<string, string> claims = new Dictionary<string, string>();
            string previousClaimId = string.Empty;
            foreach (string line in claimLines)
            {
                Match matchResult = this.claimIdRegex.Match(line);
                if (matchResult.Success)
                {
                    previousClaimId = matchResult.Groups[1].Value;
                    claims.Add($"Claim {previousClaimId}", "Test Claim text.");
                }
                else
                {
                    if (!string.IsNullOrEmpty(previousClaimId))
                    {
                        string keyName = $"Claim {previousClaimId}";
                        if (claims.ContainsKey(keyName))
                        {
                            claims[keyName] += line;
                        }
                    }
                }
            }

            this.JsonClaim = JsonConvert.SerializeObject(claims, Formatting.Indented);

        }


    }
}