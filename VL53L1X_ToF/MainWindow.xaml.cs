using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VL53LX_ToF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort serial = new SerialPort();
        string? recieved_data;
        string line = "";
        List<int> ints = new List<int>() { 0, 0 };
        List<string> data = new List<string>();
        double xDivider = 10;
        double yDivider = 1;
        int count = 0;
        bool record = false;
        List<int> average = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        CoordinatSystem distanceGraph = new CoordinatSystem();
        public MainWindow()
        {
            InitializeComponent();
            InitializeComponent();
            distanceGraph.setHeight = Convert.ToInt32(distanceCanvas.Height);
            distanceGraph.setWidth = Convert.ToInt32(distanceCanvas.Width);
            distanceGraph.InitCanvas(distanceCanvas);
            Connect_btn.Content = "Connect";
            setX.SelectedIndex = 1;
            setY.SelectedIndex = 1;
        }
        private void Connect_Comms(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(Connect_btn.Content) == "Connect")
            {
                //Sets up serial port
                serial.PortName = Comm_Port_Names.Text;
                serial.BaudRate = 115200;
                serial.Handshake = System.IO.Ports.Handshake.None;
                serial.Parity = Parity.None;
                serial.DataBits = 8;
                serial.StopBits = StopBits.One;
                serial.ReadTimeout = 200;
                serial.WriteTimeout = 50;
                try
                {
                    serial.Open();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"{ex}", "failed to connect");
                }
                //Sets button State and Creates function call on data recieved
                Connect_btn.Content = "Disconnect";
                serial.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(Recieve);
                if (serial.IsOpen)
                    System.Windows.MessageBox.Show("Connected to " + Comm_Port_Names.Text, "System Message");
            }
            else
            {
                try // just in case serial port is not open could also be acheved using if(serial.IsOpen)
                {
                    serial.Close();
                    Connect_btn.Content = "Connect";
                }
                catch
                {
                }
            }
        }
        private delegate void UpdateUiTextDelegate(string text);
        private void Recieve(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            recieved_data = serial.ReadExisting();
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), recieved_data);
        }
        private void WriteData(string text)
        {
            int index1 = text.IndexOf('\r');
            if (index1 == -1)
                line += text;
            else
            {
                string[] parts = text.Split("\r");
                line += parts[0];
                foreach (string s in line.Split(","))
                {
                    if (Int32.TryParse(s, out int i))
                        ints.Add(i);
                }
                if (record == true)
                {
                    int xd = (int)Math.Round(10 + ints[0] / xDivider);
                    int yd = (int)Math.Round(distanceCanvas.Height - ints[1] / yDivider);
                    xd = xd - (count * Convert.ToInt32(distanceCanvas.Width)) + (count * 10);
                    if (xd > Convert.ToInt32(distanceCanvas.Width))
                    {
                        distanceCanvas.Children.Clear();
                        distanceGraph.InitCanvas(distanceCanvas);
                        count++;
                    }
                    else
                        DrawPoint(xd, yd, distanceCanvas);
                    data.Add(line);
                }
                for (int j = 0; j < 9; j++)
                {
                    average[j] = average[j + 1];
                }
                //what happens in GIT if I change this??
                average[9] = ints[1];
                showDist.Text = Convert.ToString(Math.Round(average.Average(), 0));
                line = parts[1];
                ints.Clear();
            }
        }
        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            count = 0;
            distanceCanvas.Children.Clear();
            distanceGraph.InitCanvas(distanceCanvas);
            data.Clear();
            System.Windows.MessageBox.Show("Data Cleared", "Message:");
        }
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllLines(saveFileDialog1.FileName, data);
            }
        }
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            record = true;
            if (serial.IsOpen)
            {
                try
                {
                    serial.Write("1");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("failed to send" + ex);
                }
            }
            else
            {
            }
            distanceCanvas.Children.Clear();
            distanceGraph.InitCanvas(distanceCanvas);
            count = 0;
            data.Clear();
        }
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            record = false;
            if (serial.IsOpen)
            {
                try
                {
                    serial.Write("0");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("failed to send" + ex);
                }
            }
            else
            {
            }
        }
        private void DrawPoint(int x1, int y1, Canvas canvas)
        {
            Ellipse point = new Ellipse
            {
                Stroke = new SolidColorBrush(Colors.Black),
                Fill = new SolidColorBrush(Colors.Black),
                Width = 6,
                Height = 6,
                Margin = new Thickness(x1 - 3, y1 - 3, 0, 0),
            };
            canvas.Children.Add(point);
        }
        private void setY_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (setY.SelectedIndex)
            {
                case 0:
                    yDivider = 0.5;
                    break;
                case 1:
                    yDivider = 1;
                    break;
                case 2:
                    yDivider = 2;
                    break;
                default:
                    yDivider = 1;
                    break;
            }
        }
        private void setX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (setX.SelectedIndex)
            {
                case 0:
                    xDivider = 2;
                    break;
                case 1:
                    xDivider = 10;
                    break;
                case 2:
                    xDivider = 100;
                    break;
                default:
                    xDivider = 10;
                    break;
            }
        }
    }
}
