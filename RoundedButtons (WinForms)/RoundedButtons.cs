using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PCAFFINITY
{
    public class RoundedButtons
    {
        public ShadowPosition Btn_ShadowLocation { get; set; } = ShadowPosition.BottomRight;
        public int Btn_CornerRadius { get; set; } = 8;
        public int Btn_LineWidth { get; set; } = 1;
        public ShadowSize Btn_ShadowWidth { get; set; } = ShadowSize.Normal;

        public Color MainLineColor { get; set; } = Color.Black;
        public Color MainShadowColor { get; set; } = Color.DarkGray;
        public Color MainTextColor { get; set; } = Color.Empty;
        public Color MainBGColor { get; set; } = Color.Empty;

        public Color DisabledLineColor { get; set; } = Color.Black;
        public Color DisabledShadowColor { get; set; } = Color.DarkGray;
        public Color DisabledTextColor { get; set; } = Color.Empty;
        public Color DisabledBGColor { get; set; } = Color.Empty;

        public Color HighlightLineColor { get; set; } = Color.Blue;
        public Color HighlightShadowColor { get; set; } = Color.Black;
        public Color HighlightTextColor { get; set; } = Color.Empty;
        public Color HighlightBGColor { get; set; } = Color.Empty;

        public Color ClickLineColor { get; set; } = Color.Black;
        public Color ClickShadowColor { get; set; } = Color.Black;
        public Color ClickTextColor { get; set; } = Color.GhostWhite;
        public Color ClickBGColor { get; set; } = Color.Empty;

        private void RoundedButton_Paint(object sender, PaintEventArgs e, ButtonAction action = ButtonAction.Normal)
        {
            Button b = (Button)sender;

            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;

            StringFormat stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            using Brush clearBrush = new SolidBrush((b.Parent is PictureBox) ? Color.Transparent : ExtensionMethods.GetParentBackColor(b));
            e.Graphics.FillRectangle(clearBrush, new Rectangle(new Point(-1, -1), new Size(b.Size.Width + 2, b.Size.Height + 2)));

            int shadowWidth = (Btn_ShadowWidth == ShadowSize.Thin) ? 1 : (Btn_ShadowWidth == ShadowSize.Normal) ? 2 : (Btn_ShadowWidth == ShadowSize.Thick) ? 3 : 0;

            int width = b.Size.Width - 1;
            int height = b.Size.Height - 1;
            if (Btn_ShadowLocation.ToString().Contains("Bottom") || Btn_ShadowLocation.ToString().Contains("Top"))
            { height -= Btn_LineWidth * 2; height -= shadowWidth; }
            if (Btn_ShadowLocation.ToString().Contains("Right") || Btn_ShadowLocation.ToString().Contains("Left"))
            { width -= Btn_LineWidth * 2; width -= shadowWidth; }

            Size size = new Size(width, height);

            Rectangle shadowShape = new Rectangle(new Point(0, 0), size);
            Rectangle buttonShape = new Rectangle(new Point(0, 0), size);

            if (Btn_ShadowLocation.ToString().Contains("Right"))
            {
                shadowShape.X = shadowWidth;
                buttonShape.X = 0;
            }
            if (Btn_ShadowLocation.ToString().Contains("Bottom"))
            {
                shadowShape.Y = shadowWidth;
                buttonShape.Y = 0;
            }
            if (Btn_ShadowLocation.ToString().Contains("Left"))
            {
                shadowShape.X = 0;
                buttonShape.X = shadowWidth;
            }
            if (Btn_ShadowLocation.ToString().Contains("Top"))
            {
                shadowShape.Y = 0;
                buttonShape.Y = shadowWidth;
            }

            Color switchColor;
            if (shadowWidth > 0)
            {
                //Moved to switch statements to reduce if else statements
                switchColor = action switch
                {
                    ButtonAction.Click => ClickShadowColor,
                    ButtonAction.Disabled => DisabledShadowColor,
                    ButtonAction.Highlight => HighlightShadowColor,
                    ButtonAction.Normal => MainShadowColor,
                    _ => MainShadowColor
                };
                using Brush shadowBrush = new SolidBrush(switchColor);
                using Pen shadowPen = new Pen(shadowBrush, shadowWidth * 2);
                e.Graphics.DrawRoundedRectangle(shadowPen, shadowShape, Btn_CornerRadius);
            }

            switchColor = action switch
            {
                ButtonAction.Click => ClickBGColor,
                ButtonAction.Disabled => DisabledBGColor,
                ButtonAction.Highlight => HighlightBGColor,
                ButtonAction.Normal => MainBGColor,
                _ => MainBGColor
            };
            using Brush backBrush = new SolidBrush(switchColor.IsEmpty ? b.BackColor : switchColor);
            e.Graphics.FillRoundedRectangle(backBrush, buttonShape, Btn_CornerRadius);

            switchColor = action switch
            {
                ButtonAction.Click => ClickLineColor,
                ButtonAction.Disabled => DisabledLineColor,
                ButtonAction.Highlight => HighlightLineColor,
                ButtonAction.Normal => MainLineColor,
                _ => MainLineColor
            };
            using Brush buttonBrush = new SolidBrush(switchColor);
            using Pen buttonPen = new Pen(buttonBrush, Btn_LineWidth);
            e.Graphics.DrawRoundedRectangle(buttonPen, buttonShape, Btn_CornerRadius);

            switchColor = action switch
            {
                ButtonAction.Click => ClickTextColor,
                ButtonAction.Disabled => DisabledTextColor,
                ButtonAction.Highlight => HighlightTextColor,
                ButtonAction.Normal => MainTextColor,
                _ => MainTextColor
            };
            using Brush textBrush = new SolidBrush(switchColor.IsEmpty ? b.ForeColor : switchColor);
            e.Graphics.DrawString(b.Text, b.Font, textBrush, buttonShape, stringFormat);
        }
        private void Button_MouseAction(object sender, EventArgs e, ButtonAction action)
        {
            Button b = (Button)sender;
            b.Paint -= (sender, e) => RoundedButton_Paint(sender, e, ButtonAction.Disabled);
            b.Paint -= (sender, e) => RoundedButton_Paint(sender, e, ButtonAction.Highlight);
            b.Paint -= (sender, e) => RoundedButton_Paint(sender, e, ButtonAction.Click);
            b.Paint -= (sender, e) => RoundedButton_Paint(sender, e, ButtonAction.Normal);

            b.Paint += (sender, e) => RoundedButton_Paint(sender, e, b.Enabled ? action : ButtonAction.Disabled);

            //Forces refresh on Click
            b.Invalidate();
            b.Update();
        }
        public void PaintButton(object sender)
        {
            if (sender is Button b)
            {
                b.FlatAppearance.BorderSize = 0;
                b.FlatAppearance.CheckedBackColor = Color.Transparent;
                b.FlatAppearance.MouseOverBackColor = Color.Transparent;
                b.FlatAppearance.MouseDownBackColor = Color.Transparent;
                b.FlatStyle = FlatStyle.Flat;

                b.Paint += (sender, e) => RoundedButton_Paint(sender, e, ButtonAction.Normal);
                b.EnabledChanged += (sender, e) => Button_MouseAction(sender, e, ButtonAction.Normal);
                b.MouseDown += (sender, e) => Button_MouseAction(sender, e, ButtonAction.Click);
                b.MouseEnter += (sender, e) => Button_MouseAction(sender, e, ButtonAction.Highlight);
                b.MouseLeave += (sender, e) => Button_MouseAction(sender, e, ButtonAction.Normal);
                b.MouseUp += (sender, e) => Button_MouseAction(sender, e, ButtonAction.Highlight);

                //Forces Repaint of control
                b.Invalidate();
                b.Update();
            }
        }
    }

    public enum ButtonAction
    {
        Highlight,
        Click,
        Disabled,
        Normal
    }
    public enum ShadowPosition
    {
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
        TopLeft,
        Top,
        TopRight,
        Right
    }
    public enum ShadowSize
    {
        Thin,
        Normal,
        Thick,
        None
    }

    static class ExtensionMethods
    {
        internal static Color GetParentBackColor(object sender)
        {
            Color c;
            try { c = (sender as dynamic).Parent.BackColor; }
            catch { c = Color.White; }

            if (c == Color.Transparent)
            { try { c = GetParentBackColor((sender as dynamic).Parent); } catch { c = Color.White; } }

            return c;
        }
        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
        {
            using GraphicsPath path = RoundedRect(bounds, cornerRadius);
            graphics.DrawPath(pen, path);
        }

        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            using GraphicsPath path = RoundedRect(bounds, cornerRadius);
            graphics.FillPath(brush, path);
        }
        internal static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath
            { FillMode = FillMode.Winding };

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
