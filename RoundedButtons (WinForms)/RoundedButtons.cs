using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace PCAFFINITY
{
    public class RoundedButtons : IDisposable
    {
        public ShadowPosition Btn_ShadowLocation { get; set; } = ShadowPosition.BottomRight;
        public int Btn_CornerRadius { get; set; } = 8;
        public int Btn_LineWidth { get; set; } = 1;
        public ShadowSize Btn_ShadowWidth { get; set; } = ShadowSize.Normal;

        public Color MainLineColor { get; set; } = Color.Black;
        public Color MainShadowColor { get; set; } = Color.DarkGray;
        public Color MainTextColor { get; set; } = Color.Empty;
        public Color MainBGColor { get; set; } = Color.Empty;

        public Color DisabledLineColor { get; set; } = Color.LightGray;
        public Color DisabledShadowColor { get; set; } = Color.DarkGray;
        public Color DisabledTextColor { get; set; } = Color.Gray;
        public Color DisabledBGColor { get; set; } = Color.Empty;

        public Color HighlightLineColor { get; set; } = Color.Blue;
        public Color HighlightShadowColor { get; set; } = Color.Black;
        public Color HighlightTextColor { get; set; } = Color.Empty;
        public Color HighlightBGColor { get; set; } = Color.Empty;

        public Color ClickLineColor { get; set; } = Color.Black;
        public Color ClickShadowColor { get; set; } = Color.Black;
        public Color ClickTextColor { get; set; } = Color.GhostWhite;
        public Color ClickBGColor { get; set; } = Color.Empty;

        private readonly Dictionary<Button, Button> ButtonCopies = new Dictionary<Button, Button>();
        public bool IsDisposed = false;

        private event MouseEventHandler MouseEventClick;
        private event MouseEventHandler MouseEventHighlight;
        private event EventHandler EventHighlight;
        private event EventHandler EventNormal;

        #region Painting Events
        public RoundedButtons()
        {
            MouseEventClick = (sender, e) => Button_MouseAction(sender, e, ButtonAction.Click);
            MouseEventHighlight = (sender, e) => Button_MouseAction(sender, e, ButtonAction.Highlight);
            EventHighlight = (sender, e) => Button_MouseAction(sender, e, ButtonAction.Highlight);
            EventNormal = (sender, e) => Button_MouseAction(sender, e, ButtonAction.Normal);
        }
        #pragma warning disable IDE0060 // Remove unused parameter
        private void Button_MouseAction(object sender, EventArgs e, ButtonAction action)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            Button b = (Button)sender;
            b.Paint -= B_Paint_Disable;
            b.Paint -= B_Paint_Click;
            b.Paint -= B_Paint_Hightlight;
            b.Paint -= B_Paint;

            b.Paint += action switch
            {
                ButtonAction.Click => B_Paint_Click,
                ButtonAction.Disabled => B_Paint_Disable,
                ButtonAction.Highlight => B_Paint_Hightlight,
                _ => B_Paint
            };

            b.Refresh();
        }
        private void B_Paint(object sender, PaintEventArgs e)
        { RoundedButton_Paint(sender, e, ButtonAction.Normal); }
        private void B_Paint_Hightlight(object sender, PaintEventArgs e)
        { RoundedButton_Paint(sender, e, ButtonAction.Highlight); }
        private void B_Paint_Click(object sender, PaintEventArgs e)
        { RoundedButton_Paint(sender, e, ButtonAction.Click); }
        private void B_Paint_Disable(object sender, PaintEventArgs e)
        { RoundedButton_Paint(sender, e, ButtonAction.Disabled); }
        #endregion

        public void PaintButton(object sender)
        {
            if (sender is Button b)
            {
                ButtonCopies.Add(b, b.Clone());

                //Following configuration fixes transparency issues.
                //Fixes issue with Transparency over Pictureboxes.
                b.TabStop = false;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 255, 255, 255);
                b.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 255, 255, 255);
                b.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
                b.BackColor = Color.FromArgb(0, 255, 255, 255);

                //Clear text field to prevent double drawn text with Transparency.
                b.Text = "";

                b.Paint += B_Paint;
                b.EnabledChanged += EventNormal;
                b.MouseEnter += EventHighlight;
                b.MouseLeave += EventNormal;
                b.MouseDown += MouseEventClick;
                b.MouseUp += MouseEventHighlight;
                b.TextChanged += B_TextChanged;

                b.Refresh();
            }
        }
        private void B_TextChanged(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            ButtonCopies.TryGetValue(b, out Button copy);
            copy.Text = b.Text;

            b.TextChanged -= B_TextChanged;
            b.Text = "";
            b.TextChanged += B_TextChanged;
        }

        private void RoundedButton_Paint(object sender, PaintEventArgs e, ButtonAction action = ButtonAction.Normal)
        {
            Button b = (Button)sender;
            if (b.Enabled == false) { action = ButtonAction.Disabled; }

            //================Setup================
            
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;

            StringFormat stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            int shadowWidth = (Btn_ShadowWidth == ShadowSize.Thin) ? 1 : (Btn_ShadowWidth == ShadowSize.Normal) ? 2 : (Btn_ShadowWidth == ShadowSize.Thick) ? 3 : 0;

            Rectangle[] rect = CalculateRects(b, shadowWidth);
            ButtonCopies.TryGetValue(b, out Button copy);
            //=====================================

            //===============Colors================
            Color shadowColor = action switch
            {
                ButtonAction.Click => ClickShadowColor,
                ButtonAction.Disabled => DisabledShadowColor,
                ButtonAction.Highlight => HighlightShadowColor,
                ButtonAction.Normal => MainShadowColor,
                _ => MainShadowColor
            };
            shadowColor = shadowColor == Color.Transparent ? Color.FromArgb(0, 255, 255, 255) : shadowColor;

            Color bgColor = action switch
            {
                ButtonAction.Click => ClickBGColor,
                ButtonAction.Disabled => DisabledBGColor,
                ButtonAction.Highlight => HighlightBGColor,
                ButtonAction.Normal => MainBGColor,
                _ => MainBGColor
            };
            bgColor = bgColor == Color.Transparent ? Color.FromArgb(0, 255, 255, 255) : bgColor;
            bgColor = bgColor.IsEmpty ? copy.BackColor : bgColor;

            Color lineColor = action switch
            {
                ButtonAction.Click => ClickLineColor,
                ButtonAction.Disabled => DisabledLineColor,
                ButtonAction.Highlight => HighlightLineColor,
                ButtonAction.Normal => MainLineColor,
                _ => MainLineColor
            };
            lineColor = lineColor == Color.Transparent ? Color.FromArgb(0, 255, 255, 255) : lineColor;

            Color textColor = action switch
            {
                ButtonAction.Click => ClickTextColor,
                ButtonAction.Disabled => DisabledTextColor,
                ButtonAction.Highlight => HighlightTextColor,
                ButtonAction.Normal => MainTextColor,
                _ => MainTextColor
            };
            textColor = textColor == Color.Transparent ? Color.FromArgb(0, 255, 255, 255) : textColor;
            //=====================================

            //==============Drawing================
            if (shadowWidth > 0)
            {
                //Draw shadow in the back
                using Brush shadowBrush = new SolidBrush(shadowColor);
                using Pen shadowPen = new Pen(shadowBrush, shadowWidth * 2);
                if (b.Parent is PictureBox || copy.BackColor == Color.FromArgb(0, 255, 255, 255)) { e.Graphics.DrawRoundedRectanglePart(shadowPen, rect[0], Btn_CornerRadius, Btn_ShadowLocation); }
                else {  e.Graphics.DrawRoundedRectangle(shadowPen, rect[0], Btn_CornerRadius); }
            }
            
            //Draw over any existing button graphics
            using Brush clearBrush = new SolidBrush((b.Parent is PictureBox) ? Color.FromArgb(0, 255, 255, 255) : ExtensionMethods.GetParentBackColor(b));
            e.Graphics.FillRoundedRectangle(clearBrush, rect[1], Btn_CornerRadius);

            //Draw background color of the button
            using Brush backBrush = new SolidBrush(bgColor);
            e.Graphics.FillRoundedRectangle(backBrush, rect[1], Btn_CornerRadius);

            //Draw outline of the button
            using Brush buttonBrush = new SolidBrush(lineColor);
            using Pen buttonPen = new Pen(buttonBrush, Btn_LineWidth);
            e.Graphics.DrawRoundedRectangle(buttonPen, rect[1], Btn_CornerRadius);

            //Draw text of the button
            //Button text is set to "" so that transparency will not show the original text.
            //I use a copy of the button to pull the text to draw.
            using Brush textBrush = new SolidBrush(textColor.IsEmpty ? b.ForeColor : textColor);
            e.Graphics.DrawString(copy.Text, b.Font, textBrush, rect[1], stringFormat);
            //=====================================
        }

        private Rectangle[] CalculateRects(Button b, int shadowWidth)
        {
            int width = b.Size.Width - 1;
            int height = b.Size.Height - 1;
            if (Btn_ShadowLocation.ToString().Contains("Bottom") || Btn_ShadowLocation.ToString().Contains("Top"))
            { height -= Btn_LineWidth * 2; height -= shadowWidth; }
            if (Btn_ShadowLocation.ToString().Contains("Right") || Btn_ShadowLocation.ToString().Contains("Left"))
            { width -= Btn_LineWidth * 2; width -= shadowWidth; }

            Size size = new Size(width, height);

            Rectangle shadow = new Rectangle(new Point(0, 0), size);
            Rectangle button = new Rectangle(new Point(0, 0), size);

            if (Btn_ShadowLocation.ToString().Contains("Right"))
            {
                shadow.X = shadowWidth;
                button.X = 0;
            }
            if (Btn_ShadowLocation.ToString().Contains("Bottom"))
            {
                shadow.Y = shadowWidth;
                button.Y = 0;
            }
            if (Btn_ShadowLocation.ToString().Contains("Left"))
            {
                shadow.X = 0;
                button.X = shadowWidth;
            }
            if (Btn_ShadowLocation.ToString().Contains("Top"))
            {
                shadow.Y = 0;
                button.Y = shadowWidth;
            }
            return new Rectangle[] { shadow, button };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing) { }

                Button[] keys = ButtonCopies.Keys.ToArray();
                for (int i = 0; i < keys.Length; i++)
                {
                    Button b = ButtonCopies.ElementAt(i).Key;
                    Button copy = ButtonCopies.ElementAt(i).Value;

                    b.Paint -= B_Paint;
                    b.EnabledChanged -= EventNormal;
                    b.MouseEnter -= EventHighlight;
                    b.MouseLeave -= EventNormal;
                    b.MouseDown -= MouseEventClick;
                    b.MouseUp -= MouseEventHighlight;
                    b.TextChanged -= B_TextChanged;

                    copy.Copy(ref b);
                    copy.Dispose();

                    b.Refresh();
                }

                ButtonCopies.Clear();
                IsDisposed = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
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

    public static class ControlExtensions
    {
        public static T Clone<T>(this T controlToClone)
            where T : Control
        {
            PropertyInfo[] controlProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            T instance = Activator.CreateInstance<T>();

            foreach (PropertyInfo propInfo in controlProperties)
            {
                if (propInfo.CanWrite)
                {
                    if (propInfo.Name != "WindowTarget")
                        propInfo.SetValue(instance, propInfo.GetValue(controlToClone, null), null);
                }
            }

            return instance;
        }
        public static void Copy<T>(this T controlToClone, ref T targetControl)
            where T : Control
        {
            PropertyInfo[] controlProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo propInfo in controlProperties)
            {
                if (propInfo.CanWrite)
                {
                    if (propInfo.Name != "WindowTarget")
                    { propInfo.SetValue(targetControl, propInfo.GetValue(controlToClone, null), null); }
                }
            }
        }
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
        public static void DrawRoundedRectanglePart(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius, ShadowPosition position)
        {
            using GraphicsPath path = RoundedRectPart(bounds, cornerRadius, position);
            path.Flatten(new Matrix(), 0.12f);

            PointF prevP = new PointF();
            int total = path.PathPoints.Count();
            int count = 1;
            float penSize;

            int trim;
            if (position == ShadowPosition.Bottom || position == ShadowPosition.Top || position == ShadowPosition.Left || position == ShadowPosition.Right)
            { trim = 1; }
            else
            { trim = 2; }

            foreach (PointF p in path.PathPoints)
            {
                if (prevP.IsEmpty || count <= trim || count >= total - trim) { prevP = p; count++; continue; }

                if (count <= 3 || count >= total - 3) { penSize = 1; }
                else if (count <= 5 || count >= total - 5) { penSize = 2; }
                else if (count <= 8 || count >= total - 8) { penSize = 3; }
                else { penSize = pen.Width; }
                if (penSize > pen.Width) { penSize = pen.Width; }
                graphics.DrawLine(new Pen(pen.Color, penSize), prevP, p);
                prevP = p;
                count++;
            }
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
            { FillMode = FillMode.Alternate };

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(arc, 180, 90);

            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
        internal static GraphicsPath RoundedRectPart(Rectangle bounds, int radius, ShadowPosition position)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath
            { FillMode = FillMode.Alternate,  };

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            if (position == ShadowPosition.Right)
            {
                arc.X = bounds.Right - diameter;
                path.AddArc(arc, 270, 90);
                arc.Y = bounds.Bottom - diameter;
                path.AddArc(arc, 0, 90);
            }
            else if (position == ShadowPosition.BottomRight)
            {
                arc.X = bounds.Right - diameter;
                path.AddArc(arc, 270, 90);
                arc.Y = bounds.Bottom - diameter;
                path.AddArc(arc, 0, 90);
                arc.X = bounds.Left;
                path.AddArc(arc, 90, 90);
            }
            else if (position == ShadowPosition.Bottom)
            {
                arc.X = bounds.Right - diameter;
                arc.Y = bounds.Bottom - diameter;
                path.AddArc(arc, 0, 90);
                arc.X = bounds.Left;
                path.AddArc(arc, 90, 90);
            }
            else if (position == ShadowPosition.BottomLeft)
            {
                arc.X = bounds.Right - diameter;
                arc.Y = bounds.Bottom - diameter;
                path.AddArc(arc, 0, 90);
                arc.X = bounds.Left;
                path.AddArc(arc, 90, 90);
                arc.Y = bounds.Top;
                path.AddArc(arc, 180, 90);
            }
            else if (position == ShadowPosition.Left)
            {
                arc.Y = bounds.Bottom - diameter;
                arc.X = bounds.Left;
                path.AddArc(arc, 90, 90);
                arc.Y = bounds.Top;
                path.AddArc(arc, 180, 90);
            }
            else if (position == ShadowPosition.TopLeft)
            {
                arc.Y = bounds.Bottom - diameter;
                arc.X = bounds.Left;
                path.AddArc(arc, 90, 90);
                arc.Y = bounds.Top;
                path.AddArc(arc, 180, 90);
                arc.X = bounds.Right - diameter;
                path.AddArc(arc, 270, 90);
            }
            else if (position == ShadowPosition.Top)
            {
                path.AddArc(arc, 180, 90);
                arc.X = bounds.Right - diameter;
                path.AddArc(arc, 270, 90);
            }
            else if (position == ShadowPosition.TopRight)
            {
                path.AddArc(arc, 180, 90);
                arc.X = bounds.Right - diameter;
                path.AddArc(arc, 270, 90);
                arc.Y = bounds.Bottom - diameter;
                path.AddArc(arc, 0, 90);
            }
            return path;
        }
    }
}
