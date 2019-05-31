using System.Linq;

namespace Kontur.ImageTransformer
{
	/// <summary>
	/// Координаты области изображения.
	/// </summary>
	internal class ImageCoordinates
	{
		public int X => _x;
		private int _x;

		public int Y => _y;
		private int _y;

		public int Width => _width;
		private int _width;

		public int Height => _height;
		private int _height;


		public static bool TryParse(string coordsValue, out ImageCoordinates coords)
		{
			coords = default;

			string[] args = coordsValue.Split(',').ToArray();
			if (args.Length != 4)
				return false;

			bool parsed = int.TryParse(args[0], out coords._x) &&
			              int.TryParse(args[1], out coords._y) &&
			              int.TryParse(args[2], out coords._width) &&
			              int.TryParse(args[3], out coords._height);
			return parsed;
		}
	}
}
