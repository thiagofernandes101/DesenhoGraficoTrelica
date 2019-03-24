using DesenhoTrelicaUI.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesenhoTrelicaUI
{
    public partial class Form1 : Form
    {
        List<Shape> shapes = new List<Shape>();

        private PointF Corner = new Point(-5, -5);
        private Single gridStep = 20;
        private Single SnapStep = 5;
        private Single ScaleWidth = 200;

        private Font fnt = new Font("Arial", 10);

        private PointF MouseDownPt, MouseMovePt;
        private int MouseStaus = 0;

        private Graphics g = null;

        private PointF SnapToGrid(PointF thispt)
        {
            Single x = Convert.ToInt32(thispt.X / SnapStep) * SnapStep;
            Single y = Convert.ToInt32(thispt.Y / SnapStep) * SnapStep;
            return new PointF(x, y);
        }

        private int RoundToIncrement(double theValue, int roundIncrement)
        {
            return Convert.ToInt32((Convert.ToDouble(theValue) + (0.5 * roundIncrement)) / roundIncrement) * roundIncrement;
        }

        private PointF GetScalePtFromClientPt(PointF pt)
        {
            double sf = ClientSize.Width / ScaleWidth;
            return new PointF(Convert.ToSingle(Corner.X + (pt.X / sf)), Convert.ToSingle(Corner.Y + (pt.Y / sf)));
        }

        private void DrawGrid(Graphics g)
        {
            Single x1 = RoundToIncrement(Corner.X - gridStep, Convert.ToInt32(gridStep));
            Single y1 = RoundToIncrement(Corner.Y - gridStep, Convert.ToInt32(gridStep));

            Single sw = Convert.ToSingle(ScaleWidth + (2 * gridStep));
            double pxlSize = ScaleWidth / ClientSize.Width;

            Graphics graphics = g;

            using (Pen pg = new Pen(Color.DarkGray, Convert.ToSingle(pxlSize / 6)))
            {
                Font f = new Font("arial", Convert.ToSingle(11 * pxlSize));
                SolidBrush br = new SolidBrush(Color.DimGray);

                for (Single x = x1; x <= Convert.ToSingle(x1 + sw); x = x + gridStep)
                {
                    graphics.DrawLine(pg, x, y1, x, Convert.ToSingle(y1 + sw));
                    graphics.DrawString(x.ToString(), f, br, x, Corner.Y);
                }

                for (Single y = y1; y <= Convert.ToSingle(y1 + sw); y = y + gridStep)
                {
                    graphics.DrawLine(pg, x1, y, Convert.ToSingle(x1 + sw), y);
                    graphics.DrawString(y.ToString(), f, br, Corner.X, y);
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                switch (MouseStaus)
                {
                    case 1:
                        PointF pt = GetScalePtFromClientPt(e.Location);
                        MouseMovePt = SnapToGrid(pt);
                        Invalidate();
                        break;
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            switch (MouseStaus)
            {
                case 1:
                    PointF pt = GetScalePtFromClientPt(e.Location);
                    MouseMovePt = SnapToGrid(pt);

                    Shape shp = new Shape();
                    shp.pt1 = MouseDownPt;
                    shp.pt2 = MouseMovePt;
                    shp.color = Color.Red;
                    shapes.Add(shp);
                    break;
            }

            MouseStaus = 0;
            Invalidate();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PointF pt = GetScalePtFromClientPt(e.Location);
                MouseDownPt = SnapToGrid(pt);
                MouseMovePt = MouseDownPt;
                MouseStaus = 1;
            }
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            graphics.ResetTransform();
            graphics.Clear(Color.White);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Single sf = Convert.ToSingle(ClientSize.Width / ScaleWidth);
            graphics.ScaleTransform(sf, sf);
            graphics.TranslateTransform(-Corner.X, -Corner.Y);

            DrawGrid(e.Graphics);

            foreach (Shape shp in shapes)
            {
                shp.Draw(e.Graphics);
            }

            switch (MouseStaus)
            {
                case 1:
                    graphics.DrawLine(new Pen(Color.Red, graphics.VisibleClipBounds.Width / 100), MouseDownPt, MouseMovePt);
                    break;
            }
        }
    }
}
