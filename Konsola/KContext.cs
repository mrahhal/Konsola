﻿//------------------------------------------------------------------------------
// Copyright (c) 2015, Mohammad Rahhal @mrahhal
//------------------------------------------------------------------------------

using Konsola.Attributes;
using Konsola.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Konsola
{
	public class KContext
	{
		private Type _typeOfString = typeof(string);
		private Type _typeOfInt    = typeof(int);
		private Type _typeOfBool   = typeof(bool);

		private KContextBase             _context;
		private string[]                 _args;
		private Type                     _type;
		private KContextOptionsAttribute _options;

		private KContext(KContextBase context, string[] args)
		{
			_context = context;
			_args = args;
		}

		public static T Parse<T>(string[] args)
			where T : KContextBase
		{
			var context = typeof(T).CreateInstance<KContextBase>();
			return new KContext(context, args)._InternalParse<T>();
		}

		private T _InternalParse<T>()
			where T : KContextBase
		{
			_type = typeof(T);
			_options = _type.GetCustomAttribute<KContextOptionsAttribute>() ?? new KContextOptionsAttribute();

			_InternalWork();
			_InvokeOnParsedMethod();

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
				} else if (arg.StartsWith("-"))
				{
					if (args.Length == i + 1)
					{
						throw new ParsingException(ExceptionKind.MissingData, arg);
					}

					var nextArg = args[++i];
					if (nextArg.StartsWith("-"))
					{
						throw new ParsingException(ExceptionKind.MissingData, arg);
					}

					list.Add(new Token(arg.Substring(1), nextArg));
					j++;
				}
				else
				{
					if (i != 0)
					{
						if (list[j - 1].Kind != TokenKind.Command)
							// TODO: Make a better exception.
							throw new ParsingException(ExceptionKind.IncorrectParameter, arg);
					}

					list.Add(new Token(arg, true));
					j++;
				}
			}

			return list.ToArray();
		}

		private void _Parse(Token[] tokens, int offset, KContextBase context)
		{
			var type = context == _context ? _type : context.GetType();

			var firstToken = tokens[offset];
			if (firstToken.Kind == TokenKind.Command)
			{
				++offset;
				var commandContext = type
					.GetCommandContext(firstToken.Param)
					.FirstOrDefault();

				if (commandContext.IsEmpty)
				{
					// TODO: Make a better exception.
					throw new ParsingException(ExceptionKind.IncorrectParameter, firstToken.Param);
				}

				var newContext = commandContext.Type.CreateInstance<KContextBase>();
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
				else
				{
					throw new ContextException("Invalid type in a KParameter property.");
				}
			}

			for (int i = offset; i < tokens.Length; i++)
			{
				var token = tokens[i];
				var propContext = propContexts.FirstOrDefault(pc => pc.Attribute.InternalParameters.Contains(token.Param));
				if (propContext.IsEmpty)
				{
					throw new ParsingException(ExceptionKind.IncorrectParameter, token.Param);
				}

				if (token.Kind == TokenKind.Partial)
				{
					// TODO: Optimize boxing.
					propContext.Property.SetValue(context, true, null);
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
								throw new ParsingException(ExceptionKind.IncorrectData, token.Param);
							}

							propContext.Property.SetValue(context, parsed, null);
							break;
					}
				}
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