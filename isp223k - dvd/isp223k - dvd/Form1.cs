using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace isp223k___dvd
{

    public partial class MainForm : Form
    {
        Pen p;
        int dirX, dirY;
        int x, y;

        int speed;

        public MainForm()
        {
            BackColor = Color.Azure;
            TransparencyKey = Color.Azure;
            InitializeComponent();

            Dwm.Windows10EnableBlurBehind(Handle);

            label1.Visible = false;

            x = DVD_Sym.Location.X;
            y = DVD_Sym.Location.Y;
            dirX = 5;
            dirY = 5;

            speed = 1;
        }

        private void Draw(int x, int y)
        {
            DVD_Sym.Location = new Point(x, y);
        }

        private void Start_Click(object sender, EventArgs e)
        {
            Timer.Enabled = !Timer.Enabled;
            Start.Text = Timer.Enabled ? "Stop" : "Play";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            speed = int.TryParse(textBox1.Text, out speed) ? int.Parse(textBox1.Text) : 1;
        }

        private void Stop_Click(object sender, EventArgs e)
        {

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (x >= Picture.Width - DVD_Sym.Width / 1.05)
            {
                dirX = -speed;
            }
            else if (x <= Picture.Location.X)
            {
                dirX = speed;
            }

            if (y >= Picture.Height - DVD_Sym.Height / 1.75)
            {
                dirY = -speed;
            }
            else if (y <= Picture.Location.Y)
            {
                dirY = speed;
            }

            Draw(x += dirX, y += dirY);

            label1.Text = "X " + x + "Y " + y;
        }
    }
}
