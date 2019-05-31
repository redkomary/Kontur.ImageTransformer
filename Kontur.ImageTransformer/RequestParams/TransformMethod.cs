using System.ComponentModel;

namespace Kontur.ImageTransformer
{
	/// <summary>
	/// Способ преобразования изображения.
	/// </summary>
	internal enum TransformMethod
	{
		/// <summary>
		/// Поворот по часовой стрелке.
		/// </summary>
		[Description("rotate-cw")]
		RotateCw,

		/// <summary>
		/// Поворот против часовой стрелки.
		/// </summary>
		[Description("rotate-ccw")]
		RotateCcw,

		/// <summary>
		/// Отражение по горизонтали.
		/// </summary>
		[Description("flip-h")]
		FlipH,

		/// <summary>
		/// Отражение по вертикали.
		/// </summary>
		[Description("flip-v")]
		FlipV
	}
}
