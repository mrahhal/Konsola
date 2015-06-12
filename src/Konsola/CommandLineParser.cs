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
	/// <summary>
	/// A dynamic command line parser.
	/// </summary>
	public class CommandLineParser
	{
		private static readonly object s_trueBox = (object)true;

		private static readonly Type s_typeOfString = typeof(string);
		private static readonly Type s_typeOfStringArray = typeof(string[]);
		private static readonly Type s_typeOfInt = typeof(int);
		private static readonly Type s_typeOfBool = typeof(bool);

		private ContextBase _context;
		private IConsole _console;
		private string[] _args;
		private Type _contextType;
		private ContextOptionsAttribute _options;

		private CommandLineParser(ContextBase context, IConsole console, string[] args)
		{
			_context = context;
			_console = console;
			_args = args;
		}

		/// <summary>
		/// Parses the provided args into a new context instance of <typeparamref name="T"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception>
		/// <exception cref="ContextException">The context is invalid.</exception>
		/// <exception cref="CommandLineException">The args did not correctly match the expectations of the context.</exception>
		/// <returns>The new context containing the parsed options.</returns>
		public static T Parse<T>(string[] args)
			where T : ContextBase, new()
		{
			return Parse<T>(args, null);
		}

		/// <summary>
		/// Parses the provided args into a new context instance of <typeparamref name="T"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception>
		/// <exception cref="ContextException">The context is invalid.</exception>
		/// <exception cref="CommandLineException">The args did not correctly match the expectations of the context.</exception>
		/// <returns>The new context containing the parsed options.</returns>
		public static T Parse<T>(string[] args, IConsole console)
			where T : ContextBase, new()
		{
			if (args == null)
				throw new ArgumentNullException("args");
			if (console == null)
				console = SilentConsole.Instance;

			var context = new T();
			return new CommandLineParser(context, console, args)._InternalParse<T>();
		}

		private T _InternalParse<T>()
			where T : ContextBase
		{
			_contextType = typeof(T);
			_options = _context.Options = _contextType.GetCustomAttribute<ContextOptionsAttribute>() ?? new ContextOptionsAttribute();

			_InternalWork();
			if (_options.InvokeMethods)
			{
				_InvokeOnParsedMethod();
			}

			return (T)_context;
		}

		private void _InternalWork()
		{
			try
			{
				var tokens = _ParseTokens(_args);
				_ProcessTokens(tokens, 0, _context);
			}
			catch (CommandLineException ex)
			{
				_console.WriteLine(WriteKind.Error, ex.Message);

#if NET40
				if (_options.ExitOnException)
				{
					Environment.Exit(0);
				}
#endif

				throw;
			}
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
						{
							throw new CommandLineException(CommandLineExceptionKind.InvalidParameter, arg);
						}
					}

					list.Add(new Token(arg, true));
					j++;
				}
			}

			return list.ToArray();
		}

		private void _ProcessTokens(Token[] tokens, int offset, ContextBase context)
		{
			var commandType = _FindTargetCommandType(tokens, ref offset, _contextType);
			if (commandType == null)
			{
				// Resolve the default command.
				var defaultCommandAttribute = _contextType.GetCustomAttribute<DefaultCommandAttribute>();
				if (defaultCommandAttribute == null)
				{
					// No command has been specified on the command line
					// and no default command has been registered.
					// TODO: Print usage instead?
					throw new CommandLineException(CommandLineExceptionKind.NoCommand, "");
				}
				commandType = defaultCommandAttribute.DefaultCommand;
			}

			var command = commandType.CreateInstance<CommandBase>();
			context.Command = command;
			command.ContextBase = context;
			var parameterContexts = commandType.GetPropertyContexts().ToArray();
			_InitializePropertyAttributes(parameterContexts);

			// Bind the command.
			_BindCommandOptions(tokens, offset, command, parameterContexts);

			// Check for mandatory params that have not been set.
			_EnsureMandatoriesSet(parameterContexts);
		}

		private Type _FindTargetCommandType(Token[] tokens, ref int offset, Type contextType, Type lastCommandType = null)
		{
			var type = lastCommandType ?? contextType;

			var token = tokens[offset];
			if (token.Kind != TokenKind.Command)
			{
				return null;
			}

			++offset;
			var cc = type.GetCommandContextOrDefault(token.Param);
			if (cc == null)
			{
				throw new CommandLineException(CommandLineExceptionKind.InvalidCommand, token.Param);
			}

			return _FindTargetCommandType(tokens, ref offset, contextType, cc.Type) ?? cc.Type;
		}

		private void _InitializePropertyAttributes(ParameterContext[] parameterContexts)
		{
			foreach (var pc in parameterContexts)
			{
				var att = pc.ParameterAttribute;
				var prop = pc.Property;
				var propType = prop.PropertyType;

				if (propType == s_typeOfString)
				{
					att.Kind = ParameterKind.String;
				}
				else if (propType == s_typeOfInt)
				{
					att.Kind = ParameterKind.Int;
				}
				else if (propType == s_typeOfBool)
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
				else if (propType == s_typeOfStringArray)
				{
					att.Kind = ParameterKind.StringArray;
				}
				else
				{
					throw new ContextException("Invalid type in a context property.");
				}
			}
		}

		private void _BindCommandOptions(Token[] tokens, int offset, CommandBase command, ParameterContext[] parameterContexts)
		{
			for (int i = offset; i < tokens.Length; i++)
			{
				var token = tokens[i];
				var parameterContext = parameterContexts.FirstOrDefault(pc => pc.ParameterAttribute.InternalParameters.Contains(token.Param));
				if (parameterContext == null)
				{
					throw new CommandLineException(CommandLineExceptionKind.InvalidParameter, token.Param);
				}

				// Bind the values.
				if (token.Kind == TokenKind.Partial)
				{
					parameterContext.Property.SetValue(command, s_trueBox, null);
				}
				else // TokenKind.Full
				{
					Action<object> validateConstraints = (value) =>
					{
						// Validate the constraints.
						foreach (var constraint in parameterContext.ConstraintAttributes)
						{
							if (!constraint.Validate(value))
							{
								throw new CommandLineException(CommandLineExceptionKind.Constraint, token.Param, constraint.ErrorMessage);
							}
						}
					};

					switch (parameterContext.ParameterAttribute.Kind)
					{
						case ParameterKind.String:
							{
								validateConstraints(token.Value);
								parameterContext.Property.SetValue(command, token.Value, null);
							}
							break;

						case ParameterKind.Int:
							{
								int parsed;
								if (!int.TryParse(token.Value, out parsed))
								{
									throw new CommandLineException(CommandLineExceptionKind.InvalidValue, token.Param);
								}
								validateConstraints(parsed);
								parameterContext.Property.SetValue(command, parsed, null);
							}
							break;

						case ParameterKind.Enum:
							{
								var att = parameterContext.ParameterAttribute;
								var value = token.Value;
								if (!att.IsFlags)
								{
									if (value.Contains(",") || !att.ValidValues.Contains(value, StringComparer.OrdinalIgnoreCase))
									{
										throw new CommandLineException(CommandLineExceptionKind.InvalidValue, token.Param);
									}
									var e = Enum.Parse(parameterContext.Property.PropertyType, value, true);
									validateConstraints(e);
									parameterContext.Property.SetValue(command, e, null);
								}
								else
								{
									var values = value.Split(',');
									int crux = 0;
									foreach (var v in values)
									{
										if (!att.ValidValues.Contains(v, StringComparer.OrdinalIgnoreCase))
										{
											throw new CommandLineException(CommandLineExceptionKind.InvalidValue, token.Param);
										}
										var e = Enum.Parse(parameterContext.Property.PropertyType, v, true);
										crux |= (int)e;
									}
									validateConstraints(crux);
									parameterContext.Property.SetValue(command, crux, null);
								}
							}
							break;

						case ParameterKind.StringArray:
							{
								var values = token.Value.Split(',');
								validateConstraints(values);
								parameterContext.Property.SetValue(command, values, null);
							}
							break;
					}
				}
				parameterContext.ParameterAttribute.IsSet = true;
			}
		}

		private static void _EnsureMandatoriesSet(ParameterContext[] parameterContexts)
		{
			var unset = default(ParameterContext);
			for (int i = 0; i < parameterContexts.Length; i++)
			{
				if (parameterContexts[i].ParameterAttribute.IsMandatory && !parameterContexts[i].ParameterAttribute.IsSet)
				{
					unset = parameterContexts[i];
					break;
				}
			}
			if (unset != null)
			{
				throw new CommandLineException(CommandLineExceptionKind.MissingParameter, unset.ParameterAttribute.InternalParameters[0]);
			}
		}

		private void _InvokeOnParsedMethod()
		{
			var onParsedMethod = _contextType
						 .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
						 .Where(mi => mi.GetParameters().Length == 0)
						 .Where(mi => mi.GetCustomAttribute<OnParsedAttribute>() != null)
						 .FirstOrDefault();

			if (onParsedMethod != null)
			{
				onParsedMethod.Invoke(_context, null);
			}
		}
	}
}