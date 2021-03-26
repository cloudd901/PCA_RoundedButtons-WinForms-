namespace RoundedButtons_Example
{
    using PCAFFINITY;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        public RoundedButtons roundedButtons1;
        public RoundedButtons roundedButtons2;
        public RoundedButtons roundedButtons3;
        public RoundedButtons roundedButtons4;

        public RoundedButtons roundedButtons5;

        public Form1()
        {
            InitializeComponent();

            button14.Parent = pictureBox1;
            button15.Parent = pictureBox1;
            button16.Parent = pictureBox1;
        }

        private void Button17_Click(object sender, EventArgs e)
        {
            if (roundedButtons2.IsDisposed)
            {
                RoundSection2();
            }
            else
            {
                roundedButtons2.Dispose(); roundedButtons3.Dispose();
            }
        }

        private void Button18_Click(object sender, EventArgs e)
        {
            if (roundedButtons4.IsDisposed)
            {
                RoundSection3();
            }
            else
            {
                roundedButtons4.Dispose();
            }
        }

        private void Button19_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            int number = r.Next(1000, 999999);
            button8.Text = number.ToString();
            number = r.Next(1000, 999999);
            button9.Text = number.ToString();
            number = r.Next(1000, 999999);
            button10.Text = number.ToString();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            if (roundedButtons1.IsDisposed)
            {
                RoundSection1();
            }
            else
            {
                roundedButtons1.Dispose();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RoundSection1();
            RoundSection2();
            RoundSection3();

            RoundSection5();
        }

        private void RoundSection5()
        {
            roundedButtons5 = new RoundedButtons()
            {
                Btn_LineWidth = 0,
                Btn_CornerRadius = 10,
                Btn_ShadowWidth = ShadowSize.Thin,
                MainShadowColor = Color.Gray,
                Btn_ShadowLocation = ShadowPosition.BottomRight
            };

            foreach (dynamic parent in new List<dynamic>() { panel6 })
            {
                foreach (object o in parent.Controls)
                {
                    if (o is Button b)
                    {
                        roundedButtons5.PaintButton(b);
                    }
                }
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
        }

        private void RoundSection1()
        {
            roundedButtons1 = new RoundedButtons()
            {
                Btn_CornerRadius = 6,
                Btn_ShadowWidth = ShadowSize.Thin
            };

            foreach (dynamic parent in new List<dynamic>() { panel1, panel2 })
            {
                foreach (object o in parent.Controls)
                {
                    if (o is Button b)
                    {
                        roundedButtons1.PaintButton(b);
                    }
                }
            }
        }

        private void RoundSection2()
        {
            roundedButtons2 = new RoundedButtons();
            foreach (object o in panel3.Controls)
            {
                if (o is Button b)
                {
                    roundedButtons2.PaintButton(b);
                }
            }

            roundedButtons3 = new RoundedButtons()
            {
                Btn_CornerRadius = 10,
                Btn_ShadowWidth = ShadowSize.Thin,
                Btn_ShadowLocation = ShadowPosition.TopLeft
            };

            foreach (object o in panel4.Controls)
            {
                if (o is Button b)
                {
                    roundedButtons3.PaintButton(b);
                }
            }
        }

        private void RoundSection3()
        {
            roundedButtons4 = new RoundedButtons()
            {
                Btn_CornerRadius = 8,
                Btn_ShadowWidth = ShadowSize.Normal,
                MainShadowColor = Color.Black,
                MainTextColor = Color.White,
                Btn_ShadowLocation = ShadowPosition.Bottom
            };

            roundedButtons4.PaintButton(button14);
            roundedButtons4.PaintButton(button15);
            roundedButtons4.PaintButton(button16);
        }

        private void Button16_Click(object sender, EventArgs e)
        {
            button16.Visible = false;
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            button16.Visible = true;
        }
    }
}