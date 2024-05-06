using System;
using System.Drawing;
using System.Windows.Forms;

namespace BafflerStandalone
{
	internal class CustomToolStripButton : ToolStripButton
	{
		protected override void OnPaint(PaintEventArgs pe)
		{
			Graphics graphics = pe.Graphics;
			RoundedRectangle.Create(this.Bounds, 5, RoundedRectangle.RectangleCorners.None);
		}
	}
}
