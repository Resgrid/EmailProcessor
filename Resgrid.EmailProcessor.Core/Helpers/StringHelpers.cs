using System.Text.RegularExpressions;

namespace Resgrid.EmailProcessor.Core.Helpers
{
	public static class StringHelpers
	{
		public static bool ValidateEmail(string email)
		{
			string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
										+ @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
										+ @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

			Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

			return regex.IsMatch(email);
		}
	}
}
