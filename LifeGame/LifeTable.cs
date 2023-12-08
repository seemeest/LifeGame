using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LifeGame
{
    public class Cell
    {
        public bool IsAlive { get; set; }
        public Rectangle VisualRepresentation { get; set; }

        public Cell(bool isAlive, Rectangle visualRepresentation)
        {
            IsAlive = isAlive;
            VisualRepresentation = visualRepresentation;
            UpdateVisual();
        }

        public void UpdateVisual()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                VisualRepresentation.Fill = IsAlive ? new SolidColorBrush(Color.FromRgb(204, 93, 93)): Brushes.White;
            });
        }


    }

    public class LifeTable
    {
        private Cell[,] cells;

        public int Height { get; private set; }
        public int Width { get; private set; }
        public  int CellSize { get; private set; }
        public  int GenerateProcent { get; private set; }
        public void InitializeRandom()
        {
            Random random = new Random();

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    bool isAlive = random.Next(GenerateProcent) == 0; 
                    SetCellState(i, j, isAlive);
                }
            }
        }
        public LifeTable(int height, int width,int cellSize,int gnerateProcent, Canvas canvas)
        {
            GenerateProcent= gnerateProcent;
            CellSize = cellSize;
            Height = height;
            Width = width;

            cells = new Cell[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Rectangle rectangle = new Rectangle
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Stroke = Brushes.Black,
                        StrokeThickness = 0.5
                    };

                    Canvas.SetTop(rectangle, i * cellSize);
                    Canvas.SetLeft(rectangle, j * cellSize);

                    cells[i, j] = new Cell(false, rectangle);
                    canvas.Children.Add(rectangle);
                }
            }
        }

        public void SetCellState(int i, int j, bool isAlive)
        {
            cells[i, j].IsAlive = isAlive;
            cells[i, j].UpdateVisual();
        }

        public bool GetCellState(int i, int j)
        {
            return cells[i, j].IsAlive;
        }
        public override string ToString()
        {
            return $"GenerateProcent:{GenerateProcent} CellSize:{CellSize} Height:{Height} Width:{Width}  ";
        }
    }
}
