//------------------------------------------------------------------------------
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
	public static class KContext
	{
		public static T Parse<T>(string[] args) where T : class
		{
			var type = typeof(T).GetTypeInfo();
			_ValidateType(type);

			var context = (T)Activator.CreateInstance(type);
			var classAttribute = (KClassAttribute)type.GetCustomAttribute(typeof(KClassAttribute));
			
			var props = type
				.DeclaredProperties
				.Where(prop => prop.CustomAttributes.Any(att => att.AttributeType == typeof(KParameterAttribute)))
				.ToArray();

			var tokens = _ParseTokens(args);
			_ProcessTokens(tokens, context, props, classAttribute);

			var onParsedMethod = type
				.GetMethods(BindingFlags.NonPublic | BindingFlags.Public)
				.Where(mi => mi.CustomAttributes.Any(att => att.AttributeType == typeof(OnParsedAttribute)))
				.Where(mi => mi.GetParameters().Length == 0)
				.FirstOrDefault();

			if (onParsedMethod != null)
				onParsedMethod.Invoke(context, null);

			return context;
		}

		private static void _ValidateType(Type type)
		{
			var atts = type.CustomAttributes;
			if (!atts.Where(att => att.AttributeType == typeof(KClassAttribute)).Any())
			{
				throw new InvalidOperationException();
			}
		}

		private static Token[] _ParseTokens(string[] args)
		{
			var tokens = new List<Token>();
			var lastToken = default(Token);

			for (int i = 0; i < args.Length; i++)
			{
				var arg = args[i];
				var token = default(Token);
				var kind = default(TokenKind);
				if (arg.StartsWith("-") || arg.StartsWith("--"))
				{
					kind = TokenKind.Param;
				} else
				{
					kind = TokenKind.Data;
				}
				token = new Token(arg, kind, lastToken);
				tokens.Add(token);
				if (lastToken != null)
					lastToken.Next = token;
				lastToken = token;
			}

			return tokens.ToArray();
		}

		private static void _ProcessTokens(Token[] tokens, object context, PropertyInfo[] props, KClassAttribute classAttribute)
		{
			var setProps = new List<PropertyInfo>(props.Length);

			for (int i = 0; i < tokens.Length; i++)
			{
				var token = tokens[i];
				if (token.Kind == TokenKind.Data)
				{
					throw new ParsingException(ExceptionKind.FaultyData, token.Value);
				}
				if (token.Kind == TokenKind.Param)
				{
					var prop = props.Where(pi => pi.GetKAttribute().Parameters.Contains(token.Value)).FirstOrDefault();
					if (prop == null)
					{
						throw new ParsingException(ExceptionKind.IncorrectParameter, token.Value);
					}
					var pa = prop.GetKAttribute();
					switch (pa.Kind)
					{
						case ParameterKind.String:
							{
								var dataToken = tokens[++i];
								prop.SetValue(context, dataToken.Value);
								setProps.Add(prop);
							}
							break;

						case ParameterKind.Int:
							{
								var dataToken = tokens[++i];
								int data;
								if (!int.TryParse(dataToken.Value, out data))
								{
									throw new ParsingException(ExceptionKind.IncorrectData, dataToken.Value);
								}
								prop.SetValue(context, data);
								setProps.Add(prop);
							}
							break;

						case ParameterKind.Switch:
							{
								prop.SetValue(context, true);
								setProps.Add(prop);
							}
							break;
					}
				}
			}

			var missingProp = props.FirstOrDefault(prop => prop.GetKAttribute().IsMandantory && !setProps.Contains(prop));
			if (missingProp != null)
				throw new ParsingException(ExceptionKind.MissingParameter, missingProp.GetKAttribute().Parameters);
		}

		private static KParameterAttribute GetKAttribute(this PropertyInfo pi)
		{
			return (KParameterAttribute)pi.GetCustomAttribute(typeof(KParameterAttribute));
		}
	}
}