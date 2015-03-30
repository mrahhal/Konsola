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
	public class KContext
	{
		private string[] _args;
		private TypeInfo _type;
		private object _context;
		private KClassAttribute _classAttribute;
		private PropertyInfo[] _props;
		private Dictionary<PropertyInfo, KParameterAttribute> _propToParamAtt = new Dictionary<PropertyInfo, KParameterAttribute>();

		private KContext(string[] args)
		{
			_args = args;
		}

		public static T Parse<T>(string[] args) where T : class
		{
			return new KContext(args)._InternalParse<T>();
		}

		private T _InternalParse<T>()
		{
			_type = typeof(T).GetTypeInfo();
			_ValidateType(_type);

			_context = (T)Activator.CreateInstance(_type);
			_classAttribute = (KClassAttribute)_type.GetCustomAttribute<KClassAttribute>();

			_props = _type
				.DeclaredProperties
				.Where(prop => prop.CustomAttributes.Any(att => att.AttributeType == typeof(KParameterAttribute)))
				.ToArray();

			foreach (var prop in _props)
			{
				var att = GetKAttribute(prop);
				var type = prop.PropertyType;
				
				if (type == typeof(string))
				{
					att.Kind = ParameterKind.String;
				} else if (type == typeof(int))
				{
					att.Kind = ParameterKind.Int;
				} else if (type == typeof(bool))
				{
					att.Kind = ParameterKind.Switch;
				} else
				{
					throw new ContextException("Invalid type in a KParameter property.");
				}
			}

			var tokens = _ParseTokens(_args);
			try
			{
				_ProcessTokens(tokens);
			}
			catch (ParsingException ex)
			{
				if (!_classAttribute.ExitOnException)
				{
					throw;
				}

				Console.WriteLine(ex.Message);
				Environment.Exit(1);
			}

			var onParsedMethod = _type
				.DeclaredMethods
				.Where(mi => mi.CustomAttributes.Any(att => att.AttributeType == typeof(OnParsedAttribute)))
				.Where(mi => mi.GetParameters().Length == 0)
				.FirstOrDefault();

			if (onParsedMethod != null)
				onParsedMethod.Invoke(_context, null);

			var context = _context;
			return (T)context;
		}

		private void _ValidateType(Type type)
		{
			var atts = type.CustomAttributes;
			if (!atts.Where(att => att.AttributeType == typeof(KClassAttribute)).Any())
			{
				throw new InvalidOperationException();
			}
		}

		private Token[] _ParseTokens(string[] args)
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
					arg = arg.TrimStart('-');
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

		private void _ProcessTokens(Token[] tokens)
		{
			var setProps = new List<PropertyInfo>(_props.Length);

			for (int i = 0; i < tokens.Length; i++)
			{
				var token = tokens[i];
				if (token.Kind == TokenKind.Data)
				{
					throw new ParsingException(ExceptionKind.FaultyData, token.Value);
				}
				if (token.Kind == TokenKind.Param)
				{
					var prop = _props.Where(pi => GetKAttribute(pi).Parameters.Contains(token.Value)).FirstOrDefault();
					if (prop == null)
					{
						throw new ParsingException(ExceptionKind.IncorrectParameter, token.Value);
					}
					var pa = GetKAttribute(prop);
					switch (pa.Kind)
					{
						case ParameterKind.String:
							{
								var dataToken = tokens[++i];
								prop.SetValue(_context, dataToken.Value);
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
								prop.SetValue(_context, data);
								setProps.Add(prop);
							}
							break;

						case ParameterKind.Switch:
							{
								prop.SetValue(_context, true);
								setProps.Add(prop);
							}
							break;
					}
				}
			}

			var missingProp = _props.FirstOrDefault(prop => GetKAttribute(prop).IsMandantory && !setProps.Contains(prop));
			if (missingProp != null)
				throw new ParsingException(ExceptionKind.MissingParameter, GetKAttribute(missingProp).Parameters);
		}

		private KParameterAttribute GetKAttribute(PropertyInfo pi)
		{
			var att = default(KParameterAttribute);
			if (_propToParamAtt.TryGetValue(pi, out att))
			{
				return att;
			}

			att = pi.GetCustomAttribute<KParameterAttribute>();
			_propToParamAtt.Add(pi, att);
			return att;
		}
	}
}