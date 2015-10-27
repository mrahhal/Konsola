using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Konsola.Parsing
{
	/// <summary>
	/// A dynamic command line parser.
	/// </summary>
	public class CommandLineParser
	{
		private ContextBase _context;
		private IConsole _console;
		private string[] _args;
		private Type _contextType;
		private ContextOptionsAttribute _options;

		private CommandLineParser(ContextBase context, string[] args, IConsole console)
		{
			_context = context;
			_args = args;
			_console = console;
		}

		/// <summary>
		/// Parses the provided args into a new context instance of <typeparamref name="T"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception>
		/// <exception cref="ContextException">The context is invalid.</exception>
		/// <exception cref="CommandLineException">The args did not correctly match the expectations of the context.</exception>
		/// <returns>The new context containing the parsed options, or null if it has been handled.</returns>
		public static T Parse<T>(string[] args, IConsole console = null)
			where T : ContextBase, new()
		{
			if (args == null)
				throw new ArgumentNullException("args");

			console = console ?? Consoles.Silent;

			var context = new T();
			return new CommandLineParser(context, args, console)._InternalParse<T>();
		}

		private T _InternalParse<T>()
			where T : ContextBase
		{
			_contextType = typeof(T);
			_options = _context.Options = _contextType.GetCustomAttribute<ContextOptionsAttribute>() ?? new ContextOptionsAttribute();
			_context.DefaultCommandAttribute = _contextType.GetCustomAttribute<DefaultCommandAttribute>();
			_context.IncludeCommandsAttribute = _contextType.GetCustomAttribute<IncludeCommandsAttribute>();
			if (_context.IncludeCommandsAttribute != null)
				_ValidateIncludedCommands(_context.IncludeCommandsAttribute.Commands);

			if (_InternalWork())
				return null;
			if (_options.InvokeMethods)
			{
				_InvokeOnParsedMethod();
			}

			return (T)_context;
		}

		// Returns if handled.
		private bool _InternalWork()
		{
			try
			{
				var tokens = _ParseTokens(_args);
				return _ProcessTokens(tokens, 0, _context);
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
					list.Add(new Token(arg, true));
					j++;
				}
			}

			return list.ToArray();
		}

		// Returns if handled.
		private bool _ProcessTokens(Token[] tokens, int offset, ContextBase context)
		{
			if (_TryHandleNoArgs(context, _args))
				return true;

			var commandType = _FindTargetCommandType(tokens, ref offset, _contextType);
			if (commandType == null)
			{
				// Resolve the default command.
				var defaultCommandAttribute = context.DefaultCommandAttribute;
				if (defaultCommandAttribute == null)
				{
					// No command has been specified on the command line
					// and no default command has been registered.
					if (_TryHandleHelp(tokens, context))
					{
						return true;
					}
					else
					{
						throw new CommandLineException(CommandLineExceptionKind.NoCommand, "");
					}
				}
				commandType = defaultCommandAttribute.DefaultCommand;
			}

			var command = commandType.CreateInstance<CommandBase>();
			command.CommandAttribute = commandType.GetCustomAttribute<CommandAttribute>();
			command.IncludeCommandsAttribute = commandType.GetCustomAttribute<IncludeCommandsAttribute>();
			if (command.IncludeCommandsAttribute != null)
				_ValidateIncludedCommands(command.IncludeCommandsAttribute.Commands);
			context.Command = command;
			command.ContextBase = context;
			var parameterContexts = commandType.GetPropertyContexts().ToArray();
			_InitializePropertyAttributes(parameterContexts);

			if (_TryHandleHelp(tokens, command, parameterContexts))
			{
				return true;
			}

			// Bind the command.
			_BindCommandOptions(tokens, offset, command, parameterContexts);

			// Check for mandatory params that have not been set.
			_EnsureMandatoriesSet(parameterContexts);
			return false;
		}

		private bool _TryHandleNoArgs(ContextBase context, string[] args)
		{
			if (args.Length == 0)
			{
				var helpInfo = new HelpInfo(context);
				_PrintHelp(helpInfo);
				return true;
			}
			return false;
		}

		private bool _TryHandleHelp(Token[] tokens, ContextBase context)
		{
			if (tokens.Any(t => t.Kind == TokenKind.Partial
				&& (t.Param.ToLower() == "h" || t.Param.ToLower() == "help")))
			{
				var helpInfo = new HelpInfo(context);
				_PrintHelp(helpInfo);
				return true;
			}

			return false;
		}

		private bool _TryHandleHelp(Token[] tokens, CommandBase command, ParameterContext[] parameterContexts)
		{
			if (tokens.Any(t => t.Kind == TokenKind.Partial
				&& (t.Param.ToLower() == "h" || t.Param.ToLower() == "help")))
			{
				var helpInfo = new HelpInfo(command);
				_PrintHelp(helpInfo);
				return true;
			}

			return false;
		}

		private void _PrintHelp(HelpInfo helpInfo)
		{
			if (helpInfo.ProgramDescription != null)
			{
				_console.WriteLine(helpInfo.ProgramDescription);
				_console.WriteLine();
			}
			if (helpInfo.Commands != null)
			{
				_PrintCommands(helpInfo.Commands);
			}
			if (helpInfo.Parameters != null)
			{
				_PrintParameters(helpInfo.Parameters);
			}
		}

		private void _PrintCommands(CommandAttribute[] commands)
		{
			_console.WriteLine("commands:");
			_console.WriteLine("---------");
			foreach (var c in commands)
			{
				_console.Write("* " + c.Name);
				if (!string.IsNullOrWhiteSpace(c.Description))
				{
					_console.Write(" - ");
					_console.WriteLine(c.Description);
				} else
				{
					_console.WriteLine();
				}
			}
			_console.WriteLine();
		}

		private void _PrintParameters(ParameterAttribute[] parameters)
		{
			_console.WriteLine("parameters:");
			_console.WriteLine("-----------");
			foreach (var p in parameters)
			{
				_console.Write("* " + p.Parameters);
				if (p.Position != 0)
				{
					_console.Write("[" + p.Position + "]");
				}
				if (!string.IsNullOrWhiteSpace(p.Description))
				{
					_console.Write(" - ");
					_console.WriteLine(p.Description);
				} else
				{
					_console.WriteLine();
				}
			}
		}

		private Type _FindTargetCommandType(Token[] tokens, ref int offset, Type contextType, Type lastCommandType = null)
		{
			var type = lastCommandType ?? contextType;

			if (offset == tokens.Length)
			{
				throw new CommandLineException(CommandLineExceptionKind.Invalid, null);
			}
			var token = tokens[offset];
			if (token.Kind != TokenKind.Word)
			{
				return null;
			}

			++offset;

			// WARN: There are a lot of potential problems in positional
			// params for commands that contain subcommands.

			var cc = type.GetCommandContextOrDefault(token.Param);
			if (cc == null)
			{
				return lastCommandType;
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

				if (propType == typeof(string))
				{
					att.Kind = ParameterKind.String;
				}
				else if (propType == typeof(int))
				{
					att.Kind = ParameterKind.Int;
				}
				else if (propType == typeof(bool))
				{
					att.Kind = ParameterKind.Switch;
				}
				else if (propType.IsEnum)
				{
					att.Kind = ParameterKind.Enum;
					if (propType.IsAttributeDefined<FlagsAttribute>())
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
				else if (propType == typeof(string[]))
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
			var position = 1;
			for (int i = offset; i < tokens.Length; i++)
			{
				var token = tokens[i];
				var parameterContext = token.Kind == TokenKind.Word ?
					parameterContexts.FirstOrDefault(pc => pc.ParameterAttribute.Position == position) :
					parameterContexts.FirstOrDefault(pc => pc.ParameterAttribute.InternalParameters.Contains(token.Param));
				if (parameterContext == null)
				{
					throw new CommandLineException(CommandLineExceptionKind.InvalidParameter, token.Param);
				}

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

				// Bind the values.
				if (token.Kind == TokenKind.Partial)
				{
					if (parameterContext.Property.PropertyType != typeof(bool))
						throw new CommandLineException(CommandLineExceptionKind.InvalidParameter, token.Param);
					parameterContext.Property.SetValue(command, Boxes.True, null);
				}
				else // TokenKind.Full || TokenKind.Word
				{
					if (token.Kind == TokenKind.Word && parameterContext == null)
					{
						// Binding failed for the current positional param.
						throw new CommandLineException(CommandLineExceptionKind.InvalidPositionalParameters, null);
					}
					if (token.Kind == TokenKind.Word)
					{
						// Transform the token to a full kind.
						token = new Token(parameterContext.ParameterAttribute.InternalParameters[0], token.Param);
						position++;
					}
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

		private void _ValidateIncludedCommands(Type[] types)
		{
			if (types == null || types.Length == 0)
				return;
			foreach (var type in types)
			{
				if (type.GetCustomAttribute<CommandAttribute>() == null)
				{
					throw new ContextException("A command doesn't have a CommandAttribute type: " + type.Name);
				}
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
						 .Where(mi => mi.GetParameters().Length == 0
							 && mi.IsAttributeDefined<OnParsedAttribute>())
						 .FirstOrDefault();

			if (onParsedMethod != null)
			{
				onParsedMethod.Invoke(_context, null);
			}
		}
	}
}