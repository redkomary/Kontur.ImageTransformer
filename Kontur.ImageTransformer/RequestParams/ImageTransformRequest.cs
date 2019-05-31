using System;
using System.Linq;

namespace Kontur.ImageTransformer
{
	/// <summary>
	/// Запрос на обработку изображения.
	/// </summary>
	internal class ImageTransformRequest
	{
		public string Action { get; set; }

		public TransformMethod Transform { get; set; }

		public ImageCoordinates Coords { get; set; }
		
		public static bool TryParse(Uri uri, out ImageTransformRequest requestParams)
		{
			requestParams = default;

			string[] uriSegments = uri.Segments
				.Skip(1)
				.Select(x => x.Replace("/", ""))
				.ToArray();

			if (uriSegments.Length != 3)
				return false;

			if (uriSegments[0] != "process")
				return false;

			if (!EnumExtensions.TryGetValueFromDescription(uriSegments[1], out TransformMethod transform))
				return false;

			if (!ImageCoordinates.TryParse(uriSegments[2], out ImageCoordinates coords))
				return false;

			requestParams = new ImageTransformRequest
			{
				Action = uriSegments[0],
				Transform = transform,
				Coords = coords,
			};
			return true;
		}
	}
}
