using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Windows.Threading;
using System.IO;
using System.Windows.Forms;
using System.Windows.Markup;

namespace HCSR05_Distance_Sensor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FlowDocument flowDoc = new FlowDocument();
        Paragraph para = new Paragraph();
        SerialPort serial = new SerialPort();
        string ?recieved_data;
        
        public MainWindow()
        {
            InitializeComponent();
            Connect_btn.Content = "Connect";
        }

        private void Connect_Comms(object sender, RoutedEventArgs e)
        {
            if (Connect_btn.Content == "Connect")
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
                serial.Open();

                //Sets button State and Creates function call on data recieved
                Connect_btn.Content = "Disconnect";
                serial.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(Recieve);

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
            // Collecting the characters received to our 'buffer' (string).
            recieved_data = serial.ReadExisting();
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), recieved_data);
        }
        private void WriteData(string text)
        {
            // Assign the value of the recieved_data to the RichTextBox.
            para.Inlines.Add(text);
            flowDoc.Blocks.Add(para);
            showData.Document = flowDoc;
            
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            showData.SelectAll();
            showData.Selection.Text = "";
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            TextRange range = new(showData.Document.ContentStart, showData.Document.ContentEnd);
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, range.Text);
            }
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (serial.IsOpen)
            {
                try
                {
                    
                    serial.Write("1");
                }
                catch (Exception ex)
                {
                    para.Inlines.Add("Failed to SEND data" + "\n" + ex + "\n");
                    flowDoc.Blocks.Add(para);
                    showData.Document = flowDoc;
                }
            }
            else
            {
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (serial.IsOpen)
            {
                try
                {

                    serial.Write("0");
                }
                catch (Exception ex)
                {
                    para.Inlines.Add("Failed to SEND data" + "\n" + ex + "\n");
                    flowDoc.Blocks.Add(para);
                    showData.Document = flowDoc;
                }
            }
            else
            {
            }
        }
    }
}
