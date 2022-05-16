using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{ 
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);
        // глобальные переменные, в которых будут храниться координаты
        static protected long totalPixels = 0;
        static protected int currX;
        static protected int currY;
        static protected int diffX;
        static protected int diffY;

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);
        [Flags]
        public enum MOUSEEVENTF
        {
            MOVE = 0x01,
            LEFTDOWN = 0x02,
            LEFTUP = 0x04,
            RIGHTDOWN = 0x08,
            RIGHTUP = 0x10,
            MIDDLEDOWN = 0x0020,
            MIDDLEUP = 0x0040,
            XDOWN = 0x0080,
            XUP = 0x0100,
            WHEEL = 0x0800,
            HWHEEL = 0x01000,
            MOVE_NOCOALESCE = 0x2000,
            VIRTUALDESK = 0x4000,
            ABSOLUTE = 0x8000
        }

        private Color myColor = new Color();
        private string color;
        private string[] a = new string [8];
        private int counter = 0;
        private int clickcounter = 0;


        int[,] poInts = new int[2, 13]; //{ { 59, 59, 103, 199, 199, 245, 372, 372, 417, 523, 538, 538}, { 83, 118,79, 83, 118, 79, 83, 118, 79,140,120,84} }

        private bool _isDispose = false;
        private bool _isWork = false;

        private int firstPoint1;
        private int firstPoint2;
        private int firstPoint3;

        private int lastPoint1;
        private int lastPoint2;
        private int lastPoint3;

        int value;
        int UpperSide;
        int LowSide;

        int raz = 0;

        Stopwatch sw = new Stopwatch();

        private Graphics graphics;
        private Bitmap printscreen;



        public Form1()
        {
            InitializeComponent();
            textBox9.Text = "680";
            textBox8.Text = "370";
            textBox47.Text = "XY формы";
            textBox46.Text = "XY экрана";
            textBox48.Text = "Начало коорд скриншота";
            textBox49.Text = "Размеры скриншота";
            textBox51.Text = "550";
            textBox50.Text = "150";
            //textBox3.Text = "True";

            Thread th = new Thread(MakeScreenshot2);
            th.IsBackground = false;
            th.Start();
        }

        private void tmrDef_Tick(object sender, EventArgs e)
        {
            // обновление информации происходит каждые 10 мс
            Point defPnt = new Point();
            
            // заполняем defPnt информацией о координатах мышки
            GetCursorPos(ref defPnt);
            
            // выводим информацию в окно
            textBox6.Text = "X = " + defPnt.X.ToString();
            
            textBox5.Text = "Y = " + defPnt.Y.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MakeScreenshot1();
        }
        
        public void MakeScreenshot2()
        {
            while (true)
            {
                //sw.Start();

                if (_isDispose == true)
                {
                    return;
                }

                if (_isWork == true)
                {
                    try
                    {
                        printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                        graphics = Graphics.FromImage(printscreen);
                        graphics.CopyFromScreen(Convert.ToInt32(textBox9.Text), Convert.ToInt32(textBox8.Text), 0, 0, new Size(Convert.ToInt32(textBox51.Text), Convert.ToInt32(textBox50.Text)));
                        //pictureBox1.Image = printscreen;
                        GetColours2(printscreen);
                        printscreen.Dispose();
                        graphics.Dispose();
                    }
                    catch
                    {
                        continue;
                    }
                }

                //sw.Stop();
                //raz++;
                //Console.WriteLine(sw.ElapsedMilliseconds);
                //Console.WriteLine(raz);
                //sw.Reset();
                
                //Thread.Sleep(20);
            }
        }

        public void MakeScreenshot1()
        {
            Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(printscreen);
            graphics.CopyFromScreen(Convert.ToInt32(textBox9.Text), Convert.ToInt32(textBox8.Text), 0, 0, new System.Drawing.Size(Convert.ToInt32(textBox51.Text), Convert.ToInt32(textBox50.Text)));
            //GetColours2(printscreen);
            pictureBox1.Image = printscreen;
        }

        public void GetBasicColours(Bitmap printscreen)
        {
            //цвет первой точки
            myColor = printscreen.GetPixel(poInts[0, 0], poInts[1, 0]);
            color = Convert.ToString(myColor);
            a = color.Split('=', ',', ']');
            firstPoint1 = Convert.ToInt32(a[3]);
            firstPoint2 = Convert.ToInt32(a[5]);
            firstPoint3 = Convert.ToInt32(a[7]);
            //Console.WriteLine(firstPoint1 + " " + firstPoint2 + " " + firstPoint3);

            //цвет последней точки
            myColor = printscreen.GetPixel(poInts[0, 12], poInts[1, 12]);
            color = Convert.ToString(myColor);
            a = color.Split('=', ',', ']');
            lastPoint1 = Convert.ToInt32(a[3]);
            lastPoint2 = Convert.ToInt32(a[5]);
            lastPoint3 = Convert.ToInt32(a[7]);
            //Console.WriteLine(lastPoint1 + " " + lastPoint2 + " " + lastPoint3);
        }

        public void GetColours2(Bitmap printscreen)
        {
            GetBasicColours(printscreen);

            //сравниваем цвета точек - нужно чтобы совпадали с первой и отличались от последней

            for (int x = 1; x < 12; x++) //11
            {
                myColor = printscreen.GetPixel(poInts[0,x], poInts[1,x]);
                color = Convert.ToString(myColor);
                a = color.Split('=', ',', ']');
                //Console.WriteLine(Convert.ToInt32(a[1]) + " "+ Convert.ToInt32(a[3]) + " " + Convert.ToInt32(a[5]) + " " + Convert.ToInt32(a[7]));

                value = firstPoint1;
                LowSide = lastPoint1 - 5;
                UpperSide = lastPoint1 + 5;

                if (value >= LowSide && value <= UpperSide)
                {
                    return;
                }

                if (Convert.ToInt32(a[3]) >= firstPoint1-5 && Convert.ToInt32(a[3]) <= firstPoint1+5 )
                {
                    value = firstPoint2;
                    LowSide = lastPoint2 - 5;
                    UpperSide = lastPoint2 + 5;

                    if (value >= LowSide && value <= UpperSide)
                    {
                        return;
                    }

                    if (Convert.ToInt32(a[5]) >= firstPoint2-5 && Convert.ToInt32(a[3]) <= firstPoint2+5)
                    {
                        value = firstPoint3;
                        LowSide = lastPoint3 - 5;
                        UpperSide = lastPoint3 + 5;

                        if (value >= LowSide && value <= UpperSide)
                        {
                            return;
                        }

                        if (Convert.ToInt32(a[7]) >= firstPoint3 - 5 && Convert.ToInt32(a[3]) <= firstPoint2 + 5)
                        {
                            counter++;
                        }
                        else
                        {
                            counter = 0;
                            //textBox9.Text = "False";
                            //Console.WriteLine(textBox9.Text);
                            return;
                        }
                    }
                    else
                    {
                        counter = 0;
                        //textBox9.Text = "False";
                        //Console.WriteLine(textBox9.Text);
                        return;
                    }
                }
                else
                {
                    counter = 0;
                    //textBox9.Text = "False";
                    //Console.WriteLine(textBox9.Text);
                    return;
                }
            }

            if (counter==11)
            {
                counter = 0;
                //textBox3.Text = "True";
                Size resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
                int X = (65535 / resolution.Width) * 962; //Рассчитать абсолютные координаты по оси X
                int y = (65535 / resolution.Height) * 795; //Рассчитать абсолютные координаты по оси Y
                mouse_event((uint)(MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.MOVE), X, y, 0, 0); //передвинули курсор
                mouse_event((uint)(MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.LEFTDOWN), X, y, 0, 0); //нажали мышку
                mouse_event((uint)(MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.LEFTUP), X, y, 0, 0); //отпустили мышку
            }
        }
        
        private void pictureBox1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (clickcounter == 0)
            {
                PointToScreen(e.Location);
                poInts[0, clickcounter] = e.X;
                textBox11.Text = e.X.ToString();
                poInts[1, clickcounter] = e.Y;
                textBox12.Text = e.Y.ToString();
                clickcounter++;
            }
            else if (clickcounter == 1)
            {
                PointToScreen(e.Location);
                poInts[0, clickcounter] = e.X;
                textBox14.Text = e.X.ToString();
                poInts[1, clickcounter] = e.Y;
                textBox13.Text = e.Y.ToString();
                clickcounter++;
            }
            else if(clickcounter == 2)
            {
                PointToScreen(e.Location);
                poInts[0, clickcounter] = e.X;
                textBox17.Text = e.X.ToString();
                poInts[1, clickcounter] = e.Y;
                textBox16.Text = e.Y.ToString();
                clickcounter++;
            }
            else if(clickcounter == 3)
            {
                PointToScreen(e.Location);
                poInts[0, clickcounter] = e.X;
                textBox20.Text = e.X.ToString();
                poInts[1, clickcounter] = e.Y;
                textBox19.Text = e.Y.ToString();
                clickcounter++;
            }
            else if(clickcounter == 4)
            {
                PointToScreen(e.Location);
                poInts[0, clickcounter] = e.X;
                textBox23.Text = e.X.ToString();
                poInts[1, clickcounter] = e.Y;
                textBox22.Text = e.Y.ToString();
                clickcounter++;
            }
            else if(clickcounter == 5)
            {
                PointToScreen(e.Location);
                poInts[0, clickcounter] = e.X;
                textBox26.Text = e.X.ToString();
                poInts[1, clickcounter] = e.Y;
                textBox25.Text = e.Y.ToString();
                clickcounter++;
            }
            else if(clickcounter == 6)
            {
                PointToScreen(e.Location);
                poInts[0, clickcounter] = e.X;
                textBox29.Text = e.X.ToString();
                poInts[1, clickcounter] = e.Y;
                textBox28.Text = e.Y.ToString();
                clickcounter++;
            }
            else if(clickcounter == 7)
            {
                PointToScreen(e.Location);
                poInts[0, clickcounter] = e.X;
                textBox32.Text = e.X.ToString();
                poInts[1, clickcounter] = e.Y;
                textBox31.Text = e.Y.ToString();
                clickcounter++;
            }
            else if(clickcounter == 8)
            {
                PointToScreen(e.Location);
                poInts[0, clickcounter] = e.X;
                textBox35.Text = e.X.ToString();
                poInts[1, clickcounter] = e.Y;
                textBox34.Text = e.Y.ToString();
                clickcounter++;
            }
            else if(clickcounter == 9)
            {
                PointToScreen(e.Location);
                poInts[0, clickcounter] = e.X;
                textBox38.Text = e.X.ToString();
                poInts[1, clickcounter] = e.Y;
                textBox37.Text = e.Y.ToString();
                clickcounter++;
            }
            else if(clickcounter == 10)
            {
                PointToScreen(e.Location);
                poInts[0, clickcounter] = e.X;
                textBox41.Text = e.X.ToString();
                poInts[1, clickcounter] = e.Y;
                textBox40.Text = e.Y.ToString();
                clickcounter++;
            }
            else if(clickcounter == 11)
            {
                PointToScreen(e.Location);
                poInts[0, clickcounter] = e.X;
                textBox44.Text = e.X.ToString();
                poInts[1, clickcounter] = e.Y;
                textBox43.Text = e.Y.ToString();
                clickcounter++;
            }
            else if (clickcounter == 12)
            {
                PointToScreen(e.Location);
                poInts[0, clickcounter] = e.X;
                textBox4.Text = e.X.ToString();
                poInts[1, clickcounter] = e.Y;
                textBox7.Text = e.Y.ToString();
                clickcounter = 0;
            }

            //textBox3.Text = Convert.ToString(printscreen.GetPixel(e.X, e.Y));
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            textBox1.Text = e.X.ToString();
            textBox2.Text = e.Y.ToString();
            //textBox3.Text = e.X.ToString() + " " + e.Y.ToString();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            _isWork = true;
            //MakeScreenshot2();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _isWork = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _isDispose = true;
        }
    }
}

