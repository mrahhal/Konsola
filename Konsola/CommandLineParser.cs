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
		private readonly object _trueBox = (object)true;

		private Type _typeOfString = typeof(string);
		private Type _typeOfStringArray = typeof(string[]);
		private Type _typeOfInt = typeof(int);
		private Type _typeOfBool = typeof(bool);

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
				console = new Console();

			var context = new T();
			return new CommandLineParser(context, console, args)._InternalParse<T>();
		}

		private T _InternalParse<T>()
			where T : ContextBase
		{
			_contextType = typeof(T);
			_options = _contextType.GetCustomAttribute<ContextOptionsAttribute>() ?? new ContextOptionsAttribute();

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
				if (_options.ExitOnException)
				{
					_console.WriteErrorLine(ex.Message);
					Environment.Exit(1);
				}

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
			var propContexts = commandType.GetPropertyContexts().ToArray();
			_InitializePropertyAttributes(propContexts);

			// Bind the command.
			_BindCommandOptions(tokens, offset, command, propContexts);

			// Check for mandatory params that have not been set.
			_EnsureMandatoriesSet(propContexts);
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
			if (cc.IsEmpty)
			{
				throw new CommandLineException(CommandLineExceptionKind.InvalidCommand, token.Param);
			}

			return _FindTargetCommandType(tokens, ref offset, contextType, cc.Type) ?? cc.Type;
		}

		private void _InitializePropertyAttributes(PropertyContext[] propContexts)
		{
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
				else if (propType == _typeOfStringArray)
				{
					att.Kind = ParameterKind.StringArray;
				}
				else
				{
					throw new ContextException("Invalid type in a context property.");
				}
			}
		}

		private void _BindCommandOptions(Token[] tokens, int offset, CommandBase command, PropertyContext[] propContexts)
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
					propContext.Property.SetValue(command, _trueBox, null);
				}
				else // TokenKind.Full
				{
					switch (propContext.Attribute.Kind)
					{
						case ParameterKind.String:
							{
								propContext.Property.SetValue(command, token.Value, null);
							}
							break;

						case ParameterKind.Int:
							{
								int parsed;
								if (!int.TryParse(token.Value, out parsed))
								{
									throw new CommandLineException(CommandLineExceptionKind.InvalidValue, token.Param);
								}
								propContext.Property.SetValue(command, parsed, null);
							}
							break;

						case ParameterKind.Enum:
							{
								var att = propContext.Attribute;
								var value = token.Value;
								if (!att.IsFlags)
								{
									if (value.Contains(',') || !att.ValidValues.Contains(value, StringComparer.InvariantCultureIgnoreCase))
									{
										throw new CommandLineException(CommandLineExceptionKind.InvalidValue, token.Param);
									}
									var e = Enum.Parse(propContext.Property.PropertyType, value, true);
									propContext.Property.SetValue(command, e, null);
								}
								else
								{
									var values = value.Split(',');
									int crux = 0;
									foreach (var v in values)
									{
										if (!att.ValidValues.Contains(v, StringComparer.InvariantCultureIgnoreCase))
										{
											throw new CommandLineException(CommandLineExceptionKind.InvalidValue, token.Param);
										}
										var e = Enum.Parse(propContext.Property.PropertyType, v, true);
										crux |= (int)e;
									}
									propContext.Property.SetValue(command, crux, null);
								}
							}
							break;

						case ParameterKind.StringArray:
							{
								var values = token.Value.Split(',');
								propContext.Property.SetValue(command, values, null);
							}
							break;
					}
				}
				propContext.Attribute.IsSet = true;
			}
		}

		private static void _EnsureMandatoriesSet(PropertyContext[] propContexts)
		{
			var unset = default(PropertyContext);
			for (int i = 0; i < propContexts.Length; i++)
			{
				if (propContexts[i].Attribute.IsMandatory && !propContexts[i].Attribute.IsSet)
				{
					unset = propContexts[i];
					break;
				}
			}
			if (!unset.IsEmpty)
			{
				throw new CommandLineException(CommandLineExceptionKind.MissingParameter, unset.Attribute.InternalParameters[0]);
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