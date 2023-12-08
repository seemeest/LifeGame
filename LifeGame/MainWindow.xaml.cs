using System.Diagnostics;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LifeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LifeTable lifeTable;
        private LifeTable lifeTable2;
        private CancellationTokenSource cancellationTokenSource;
        private Point lastMousePos;
        private MatrixTransform transform = new MatrixTransform();
        private bool isSimulationRunning = false;
        // Определение лямбда-выражения lifeRules внутри класса MainWindow
        private Func<int, bool, bool> lifeRules = new Func<int, bool, bool>((p, state) =>
        {
            if (p == 3) return true;
            else if (p == 2) return state;
            else return false;
        });

        public MainWindow()
        {
           
            InitializeComponent();
            StartSettings.Visibility = Visibility.Collapsed;
            CanvasPanel.RenderTransform = transform;
            CanvasPanel2.RenderTransform = transform;

            // Подписываемся на события мыши
            CanvasPanel.MouseWheel += Canvas_MouseWheel;
            CanvasPanel.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            CanvasPanel.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
            CanvasPanel2.MouseWheel += Canvas_MouseWheel1;
            CanvasPanel2.MouseLeftButtonDown += Canvas_MouseLeftButtonDown1;
            CanvasPanel2.MouseLeftButtonUp += Canvas_MouseLeftButtonUp1;

            Thread tlist = new Thread(ThreadList);
            tlist.Name = "ThreadList";
            tlist.Start();

        }
        public void ThreadList()
        {
            while (Application.Current != null)
            {
                Process currentProcess = Process.GetCurrentProcess();

      
                ProcessThreadCollection currentThreads = Process.GetCurrentProcess().Threads;
                foreach (ProcessThread thread in currentThreads)
                {
                    {
                        if (thread.ThreadState == System.Diagnostics.ThreadState.Running)
                        {
                            Console.WriteLine($"ID потока: {thread.Id}, Состояние: {thread.ThreadState}");
                            Console.WriteLine($"  Время выполнения : {thread.UserProcessorTime} время запуска {thread.StartTime}");
                            Console.WriteLine(thread.ToString());
                        }
                    }

                }

                Thread.Sleep(1000);
                Console.Clear(); 
            }
        }





        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Приближение и отдаление при вращении колесика мыши
            double scale = e.Delta > 0 ? 1.1 : 0.9;
            Point mousePos = e.GetPosition(canvasParalel);
            Matrix matrix = transform.Matrix;
            matrix.ScaleAtPrepend(scale, scale, mousePos.X, mousePos.Y);
            transform.Matrix = matrix;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Захватываем мышь при нажатии левой кнопки
            canvasParalel.CaptureMouse();
            lastMousePos = e.GetPosition(canvasParalel);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Освобождаем мышь при отпускании левой кнопки
            canvasParalel.ReleaseMouseCapture();
        }  private void Canvas_MouseWheel1(object sender, MouseWheelEventArgs e)
        {
            // Приближение и отдаление при вращении колесика мыши
            double scale = e.Delta > 0 ? 1.1 : 0.9;
            Point mousePos = e.GetPosition(canvasParalel);
            Matrix matrix = transform.Matrix;
            matrix.ScaleAtPrepend(scale, scale, mousePos.X, mousePos.Y);
            transform.Matrix = matrix;
        }

        private void Canvas_MouseLeftButtonDown1(object sender, MouseButtonEventArgs e)
        {
            // Захватываем мышь при нажатии левой кнопки
            CanvasPanel2.CaptureMouse();
            lastMousePos = e.GetPosition(canvasParalel);
        }

        private void Canvas_MouseLeftButtonUp1(object sender, MouseButtonEventArgs e)
        {
            // Освобождаем мышь при отпускании левой кнопки
            CanvasPanel2.ReleaseMouseCapture();
        }

        private int CalcPotential(int i, int j)
        {
            int p = 0;
            for (int x = i - 1; x <= i + 1; x++)
            {
                for (int y = j - 1; y <= j + 1; y++)
                {
                    if (x < 0 || y < 0 || x >= lifeTable.Height || y >= lifeTable.Width || (x == i && y == j))
                        continue;

                    if (lifeTable.GetCellState(x, y))
                        p++;
                }
            }
            return p;
        }

        private void SaveSettingClick(object sender, RoutedEventArgs e) {

            int height,width,cellsize, GenerateProcent;

            if (!string.IsNullOrEmpty(HeightTB.Text))
                if (int.TryParse(HeightTB.Text, out int result)) height = result;
                else { ErorText.Text = "Введите  Высоту ";  return; }
            else { ErorText.Text = "Введите валидное значение Высоты "; return; }

            if (!string.IsNullOrEmpty(WitdhTB.Text))
                if (int.TryParse(WitdhTB.Text, out int result)) width = result;
                else { ErorText.Text = "Введите  Ширину "; return; }
            else { ErorText.Text = "Введите валидное значение Ширины "; return; }

            if (!string.IsNullOrEmpty(CellSizeTB.Text))
                if (int.TryParse(CellSizeTB.Text, out int result)) cellsize = result;
                else { ErorText.Text = "Введите  Ширину "; return; }
            else { ErorText.Text = "Введите валидное значение Ширины "; return; }
            GenerateProcent = Convert.ToInt32(GenerateProcentSlider.Value);
            
            lifeTable = new LifeTable(height, width, cellsize, GenerateProcent, canvasParalel);
            lifeTable2 = new LifeTable(height, width, cellsize, GenerateProcent, canvasSequential);
            lifeTable.InitializeRandom();
            lifeTable2.InitializeRandom();
            Console.WriteLine(lifeTable.ToString()) ;
            Settings.Visibility = Visibility.Collapsed;
            StartSettings.Visibility = Visibility.Visible;
        }




        private void StartWithCancellation_Click(object sender, RoutedEventArgs e)
        {
            if (!isSimulationRunning)
            {
                isSimulationRunning = true;
                Thread t = new Thread(StartCalculationWithCancellation);
                t.Name = "WithCancellation";
                t.Start();
            }
        }

        private void StartSequential_Click(object sender, RoutedEventArgs e)
        {

           Thread t = new Thread(SequentialCalculation); t.Start();
        }



        private void StartCalculationWithCancellation()
        {
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            Task.Run(() =>
            {
                ParallelCalculationWithCancellation(cancellationToken);
           
            }, cancellationToken);
        }

        private void ParallelCalculationWithCancellation(CancellationToken cancellationToken)
        {
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                while (!cancellationToken.IsCancellationRequested)
                {

                    sw.Start();
                    sw.Restart();
                    Parallel.For(0, lifeTable.Height, (i, parallelLoopState) =>
                    {
                        for (int j = 0; j < lifeTable.Width; j++)
                        {
                            int p = CalcPotential(i, j);
                            bool newState = lifeRules(p, lifeTable.GetCellState(i, j));

                            try
                            {
                                if(Application.Current != null)
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        lifeTable.SetCellState(i, j, newState);
                                    });
                            }
                            catch
                            {

                            }
                        }
                    });
                    sw.Stop();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ParalelTime.Text = " ParallelCalculation "+ (1000/sw.ElapsedMilliseconds)+" fps";
                    });
                }
            }
            catch (OperationCanceledException)
            {
               
            }
        }

        private void SequentialCalculation()
        {
            while (true)
            {
                for (int i = 0; i < lifeTable2.Height; i++)
                {
                    for (int j = 0; j < lifeTable2.Width; j++)
                    {
                        int p = CalcPotential(i, j);
                        bool newState = lifeRules(p, lifeTable2.GetCellState(i, j));

                        if(Application.Current!=null)
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            lifeTable2.SetCellState(i, j, newState);
                        });
                    }
                }
            }
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            StopCalculation();
            isSimulationRunning = false;
        }
        private void StopCalculation()
        {
            cancellationTokenSource?.Cancel();
        }

    }
}