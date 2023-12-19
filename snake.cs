using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace snape
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    public enum Tile
    {
        Void,
        Head,
        Body,
        Apple
    }

    public class snape
    {
        public int countApple = 0;        
        public static int W = 20;
        public static int H = 20;
        public Tile[,] Field = new Tile[W,H];

        public List<Coord> snake = new List<Coord>();


        public void Init()
        {
            snake.Clear();
            countApple = 0;
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    Field[i, j] = Tile.Void;
                }
            }

            snake.Add(new Coord(W/2,H/2));
            Field[W/2, H/2] = Tile.Head;

            CurrentDirection = Direction.Up;
            SetApple();
        }


        public Boolean isDead()
        {
            Coord head = new Coord(snake[0].X, snake[0].Y);

            Coord up = new Coord(head.X, (head.Y - 1 + H) % H);

            Coord down = new Coord(head.X, (head.Y + 1 + H) % H);

            Coord left = new Coord((head.X - 1 + W) % W, head.Y);

            Coord right = new Coord((head.X + 1 + W) % W, head.Y);

            return (Field[up.X, up.Y] == Tile.Body && Field[down.X, down.Y] == Tile.Body && Field[left.X, left.Y] == Tile.Body && Field[right.X, right.Y] == Tile.Body);
        }

        public void Tern()
        {
            Coord head = new Coord(snake[0].X, snake[0].Y);
                
            
            switch (CurrentDirection)
            {
                case Direction.Up:
                    head.Y = (head.Y - 1 + H) % H;
                    break;
                case Direction.Down:
                    head.Y = (head.Y + 1 + H) % H;
                    break;
                case Direction.Left:
                    head.X = (head.X - 1 + W) % W;
                    break;
                case Direction.Right:
                    head.X = (head.X + 1 + W) % W;
                    break;
                default:
                    break;
            }
            if (Field[head.X, head.Y] == Tile.Apple)
            {
                Field[head.X, head.Y] = Tile.Head;
                Field[snake[0].X, snake[0].Y] = Tile.Body;

                snake.Insert(0, head);

                countApple++;

                SetApple();
            }

            if(Field[head.X, head.Y] == Tile.Void)
            {
                Field[head.X, head.Y] = Tile.Head;
                Field[snake[0].X, snake[0].Y] = Tile.Body;

                snake.Insert(0, head);

                Coord tail = snake[snake.Count - 1];
                Field[tail.X, tail.Y] = Tile.Void;
                snake.RemoveAt(snake.Count - 1);

            }

        }

        Random rnd = new Random();


        public SolidBrush currentBrush = new SolidBrush(Color.Red);
        public SolidBrush currentBrushPitonBody = new SolidBrush(Color.Gray);
        public void SetApple()
        {
            List<Coord> lst = new List<Coord>();
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    if (Field[i, j] == Tile.Void) lst.Add(new Coord(i,j));
                }
            }

            Coord apple = lst[rnd.Next(lst.Count)];

            Field[apple.X, apple.Y] = Tile.Apple;


            currentBrushPitonBody = currentBrush;
            currentBrush = new SolidBrush(Color.FromArgb(255, (byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256)));

        }

        public Direction CurrentDirection = Direction.Up;
        public snape(int w, int h)
        {
            W = w;
            H = h;
            Field = new Tile[W, H];
            Init();
        }
    }

    public class Coord
    {
        public int X;
        public int Y;

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Coord() { }
    }
}

Листинг программы Form1.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace snape
{
    public partial class Form1 : Form
    {
        snape play = null;

        public Form1()
        {
            InitializeComponent();
        }

        int atomR = 10;
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if(play!=null)
            {

                int wR = (pictureBox1.Width / snape.W) / 2 - 1;
                int hR = (pictureBox1.Height / snape.H) / 2 - 1;


                Graphics g = e.Graphics;
                for (int i = 0; i < snape.W; i++)
                {
                    for (int j = 0; j < snape.H; j++)
                    {
                        Rectangle r = new Rectangle(i * wR*2, j * hR*2, 2 * wR-1, 2 * hR-1);
                        switch (play.Field[i,j])
                        {
                            case Tile.Void:
                                g.DrawEllipse(Pens.LightGray, r);
                                //g.FillEllipse(Brushes.Green, r);
                                break;
                            case Tile.Head:
                                g.DrawEllipse(Pens.Green, r);
                                g.FillEllipse(Brushes.Green, r);
                                break;
                            case Tile.Body:
                                g.DrawEllipse(Pens.White, r);
                                g.FillEllipse(play.currentBrushPitonBody, r);
                                break;
                            case Tile.Apple:
                                g.DrawEllipse(Pens.White, r);
                                g.FillEllipse(play.currentBrush, r);
                                break;
                            default:
                                break;
                        }
                    }

                    Text = "Съедено яблок: " + play.countApple.ToString();
                }

                if (play.isDead())
                {
                    Text = "СМЕРТЬ!!!!";
                    timer1.Stop();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            play = new snape((int)numericUpDown2.Value, (int)numericUpDown1.Value);
            play.Init();
            pictureBox1.Invalidate();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            play.Tern();
            pictureBox1.Invalidate();       
        }

        private void button1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Up) play.CurrentDirection = Direction.Up;
            if (e.KeyCode == Keys.Down) play.CurrentDirection = Direction.Down;
            if (e.KeyCode == Keys.Left) play.CurrentDirection = Direction.Left;
            if (e.KeyCode == Keys.Right) play.CurrentDirection = Direction.Right;

            if (e.KeyCode == Keys.Add)
                if (timer1.Interval > 20) timer1.Interval = timer1.Interval - 20;
            if (e.KeyCode == Keys.Subtract)
                if (timer1.Interval < 300) timer1.Interval = timer1.Interval + 20;
            pictureBox1.Invalidate();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}