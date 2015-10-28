using Konsola.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Konsola.Parser
{
	/// <summary>
	/// A dynamic command line parser.
	/// </summary>
	/// <typeparam name="T">The context.</typeparam>
	public class CommandLineParser<T>
		where T : ContextBase, new()
	{
		private IConsole _console;
		private IErrorFormatter _errorFormatter;
		private IHelpFormatter _helpFormatter;
		private Tokenizer _tokenizer;

		public CommandLineParser()
			: this(null)
		{
		}

		public CommandLineParser(
			IConsole console = null,
			IErrorFormatter errorFormatter = null,
			IHelpFormatter helpFormatter = null,
			Tokenizer tokenizer = null)
		{
			_console = console ?? Consoles.Silent;
			_errorFormatter = errorFormatter ?? new DefaultErrorFormatter();
			_helpFormatter = helpFormatter ?? new DefaultHelpFormatter();
			_tokenizer = tokenizer ?? new DefaultTokenizer();
		}

		/// <summary>
		/// Parses the specified arguments.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception>
		/// <exception cref="ContextException">The context is invalid.</exception>
		/// <exception cref="CommandLineException">The args did not correctly match the expectations of the context.</exception>
		/// <returns>The parsing result.</returns>
		public ParsingResult<T> Parse(string[] args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}

			var tokens = _tokenizer.Process(args);
			var metadata = MetadataProviders.Current.GetFor<T>();

			var wrapper = CreateWrapper(metadata);
			ValidateWrapper(wrapper);
			var parsingContext = new ParsingContext
			{
				Wrapper = wrapper,
				Tokens = tokens,
			};

			try
			{
				return ParseCore(parsingContext);
			}
			catch (CommandLineException ex)
			{
				_console.WriteLine(WriteKind.Error, _errorFormatter.Format(ex));
				return new ParsingResult<T>()
				{
					Kind = ParsingResultKind.Failure,
					Exception = ex,
				};
			}
		}

		public HelpContext GenerateHelpContext()
		{
			return HelpContextGenerator.Generate(typeof(T), _tokenizer);
		}

		public CommandHelpContext GenerateCommandHelpContext(Type type)
		{
			return HelpContextGenerator.GenerateForCommand(type, _tokenizer);
		}

		private ParsingResult<T> ParseCore(ParsingContext c)
		{
			var cw = c.Wrapper;
			var tokens = c.Tokens;

			var machine = new StateMachine<Token>(tokens);
			if (machine.PeekNext() == null)
			{
				// This is an empty invocation of the program.
				if (cw.Options.HandleEmptyInvocationAsHelp)
				{
					PrintHelpForDefaultCommand();
					return new ParsingResult<T>()
					{
						Kind = ParsingResultKind.Handled,
					};
				}
			}

			var commandMetadata = FindCommandMetadata(cw, machine);
			if (commandMetadata == null)
			{
				PrintHelpForDefaultCommand();
				return new ParsingResult<T>()
				{
					Kind = ParsingResultKind.Handled,
				};
			}
			cw.Command = cw.Context.Command = commandMetadata.Type.CreateInstance<CommandBase>();
			ValidateParameters(commandMetadata.Properties);

			if (IsHelpRequested(machine))
			{
				if (commandMetadata.Type == cw.DefaultCommand.Command)
				{
					PrintHelpForDefaultCommand();
				}
				else
				{
					var commandHelpContext = GenerateCommandHelpContext(commandMetadata.Type);
					_helpFormatter.FormatForCommand(commandHelpContext, _console);
				}
				return new ParsingResult<T>()
				{
					Kind = ParsingResultKind.Handled,
				};
			}

			// Throws CommandLineException on errors.
			Bind(machine, commandMetadata, cw);

			// If we're here, binding worked and the cw.Context is now bound and ready. Success.
			return new ParsingResult<T>()
			{
				Context = cw.Context,
				Kind = ParsingResultKind.Success,
			};
		}

		private void ValidateParameters(IEnumerable<PropertyMetadata> properties)
		{
			foreach (var prop in properties)
			{
				var attribute = prop.Attributes.FirstOrDefaultOfRealType<ParameterAttribute>();
				if (attribute != null)
				{
					ValidationHelper.ValidateParameterAttribute(attribute);
				}
			}
		}

		private void Bind(StateMachine<Token> machine, ObjectMetadata commandMetadata, ContextWrapper cw)
		{
			var sources = CreateSources(machine);
			var targets = CreateTargets(commandMetadata.Properties, cw.Command);

			var bindingContext = new BindingContext()
			{
				Sources = sources,
				Targets = targets,
			};
			Binder.Bind(bindingContext);

			ValidateTargets(targets);
		}

		private DataSource[] CreateSources(StateMachine<Token> machine)
		{
			var list = new List<DataSource>();
			while (machine.PeekNext() != null)
			{
				var current = machine.Next();
				Debug.Assert(current.Kind != TokenKind.Command);

				var source = new DataSource();
				var identifier = _tokenizer.ExtractIdentifier(current.Kind, current.Value);
				switch (current.Kind)
				{
					case TokenKind.Option:
						{
							source.FullIdentifier = current.Value;
							source.Identifier = identifier;
							source.Kind = RawTokenKind.Option;
							if (machine.HasNext && machine.PeekNext().Kind == TokenKind.Data)
							{
								var next = machine.Next();
								source.Value = next.Value;
							}
						}
						break;
					case TokenKind.Switch:
						{
							source.FullIdentifier = current.Value;
							source.Identifier = identifier;
							source.Kind = RawTokenKind.Switch;
						}
						break;
					case TokenKind.Data:
						{
							source.Value = current.Value;
							source.Kind = RawTokenKind.Raw;
						}
						break;
				}

				list.Add(source);
			}
			return list.ToArray();
		}

		private PropertyTarget[] CreateTargets(IEnumerable<PropertyMetadata> properties, Object commandObject)
		{
			return
				properties
				.Select(p => new
				{
					Attribute = p.Attributes.FirstOrDefaultOfRealType<ParameterAttribute>(),
					Property = p,
				})
				.Where(pa => pa.Attribute != null)
				.Select(pa => new PropertyTarget(commandObject)
				{
					Attribute = pa.Attribute,
					Metadata = pa.Property,
				})
				.ToArray();
		}

		private void ValidateTargets(PropertyTarget[] targets)
		{
			foreach (var target in targets)
			{
				if (target.Attribute.IsMandatory && !target.IsSet)
				{
					throw new CommandLineException(CommandLineExceptionKind.MissingParameter, target.Attribute.Names);
				}
			}
		}

		private void PrintHelpForDefaultCommand()
		{
			var helpContext = GenerateHelpContext();
			_helpFormatter.Format(helpContext, _console);
		}

		private bool IsHelpRequested(StateMachine<Token> machine)
		{
			var helpRequested = false;
			machine.VisitAllNext((i, t) =>
				{
					if (string.Equals(t.Value, "--help", StringComparison.OrdinalIgnoreCase) ||
						string.Equals(t.Value, "--h", StringComparison.OrdinalIgnoreCase))
					{
						helpRequested = true;
					}
					return t;
				});
			return helpRequested;
		}

		private ObjectMetadata FindCommandMetadata(ContextWrapper cw, StateMachine<Token> machine)
		{
			if (machine.PeekNext() == null || machine.PeekNext().Kind != TokenKind.Command)
			{
				// Empty machine or first token is not a command token.
				// We want the default command.
				if (cw.DefaultCommand == null)
				{
					return null;
				}
				return MetadataProviders.Current.GetFor(cw.DefaultCommand.Command);
			}
			var currentMetadata = default(ObjectMetadata);
			while (machine.PeekNext() != null && machine.PeekNext().Kind == TokenKind.Command)
			{
				var commandToken = machine.Next();
				var metadata = (currentMetadata ?? cw.Metadata).FindCommandMetadataWithName(commandToken.Value);
				if (metadata == null)
				{
					// Could not find a command by that name.
					if (currentMetadata == null)
					{
						// Neither was a previous command found.
						return null;
					}
					else
					{
						// A previous command exists.
						// Replace all next command tokens with data tokens (to correctly handle positional args).
						machine.Previous();
						machine.VisitAllNext((i, t) =>
							{
								if (t.Kind == TokenKind.Command)
								{
									return new DataToken(t.Value);
								}
								return t;
							});
						return currentMetadata;
					}
				}
				currentMetadata = metadata;
			}
			return currentMetadata;
		}

		private ContextWrapper CreateWrapper(ObjectMetadata metadata)
		{
			var options =
				metadata.Attributes.FirstOrDefaultOfRealType<ContextOptionsAttribute>() ??
				new ContextOptionsAttribute();
			var defaultCommand = metadata.Attributes.FirstOrDefaultOfRealType<DefaultCommandAttribute>();
			var includeCommands = metadata.Attributes.FirstOrDefaultOfRealType<IncludeCommandsAttribute>();

			return new ContextWrapper
			{
				Context = new T(),
				Metadata = metadata,
				Options = options,
				DefaultCommand = defaultCommand,
				IncludeCommands = includeCommands,
			};
		}

		private void ValidateWrapper(ContextWrapper wrapper)
		{
			if (wrapper.DefaultCommand == null && wrapper.IncludeCommands == null)
			{
				throw new ContextException(
					"The context must have DefaultCommandAttribute, IncludeCommandsAttribute, or both.");
			}

			if (wrapper.DefaultCommand != null)
			{
				ValidationHelper.ValidateCommandType(wrapper.DefaultCommand.Command);
			}

			if (wrapper.IncludeCommands != null)
			{
				if (wrapper.IncludeCommands.Commands == null)
				{
					throw new ContextException("IncludeCommandsAttribute.Commands cannot be null.");
				}
				foreach (var command in wrapper.IncludeCommands.Commands)
				{
					ValidationHelper.ValidateCommandType(command);
				}
				if (wrapper.DefaultCommand != null &&
					wrapper.IncludeCommands.Commands.Any(t => wrapper.DefaultCommand.Command == t))
				{
					throw new ContextException("The default command should not be included in IncludeCommandsAttribute.");
				}
			}
		}

		private class ParsingContext
		{
			public ContextWrapper Wrapper { get; set; }
			public Token[] Tokens { get; set; }
		}

		private class ContextWrapper
		{
			public T Context { get; set; }
			public CommandBase Command { get; set; }
			public ObjectMetadata Metadata { get; set; }
			public ContextOptionsAttribute Options { get; set; }
			public DefaultCommandAttribute DefaultCommand { get; set; }
			public IncludeCommandsAttribute IncludeCommands { get; set; }
		}
	}

	[DebuggerDisplay("Kind={Kind}, Context={Context}")]
	public class ParsingResult<T>
	{
		public ParsingResultKind Kind { get; set; }
		public T Context { get; set; }
		public CommandLineException Exception { get; set; }
	}

	public enum ParsingResultKind
	{
		Success,
		Handled,
		Failure,
	}
}