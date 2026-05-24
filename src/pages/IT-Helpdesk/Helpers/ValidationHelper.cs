using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace IT_Helpdesk.Helpers
{
    /// <summary>
    /// Provides reusable, static input validation methods for use across ViewModels
    /// and code-behind files throughout the application.
    /// </summary>
    public static class ValidationHelper
    {
        // Reasonable date range: no earlier than year 2000, no later than 10 years from now.
        private static readonly DateTime MinReasonableDate = new DateTime(2000, 1, 1);
        private static readonly DateTime MaxReasonableDate = DateTime.Today.AddYears(10);

        // Accepts digits and common phone-formatting characters: +, -, spaces, parentheses.
        private static readonly Regex PhonePattern = new Regex(@"^[\d\+\-\s\(\)]+$", RegexOptions.Compiled);

        // Username: 4–30 chars, letters/digits/underscores/hyphens, must start with a letter.
        private static readonly Regex UsernamePattern = new Regex(@"^[a-zA-Z][a-zA-Z0-9_\-]{3,29}$", RegexOptions.Compiled);

        // ── Public validation methods ────────────────────────────────────────

        /// <summary>
        /// Validates that <paramref name="value"/> is not null, empty, or whitespace.
        /// </summary>
        public static (bool IsValid, string ErrorMessage) IsNotEmpty(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                return (false, $"{fieldName} is required.");

            return (true, string.Empty);
        }

        /// <summary>
        /// Validates a username:
        /// - 4 to 30 characters
        /// - Must start with a letter
        /// - Only letters, digits, underscores, and hyphens allowed
        /// </summary>
        public static (bool IsValid, string ErrorMessage) IsValidUsername(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return (false, "Username is required.");

            if (value.Length < 4)
                return (false, "Username must be at least 4 characters.");

            if (value.Length > 30)
                return (false, "Username must not exceed 30 characters.");

            if (!char.IsLetter(value[0]))
                return (false, "Username must start with a letter.");

            if (!UsernamePattern.IsMatch(value))
                return (false, "Username may only contain letters, digits, underscores (_), and hyphens (-).");

            return (true, string.Empty);
        }

        /// <summary>
        /// Validates a strong password:
        /// - At least 8 characters
        /// - At least one uppercase letter
        /// - At least one lowercase letter
        /// - At least one digit
        /// - At least one special character
        /// </summary>
        public static (bool IsValid, string ErrorMessage) IsStrongPassword(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return (false, "Password is required.");

            if (value.Length < 8)
                return (false, "Password must be at least 8 characters.");

            if (!value.Any(char.IsUpper))
                return (false, "Password must contain at least one uppercase letter (A–Z).");

            if (!value.Any(char.IsLower))
                return (false, "Password must contain at least one lowercase letter (a–z).");

            if (!value.Any(char.IsDigit))
                return (false, "Password must contain at least one number (0–9).");

            if (!value.Any(c => !char.IsLetterOrDigit(c)))
                return (false, "Password must contain at least one special character (e.g. @, #, !, $).");

            return (true, string.Empty);
        }

        /// <summary>
        /// Returns a password strength label: Weak, Fair, Strong, or Very Strong.
        /// </summary>
        public static string GetPasswordStrength(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            int score = 0;
            if (value.Length >= 8)  score++;
            if (value.Length >= 12) score++;
            if (value.Any(char.IsUpper)) score++;
            if (value.Any(char.IsLower)) score++;
            if (value.Any(char.IsDigit)) score++;
            if (value.Any(c => !char.IsLetterOrDigit(c))) score++;

            return score switch
            {
                <= 2 => "Weak",
                3    => "Fair",
                4 or 5 => "Strong",
                _    => "Very Strong"
            };
        }

        /// <summary>
        /// Validates that <paramref name="value"/> contains only numeric digits and optional
        /// phone-formatting characters (+, -, spaces, parentheses).
        /// </summary>
        public static (bool IsValid, string ErrorMessage) IsNumericOnly(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                return (true, string.Empty);

            if (!PhonePattern.IsMatch(value))
                return (false, $"{fieldName} must contain numbers only.");

            return (true, string.Empty);
        }

        /// <summary>
        /// Validates Philippine mobile number format:
        /// - Must be exactly 11 digits
        /// - Must start with "09"
        /// - Example: 09756061460
        /// </summary>
        public static (bool IsValid, string ErrorMessage) IsValidPhilippineContact(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return (true, string.Empty); // Contact is optional

            if (value.Length != 11)
                return (false, "Contact number must be exactly 11 digits.");

            if (!value.StartsWith("09"))
                return (false, "Contact number must start with 09.");

            if (!value.All(char.IsDigit))
                return (false, "Contact number must contain only digits.");

            return (true, string.Empty);
        }

        /// <summary>
        /// Validates that <paramref name="value"/> is not null and not the default
        /// <see cref="DateTime"/> value, and falls within a reasonable date range.
        /// </summary>
        public static (bool IsValid, string ErrorMessage) IsValidDate(DateTime? value, string fieldName)
        {
            if (value == null || value == default(DateTime))
                return (false, $"{fieldName} is required.");

            if (value < MinReasonableDate)
                return (false, $"{fieldName} must be on or after {MinReasonableDate:MM/dd/yyyy}.");

            if (value > MaxReasonableDate)
                return (false, $"{fieldName} must be on or before {MaxReasonableDate:MM/dd/yyyy}.");

            return (true, string.Empty);
        }

        /// <summary>
        /// Validates that <paramref name="startDate"/> is not later than <paramref name="endDate"/>.
        /// </summary>
        public static (bool IsValid, string ErrorMessage) IsValidDateRange(DateTime? startDate, DateTime? endDate)
        {
            var fromCheck = IsValidDate(startDate, "From Date");
            if (!fromCheck.IsValid) return fromCheck;

            var toCheck = IsValidDate(endDate, "To Date");
            if (!toCheck.IsValid) return toCheck;

            if (startDate!.Value > endDate!.Value)
                return (false, "From Date must be on or before To Date.");

            return (true, string.Empty);
        }

        /// <summary>
        /// Aggregates multiple validation results and returns the first failure found.
        /// </summary>
        public static (bool IsValid, string ErrorMessage) ValidateAll(
            params (bool IsValid, string ErrorMessage)[] results)
        {
            foreach (var result in results)
            {
                if (!result.IsValid)
                    return result;
            }

            return (true, string.Empty);
        }
    }
}
