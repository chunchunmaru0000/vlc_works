﻿using System;
using System.Linq;
using System.Reflection;

namespace vlc_works
{
	public class StringValueAttribute : Attribute
	{
		public string Value { get; }

		public StringValueAttribute(string value)
		{
			Value = value;
		}
	}

    public class ArrayValueAttribute : Attribute
    {
        public string[] Value { get; }

        public ArrayValueAttribute(string[] value)
        {
            Value = value;
        }
    }

    public static class EnumExtensions
	{
		public static string View(this Enum value)
		{
			Type type = value.GetType();
			FieldInfo fieldInfo = type.GetField(value.ToString());
			StringValueAttribute[] attribs = 
				fieldInfo
					.GetCustomAttributes(typeof(StringValueAttribute), false)
					as StringValueAttribute[];
			return attribs.Length > 0 ? attribs[0].Value : null;
		}

        public static string[] Views(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            ArrayValueAttribute[] attribs =
                fieldInfo
                    .GetCustomAttributes(typeof(ArrayValueAttribute), false)
                    as ArrayValueAttribute[];
            return attribs.Length > 0 ? attribs[0].Value : new string[0];
        }
    }
}
