using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VL53LX_ToF
{
    public class CoordinatSystem
    {
        private int height;
        private int width;
        public int setHeight
        {
            get => height;
            set => height = value;
        }
        public int setWidth
        {
            get => width;
            set => width = value;
        }
        public void InitCanvas(Canvas drawingCanvas)
        {
            DrawLine(10, 10, 10, height, drawingCanvas);
            DrawLine(10, height, width, height, drawingCanvas);
            DrawLine(5, 15, 10, 10, drawingCanvas);
            DrawLine(15, 15, 10, 10, drawingCanvas);
            DrawLine(width - 5, height - 5, width, height, drawingCanvas);
            DrawLine(width - 5, height + 5, width, height, drawingCanvas);
            DrawLine(0, 490, 10, 490, drawingCanvas);
            DrawLine(510, 850, 510, 860, drawingCanvas);
            Label xlabel = new Label()
            {
                Content = "x",
                FontSize = 30,
                Margin = new System.Windows.Thickness(20, 10, 0, 0)
            };
            drawingCanvas.Children.Add(xlabel);
            Label ylabel = new Label()
            {
                Content = "t",
                FontSize = 30,
                Margin = new System.Windows.Thickness(width - 50, height - 50, 0, 0)
            };
            drawingCanvas.Children.Add(ylabel);
        }
        private void DrawLine(int x1, int y1, int x2, int y2, Canvas drawingCanvas)
        {
            Line line = new Line()
            {
                Stroke = new SolidColorBrush(Colors.Black),
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
            };
            drawingCanvas.Children.Add(line);
        }
    }
}
