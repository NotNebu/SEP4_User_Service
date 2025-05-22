using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class PasswordStrength : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var password = value as string;

        if (string.IsNullOrWhiteSpace(password))
            return new ValidationResult("Kodeord er nødvendig.");

        if (password.Length < 8)
            return new ValidationResult("Mindst 8 tegn");
        if (!Regex.IsMatch(password, "[A-Z]"))
            return new ValidationResult("Mindst ét stort bogstav");
        if (!Regex.IsMatch(password, "[a-z]"))
            return new ValidationResult("Mindst ét lille bogstav");
        if (!Regex.IsMatch(password, "[0-9]"))
            return new ValidationResult("Mindst ét tal");
        if (!Regex.IsMatch(password, @"[\W_]")) // matcher symbols som i frontend
            return new ValidationResult("Mindst ét symbol");

        return ValidationResult.Success;
    }
}
