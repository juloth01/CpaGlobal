// ------------------------------------------------------------------------------------------
//     ClaimsParser
//     Claim.cs
//  Created: 01/26/2018 4:11 PM
//  Designed and maintained by Joe Uloth
//  Object to hold hierarchal claims data
// ------------------------------------------------------------------------------------------

namespace ClaimsParser
{
    #region References

    using System.Collections.Generic;

    #endregion

    public class Claim
    {
        public Claim()
        {
            this.ClaimId = string.Empty;
            this.ClaimText = string.Empty;
            this.ParentClaimId = string.Empty;
            this.SubClaims = new List<Claim>();
        }

        public string ClaimId { get; set; }

        public string ClaimText { get; set; }

        public string ParentClaimId { get; set; }

        public List<Claim> SubClaims { get; private set; }

        public bool HasSubClaims()
        {
            return this.SubClaims.Count > 0;
        }
    }
}