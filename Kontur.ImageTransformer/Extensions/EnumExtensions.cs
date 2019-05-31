using System;
using System.ComponentModel;

namespace Kontur.ImageTransformer
{
	internal class EnumExtensions
	{
		/// <summary>
		/// Возвращает значение перечисления по его <<see cref="DescriptionAttribute"/>
		/// </summary>
		public static bool TryGetValueFromDescription<T>(string description, out T value)
			where T : Enum
		{
			var typeOfEnum = typeof(T);
			foreach (var field in typeOfEnum.GetFields())
			{
				var descriptionAttribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
				if (descriptionAttribute == null || descriptionAttribute.Description != description)
					continue;

				value = (T)field.GetValue(null);
				return true;
			}

			value = default;
			return false;
		}
	}
}
