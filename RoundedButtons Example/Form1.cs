using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PCAFFINITY;

namespace RoundedButtons_Example
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RoundedButtons roundedButtons1 = new RoundedButtons()
            { Btn_CornerRadius = 6, Btn_ShadowWidth = ShadowSize.Thin };

            List<dynamic> parents = new List<dynamic>() { panel1, panel2 };
            foreach (dynamic parent in parents)
            {
                foreach (object o in parent.Controls)
                {
                    if (o is Button b)
                    {
                        roundedButtons1.PaintButton(b);
                    }
                }
            }

            RoundedButtons roundedButtons2 = new RoundedButtons();

            foreach (object o in panel3.Controls)
            {
                if (o is Button b)
                {
                    roundedButtons2.PaintButton(b);
                }
            }

            RoundedButtons roundedButtons3 = new RoundedButtons()
            { Btn_CornerRadius = 10, Btn_ShadowWidth = ShadowSize.Thin, Btn_ShadowLocation = ShadowPosition.TopLeft };

            foreach (object o in panel4.Controls)
            {
                if (o is Button b)
                {
                    roundedButtons3.PaintButton(b);
                }
            }

            RoundedButtons roundedButtons4 = new RoundedButtons()
            { Btn_CornerRadius = 5, Btn_ShadowWidth = ShadowSize.Thick };

            roundedButtons4.PaintButton(button14);

            button15.Parent = pictureBox1;
            roundedButtons4.PaintButton(button15);
            button16.Parent = pictureBox1;
            roundedButtons4.PaintButton(button16);
        }
    }
}
