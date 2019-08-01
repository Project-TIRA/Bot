namespace Shared
{
    public static class PhoneNumber
    {
        /// <summary>
        /// Converts a phone number to XXXYYYZZZZ format.
        /// </summary>
        public static string Standardize(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return phoneNumber;
            }

            phoneNumber = StripSeparators(phoneNumber);

            if (phoneNumber.Length == 11 && phoneNumber.StartsWith("1"))
            {
                return phoneNumber.Substring(1);
            }
            else if (phoneNumber.Length == 12 && phoneNumber.StartsWith("+1"))
            {
                return phoneNumber.Substring(2);
            }

            // Can't do anything with the number.
            return string.Empty;
        }

        private static string StripSeparators(string phoneNumber)
        {
            return phoneNumber
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Replace("-", string.Empty)
                .Replace(".", string.Empty)
                .Replace(" ", string.Empty);
        }
    }
}
