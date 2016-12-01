using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using NAudio.Wave;
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

namespace WpfSampleWaveIn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<double> animatedX = new List<double>();
        List<double> animatedY = new List<double>();
        EnumerableDataSource<double> animatedDataSource = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                animatedX.Add(i);
                animatedY.Add(0);
            }
            EnumerableDataSource<double> xSrc = new EnumerableDataSource<double>(animatedX);
            xSrc.SetXMapping(x => x);
            animatedDataSource = new EnumerableDataSource<double>(animatedY);
            animatedDataSource.SetYMapping(y => y);

            // Adding graph to plotter
            plotter.AddLineGraph(new CompositeDataSource(xSrc, animatedDataSource),
                new Pen(Brushes.Magenta, 1),
                new PenDescription("Wave"));

            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var deviceInfo = WaveIn.GetCapabilities(i);
                this.label1.Content = String.Format("Device {0}: {1}, {2} channels",
                    i, deviceInfo.ProductName, deviceInfo.Channels);
            }

            var waveIn = new WaveIn()
            {
                DeviceNumber = 0, // Default
            };
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.WaveFormat = new WaveFormat(sampleRate: 8000, channels: 1);
            waveIn.StartRecording();
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            short sample = (short)((e.Buffer[1] << 8) | e.Buffer[0]);
            double sample32 = sample / 32768f;
            ProcessSample(sample32);
        }

        private void ProcessSample(double data)
        {
            animatedY.RemoveAt(0);
            animatedY.Add(data);

            // 
            animatedDataSource.RaiseDataChanged();
        }
    }
}
