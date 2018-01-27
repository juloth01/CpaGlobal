/* ------------------------------------------------------------------------------------------
Assumptions made in this iteration for Claims Parsing:
    Claims always start out with digits followed by `.` at the beginning of a line
    Claims that are subclaims will always follow it's parent claim
    All claim lines end in `.` 
    
Refactor #2
    Parent claims (claims not part of any other) are assumed to be multi-lined
    Recursive searching to add child claims
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
        private Regex currentClaimIdRegex = new Regex(@"^(?<CurrentClaimId>\d+)\.\s(?<CurrentClaimText>.+)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        private Regex parentClaimIdRegex = new Regex(@"The system of (?<ParentClaimId>claim \d+),\s(?<ParentClaimText>.+)|The method of (?<ParentClaimId>claim \d+),\s(?<ParentClaimText>.+)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

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

        private List<Claim> claims = new List<Claim>();

        private Claim ParentClaim { get; set; }

        public void ParseClaims()
        {
            List<string> claimLines = new List<string>();
            claimLines = this.RawClaim.Split(Environment.NewLine.ToCharArray()).ToList();
            string previousClaimId = string.Empty;

            foreach (string line in claimLines)
            {
                Match currentClaimIdMatchResult = this.currentClaimIdRegex.Match(line);
                if (currentClaimIdMatchResult.Success)
                {
                    // This is the current Claim line - may be a parent/child/single - let's check
                    Match parentClaimIdMatchResult = this.parentClaimIdRegex.Match(line);
                    if (parentClaimIdMatchResult.Success)
                    {
                        string parentClaimId = parentClaimIdMatchResult.Groups["ParentClaimId"].Value;
                        this.FindParentClaim(this.claims, parentClaimId);
                        if (this.ParentClaim != null)
                        {
                            Claim childClaim = new Claim
                            {
                                ClaimId = $"claim {currentClaimIdMatchResult.Groups["CurrentClaimId"].Value}",
                                ClaimText = parentClaimIdMatchResult.Groups["ParentClaimText"].Value,
                                ParentClaimId = parentClaimId
                            };

                            this.ParentClaim.SubClaims.Add(childClaim);
                        }
                    }
                    else
                    {
                        // this is not a child claim 
                        Claim parentClaim = new Claim
                        {
                            ClaimId = $"claim {currentClaimIdMatchResult.Groups["CurrentClaimId"].Value}",
                            ClaimText = currentClaimIdMatchResult.Groups["CurrentClaimText"].Value
                        };

                        previousClaimId = parentClaim.ClaimId;
                        this.claims.Add(parentClaim);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(previousClaimId))
                    {
                        foreach (Claim claim in this.claims)
                        {
                            if (claim.ClaimId.Equals(previousClaimId))
                            {
                                claim.ClaimText += $" {line}"; // append to the claim
                            }
                        }
                    }
                }
            }

            this.JsonClaim = JsonConvert.SerializeObject(this.claims, Formatting.Indented);

        }

        private void FindParentClaim(List<Claim> claimsList, string claimId)
        {
            this.ParentClaim = null;

            foreach (Claim claim in claimsList)
            {
                if (this.ParentClaim == null)
                {
                    this.SearchChildClaims(claim, claimId);
                }
            }
        }

        public void SearchChildClaims(Claim childClaim, string claimId)
        {
            if (childClaim == null)
            {
                return;
            }

            if (childClaim.ClaimId.Equals(claimId))
            {
                this.ParentClaim = childClaim;
            }

            foreach (Claim subClaim in childClaim.SubClaims)
            {
                this.SearchChildClaims(subClaim, claimId);
            }
        }
    }
}