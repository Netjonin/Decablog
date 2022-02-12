using System.Text.RegularExpressions;

namespace DecaBlog.Services.Extensions
{
    public static class ValidateEmail
    {
        public static bool IsValidDecadevEmail(string email)
        {
            Regex regex = new Regex(@"^[A-Za-z0-9._%+-]+@decagon.dev$");
            return regex.Match(email).Success;
        }
    }
}
