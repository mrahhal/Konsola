using System;

namespace Konsola.Parser
{
	public static class ValidationHelper
	{
		private static char[] _invalidCharacters;

		static ValidationHelper()
		{
			_invalidCharacters = " \t\r\n\\/".ToCharArray();
		}

		public static void ValidateCommandType(Type command)
		{
			if (command == null)
			{
				throw new ContextException("Command type cannot be null.");
			}
			if (!command.IsCommandType())
			{
				throw new ContextException("Commands must extend CommandBase.");
			}
		}

		public static void ValidateCommandAttribute(CommandAttribute command)
		{
			if (ContainsAny(command.Name, _invalidCharacters))
			{
				throw new ContextException("The command's name contains invalid characters.");
			}
		}

		public static void ValidateParameterAttribute(ParameterAttribute parameter)
		{
			if (ContainsAny(parameter.Names, _invalidCharacters))
			{
				throw new ContextException("The parameter's names contain invalid characters.");
			}
		}

		private static bool ContainsAny(string value, char[] chars)
		{
			for (int i = 0; i < value.Length; i++)
			{
				for (int j = 0; j < chars.Length; j++)
				{
					if (value[i] == chars[j])
						return true;
				}
			}
			return false;
		}
	}
}
