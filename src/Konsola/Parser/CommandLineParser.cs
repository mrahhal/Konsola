using Konsola.Metadata;
using System;
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
		private Tokenizer _tokenizer;

		public CommandLineParser()
			//: this(null)
		{
		}

		public CommandLineParser(
			IConsole console = null,
			IErrorFormatter errorFormatter = null,
			Tokenizer tokenizer = null)
		{
			_console = console ?? Consoles.Silent;
			_errorFormatter = errorFormatter ?? new DefaultErrorFormatter();
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

		private ParsingResult<T> ParseCore(ParsingContext c)
		{
			var cw = c.Wrapper;
			var tokens = c.Tokens;

			throw new NotImplementedException();
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