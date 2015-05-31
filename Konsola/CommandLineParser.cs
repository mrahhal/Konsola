//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Konsola.Attributes;
using Konsola.Internal;

namespace Konsola
{
	public class CommandLineParser
	{
		private readonly object _boxedTrue = (object)true;

		private Type _typeOfString = typeof(string);
		private Type _typeOfInt = typeof(int);
		private Type _typeOfBool = typeof(bool);

		private ContextBase _context;
		private string[] _args;
		private Type _type;
		private ContextOptionsAttribute _options;

		private CommandLineParser(ContextBase context, string[] args)
		{
			_context = context;
			_args = args;
		}

		public static T Parse<T>(string[] args)
			where T : ContextBase
		{
			var context = typeof(T).CreateInstance<ContextBase>();
			return new CommandLineParser(context, args)._InternalParse<T>();
		}

		private T _InternalParse<T>()
			where T : ContextBase
		{
			_type = typeof(T);
			_options = _type.GetCustomAttribute<ContextOptionsAttribute>() ?? new ContextOptionsAttribute();

			_InternalWork();
			_InvokeOnParsedMethod(); // TODO: Invoke only with an option in ContextOptionsAttribute?

			return (T)_context;
		}

		private void _InternalWork()
		{
			var tokens = _ParseTokens(_args);

#if !DEBUG
			try
			{
#endif
			_Parse(tokens, 0, _context);
#if !DEBUG
			}
			catch (ParsingException ex)
			{
				if (!_options.ExitOnException)
				{
					throw;
				}

				Console.WriteLine(ex.Message);
				Environment.Exit(1);
			}
#endif
		}

		private Token[] _ParseTokens(string[] args)
		{
			var list = new List<Token>();

			var j = default(int);
			for (int i = 0; i < args.Length; i++)
			{
				var arg = args[i];

				if (arg.StartsWith("--"))
				{
					list.Add(new Token(arg.Substring(2), false));
					j++;
				}
				else if (arg.StartsWith("-"))
				{
					if (args.Length == i + 1)
					{
						throw new CommandLineException(CommandLineExceptionKind.MissingValue, arg);
					}

					var nextArg = args[++i];
					if (nextArg.StartsWith("-"))
					{
						throw new CommandLineException(CommandLineExceptionKind.MissingValue, arg);
					}

					list.Add(new Token(arg.Substring(1), nextArg));
					j++;
				}
				else
				{
					if (i != 0)
					{
						if (list[j - 1].Kind != TokenKind.Command)
							throw new CommandLineException(CommandLineExceptionKind.InvalidParameter, arg);
					}

					list.Add(new Token(arg, true));
					j++;
				}
			}

			return list.ToArray();
		}

		private void _Parse(Token[] tokens, int offset, ContextBase context)
		{
			var type = context == _context ? _type : context.GetType();

			var firstToken = tokens[offset];
			if (firstToken.Kind == TokenKind.Command)
			{
				++offset;
				var commandContext = type
					.GetCommandContexts(firstToken.Param)
					.FirstOrDefault();

				if (commandContext.IsEmpty)
				{
					// TODO: + CLEK.InvalidCommand?
					throw new CommandLineException(CommandLineExceptionKind.InvalidParameter, firstToken.Param);
				}

				var newContext = commandContext.Type.CreateInstance<ContextBase>();
				context.InnerContext = newContext;
				_Parse(tokens, offset, newContext);
				return;
			}

			// We are at the end of the contexts.
			var propContexts = type.GetPropertyContexts().ToArray();

			foreach (var propc in propContexts)
			{
				var att = propc.Attribute;
				var prop = propc.Property;
				var propType = prop.PropertyType;

				if (propType == _typeOfString)
				{
					att.Kind = ParameterKind.String;
				}
				else if (propType == _typeOfInt)
				{
					att.Kind = ParameterKind.Int;
				}
				else if (propType == _typeOfBool)
				{
					att.Kind = ParameterKind.Switch;
				}
				else if (propType.IsEnum)
				{
					att.Kind = ParameterKind.Enum;
					if (propType.GetCustomAttribute<FlagsAttribute>() != null)
					{
						att.IsFlags = true;
					}
					var sb = new StringBuilder();
					foreach (var fi in propType.GetFields())
					{
						if (fi.FieldType.BaseType.Name != "Enum")
							continue;
						if (sb.Length != 0)
							sb.Append(',');
						sb.Append(fi.Name);
					}
					att.ValidValues = sb.ToString().Split(',');
				}
				else
				{
					throw new ContextException("Invalid type in a KParameter property.");
				}
			}

			_BindContext(tokens, offset, context, propContexts);

			// Check for mandatory params that have not been set.
			var unset = propContexts.FirstOrDefault(pc => pc.Attribute.IsMandatory && !pc.Attribute.IsSet);
			if (!unset.IsEmpty)
			{
				throw new CommandLineException(CommandLineExceptionKind.MissingParameter, unset.Attribute.InternalParameters[0]);
			}
		}

		private void _BindContext(Token[] tokens, int offset, ContextBase context, PropertyContext[] propContexts)
		{
			for (int i = offset; i < tokens.Length; i++)
			{
				var token = tokens[i];
				var propContext = propContexts.FirstOrDefault(pc => pc.Attribute.InternalParameters.Contains(token.Param));
				if (propContext.IsEmpty)
				{
					throw new CommandLineException(CommandLineExceptionKind.InvalidParameter, token.Param);
				}

				if (token.Kind == TokenKind.Partial)
				{
					propContext.Property.SetValue(context, _boxedTrue, null);
				}
				else // TokenKind.Full
				{
					switch (propContext.Attribute.Kind)
					{
						case ParameterKind.String:
							propContext.Property.SetValue(context, token.Value, null);
							break;

						case ParameterKind.Int:
							int parsed;
							if (!int.TryParse(token.Value, out parsed))
							{
								throw new CommandLineException(CommandLineExceptionKind.InvalidValue, token.Param);
							}
							propContext.Property.SetValue(context, parsed, null);
							break;

						case ParameterKind.Enum:
							var att = propContext.Attribute;
							var value = token.Value;
							if (!att.IsFlags)
							{
								if (value.Contains(',') || !att.ValidValues.Contains(value, StringComparer.InvariantCultureIgnoreCase))
									throw new CommandLineException(CommandLineExceptionKind.InvalidValue, token.Param);
								var e = Enum.Parse(propContext.Property.PropertyType, value, true);
								propContext.Property.SetValue(context, e, null);
							}
							else
							{
								var values = value.Split(',');
								int crux = 0;
								foreach (var v in values)
								{
									if (!att.ValidValues.Contains(v, StringComparer.InvariantCultureIgnoreCase))
										throw new CommandLineException(CommandLineExceptionKind.InvalidValue, token.Param);
									var e = Enum.Parse(propContext.Property.PropertyType, v, true);
									crux |= (int)e;
								}
								propContext.Property.SetValue(context, crux, null);
							}
							break;
					}
				}
				propContext.Attribute.IsSet = true;
			}
		}

		private void _InvokeOnParsedMethod()
		{
			var onParsedMethod = _type
						 .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
						 .Where(mi => mi.GetParameters().Length == 0)
						 .Where(mi => mi.GetCustomAttribute<OnParsedAttribute>() != null)
						 .FirstOrDefault();

			if (onParsedMethod != null)
				onParsedMethod.Invoke(_context, null);
		}
	}
}