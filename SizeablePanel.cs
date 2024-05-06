using System;
using System.Drawing;
using System.Windows.Forms;

namespace BafflerStandalone
{
	public class SizeablePanel : Panel
	{
		private const int cGripSize = 20;
		private bool mDragging;
		private Point mDragPos;
		private SizePanelPosition mPosResize;
		
		public SizeablePanel()
		{
			this.DoubleBuffered = true;
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			this.BackColor = Color.White;
			this.mPosResize = SizePanelPosition.None;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, new Rectangle(base.ClientSize.Width - 20, base.ClientSize.Height - 20, 20, 20));
			base.OnPaint(e);
		}

		private bool IsOnGrip(Point pos)
		{
			if (pos.X >= base.ClientSize.Width - 20 && pos.Y >= base.ClientSize.Height - 20)
			{
				this.mPosResize = SizePanelPosition.BottomRight;
				return true;
			}
			if (pos.X <= 20 && pos.Y <= 20)
			{
				this.mPosResize = SizePanelPosition.TopLeft;
				return true;
			}
			this.mPosResize = SizePanelPosition.None;
			return false;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.mDragging = this.IsOnGrip(e.Location);
			this.mDragPos = e.Location;
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			this.mDragging = false;
			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (this.mDragging)
			{
				if (this.mPosResize == SizePanelPosition.BottomRight)
				{
					int num = base.Width + e.X - this.mDragPos.X;
					int num2 = base.Height + e.Y - this.mDragPos.Y;
					if (num <= base.Parent.ClientSize.Width && num2 <= base.Parent.ClientSize.Height)
					{
						base.Size = new Size(num, num2);
					}
				}
				this.mDragPos = e.Location;
			}
			else if (this.IsOnGrip(e.Location))
			{
				this.Cursor = Cursors.SizeNWSE;
			}
			else
			{
				this.Cursor = Cursors.Default;
			}
			base.OnMouseMove(e);
		}
	}
}
