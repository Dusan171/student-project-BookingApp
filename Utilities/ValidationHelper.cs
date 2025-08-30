using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace BookingApp.Utilities
{

    public static class ValidationHelper
    {

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }


        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;


            return Regex.IsMatch(name, @"^[a-zA-ZšđčćžŠĐČĆŽ\s]{2,}$");
        }


        public static bool IsValidAge(int age)
        {
            return age >= 1 && age <= 120;
        }


        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;


            return Regex.IsMatch(phone, @"^(\+381|0)[0-9]{8,9}$");
        }


        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Minimum 6 characters, at least one letter and one number
            return password.Length >= 6 &&
                   Regex.IsMatch(password, @"^(?=.*[a-zA-Z])(?=.*\d).+$");
        }


        public static bool IsNotEmpty(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }


        public static bool IsPositiveNumber(double number)
        {
            return number > 0;
        }


        public static bool IsPositiveInteger(int number)
        {
            return number > 0;
        }


        public static bool IsNotPastDate(DateTime date)
        {
            return date.Date >= DateTime.Today;
        }


        public static bool IsValidRating(int rating)
        {
            return rating >= 1 && rating <= 5;
        }


        public static string? GetNameValidationError(string fieldName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return $"{fieldName} je obavezno polje.";

            if (!IsValidName(value))
                return $"{fieldName} može sadržavati samo slova i razmake (minimum 2 karaktera).";

            return null;
        }


        public static string? GetEmailValidationError(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "Email je obavezno polje.";

            if (!IsValidEmail(email))
                return "Email adresa nije u validnom formatu.";

            return null;
        }


        public static string? GetAgeValidationError(int age)
        {
            if (!IsValidAge(age))
                return "Godine moraju biti između 1 i 120.";

            return null;
        }


        public static string? GetPasswordValidationError(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "Lozinka je obavezna.";

            if (!IsValidPassword(password))
                return "Lozinka mora imati minimum 6 karaktera, bar jedno slovo i jedan broj.";

            return null;
        }
    }
}