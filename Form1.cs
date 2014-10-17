using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace WindowsFormsApplication
{
    public partial class Form1 : Form
    {
        public GenerateMap.Generator map;
        private Timer timer = new Timer();
        private Image backbuffer;
        private int mouseX, mouseY;
        public class Vector
        {
            public float x, y, z;
        }
        public class Cordinate
        {
            public float direction;
            public Vector pos = new Vector();
        }
        public Cordinate position = new Cordinate();
        public Cordinate target = new Cordinate();
	    protected override void OnPaint(PaintEventArgs e)
	    {
		    base.OnPaint (e);
            e.Graphics.DrawImage(backbuffer, 0, 0);
	    }

	    protected override void OnMouseMove(MouseEventArgs e)
	    {
		    base.OnMouseMove (e);
            mouseX = e.X;
            mouseY = e.Y;
	    }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                target.pos.x = mouseX;
                target.pos.y = mouseY;
            }
        }
        private void Update()
        {
        }
        private void DebugFont()
        {
            Graphics g = Graphics.FromImage(backbuffer);
            SolidBrush brush = new SolidBrush(Color.Black);
            float hsize = 10 * g.DpiX / 72;
            g.DrawString("Mouse  X: " + mouseX + " Y:" + mouseY, this.Font, brush, 2, hsize * 0);
            int idx = 1 ;
            foreach (GenerateMap.Territory r in map.territory)
                {
                    //                g.DrawString("LX" + r.lx + "LY" + r.ly + "HX" + r.hx + "HY" + r.hy, this.Font, brush, 2, hsize * idx);
                    ++idx;
                }
//            g.DrawString("Start  X: " + position.pos.x + " Y:" + position.pos.y, this.Font, brush, 2, hsize * 1);
//            g.DrawString("Target X: " + target.pos.x + " Y:" + target.pos.y, this.Font, brush, 2, hsize * 2);
        }
        private void callDraw(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromImage(backbuffer);
            {   // clear.
                Brush brush = new SolidBrush(Color.White);
                Rectangle rect = new Rectangle(0, 0, this.Size.Width, this.Size.Height);
                g.FillRectangle(brush, rect);
            }
            {
                int blocksize = 4;
                List<Brush> listBrush = new List<Brush>();
                listBrush.Add(new SolidBrush(Color.Black));
                listBrush.Add(new SolidBrush(Color.Gray));
                listBrush.Add(new SolidBrush(Color.Yellow));
                listBrush.Add(new SolidBrush(Color.Red));
                listBrush.Add(new SolidBrush(Color.Aqua));
                listBrush.Add(new SolidBrush(Color.Brown));
                listBrush.Add(new SolidBrush(Color.CadetBlue));
                listBrush.Add(new SolidBrush(Color.Coral));
                for (int i = 0; i < map.GetConfig().width; i++)
                {
                    for (int j = 0; j < map.GetConfig().height; j++)
                    {
                        Rectangle re = new Rectangle((i * blocksize), (j * blocksize),blocksize, blocksize);
                        if (map.mapchip.entity[i, j] != 0)
                        {
                            g.FillRectangle(listBrush[map.mapchip.entity[i, j]], re);
                        }
                    }
                }
            }

            /*
                        {   // glid
                                int gridsize = 16;
                                int ws = this.Size.Width / gridsize;
                                int hs = this.Size.Height / gridsize;
                                for ( int i = 1 ; i < ws ; i ++ )
                                {
                                    Point sp = new Point((i * gridsize), 0);
                                    Point ep = new Point((i * gridsize), this.Size.Height);
                                    g.DrawLine(Pens.Black, sp, ep);
                                }
                                for (int i = 1 ; i < hs; i++)
                                {
                                    Point sp = new Point(0, (i * gridsize));
                                    Point ep = new Point(this.Size.Width, (i * gridsize));
                                    g.DrawLine(Pens.Black, sp, ep);
                                }
                        }
                        position.direction += 1.0f;
                        if (position.direction >= 180.0f)
                        {
                            position.direction -= 360.0f; 
                        }
                        target.direction -= 1.0f;
                        if (target.direction <= 180.0f)
                        {
                            target.direction += 360.0f;
                        }
                        {   // position circles
                            int radius = 20;
                            float directionLength = 20.0f;
                            {   // position
                                Rectangle rect = new Rectangle((int)position.pos.x - (radius / 2), (int)position.pos.y - (radius / 2), radius, radius);
                                g.DrawEllipse(Pens.Red, rect);
                                Point sp = new Point((int)position.pos.x, (int)position.pos.y);
                                Point ep = new Point((int)(position.pos.x + (int)(directionLength * Math.Cos(Math.PI * position.direction / 180.0f)))
                                                   , (int)(position.pos.y + (int)(directionLength * Math.Sin(Math.PI * position.direction / 180.0f))));
                                g.DrawLine(Pens.Blue, sp, ep);
                            }
                            {   // position
                                Rectangle rect = new Rectangle((int)target.pos.x - (radius / 2), (int)target.pos.y - (radius / 2), radius, radius);
                                g.DrawEllipse(Pens.Green, rect);
                                Point sp = new Point((int)target.pos.x, (int)target.pos.y);
                                Point ep = new Point((int)(target.pos.x + (int)(directionLength * Math.Cos(Math.PI * target.direction / 180.0f)))
                                                   , (int)(target.pos.y + (int)(directionLength * Math.Sin(Math.PI * target.direction / 180.0f))));
                                g.DrawLine(Pens.Green, sp, ep);
                            }
                        }
            */
            Update();
            DebugFont();
            Invalidate();
        }

        public Form1() 
	    {
            InitializeComponent();

            SetStyle(
			    ControlStyles.DoubleBuffer |
			    ControlStyles.UserPaint |
			    ControlStyles.AllPaintingInWmPaint, true
		    );

            backbuffer = new Bitmap(this.Size.Width, this.Size.Height);
            Graphics g = Graphics.FromImage(backbuffer);
		    Brush brush = new SolidBrush(Color.White);
            g.FillRectangle(brush, 0, 0, backbuffer.Width, backbuffer.Height);
            target.pos.x = position.pos.x = (backbuffer.Width / 2.0f);
            target.pos.y = position.pos.y = (backbuffer.Height / 2.0f);
            timer.Tick += new EventHandler(callDraw);
            timer.Interval = 16;
            timer.Enabled = true;
	    }    
    }
}
