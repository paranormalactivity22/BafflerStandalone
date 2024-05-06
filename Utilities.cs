using System;
using System.IO;
using System.Windows.Forms;

namespace BafflerStandalone
{
	public static class Utilities
	{
		public static int AlignToInt(string alignment)
		{
			switch (alignment)
			{
				case "Center":
					return 0;
				case "Left":
					return 1;
				case "Right":
					return 2;
				case "Top":
					return 4;
				case "TopLeft":
					return 5;
				case "TopRight":
					return 6;
				case "Bottom":
					return 8;
				case "BottomLeft":
					return 9;
				case "BottomRight":
					return 10;
			}
			return 0;
		}

		public static string KeyToString(Keys key)
		{
			switch (key)
			{
				case Keys.D0:
					return "0";
				case Keys.D1:
					return "1";
				case Keys.D2:
					return "2";
				case Keys.D3:
					return "3";
				case Keys.D4:
					return "4";
				case Keys.D5:
					return "5";
				case Keys.D6:
					return "6";
				case Keys.D7:
					return "7";
				case Keys.D8:
					return "8";
				case Keys.D9:
					return "9";
				default:
					switch (key)
					{
					case Keys.OemSemicolon:
						return "; SemiColon";
					case Keys.Oemplus:
						return "+ Plus";
					case Keys.Oemcomma:
						return ", Comma";
					case Keys.OemMinus:
						return "- Minus";
					case Keys.OemPeriod:
						return ". Period";
					case Keys.OemQuestion:
						return "/ Fwd Slash";
					case Keys.Oemtilde:
						return "` Tilde";
					default:
						switch (key)
						{
						case Keys.OemOpenBrackets:
							return "[";
						case Keys.OemPipe:
							return "\\ Back Slash";
						case Keys.OemCloseBrackets:
							return "]";
						case Keys.OemQuotes:
							return "' Quote";
						default:
							return key.ToString();
						}
					}
			}
		}
	}
}
