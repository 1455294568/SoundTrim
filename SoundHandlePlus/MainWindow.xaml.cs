using Microsoft.Win32;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SoundHandlePlus.Models;
using SoundHandlePlus.UI;
using SoundHandlePlus.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoundHandlePlus
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mainView;

        private ConfigUtil config;
        public MainWindow()
        {
            InitializeComponent();
            mainView = new MainViewModel();
            config = ConfigUtil.GetInstance();
            this.DataContext = mainView;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ControlBtn_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                if (sender == minwin)
                {
                    this.WindowState = WindowState.Minimized;
                }
                else if (sender == clswin)
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            aboutBox about = new aboutBox();
            about.ShowDialog();
        }

        private void OpenFile()
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Multiselect = true
            };
            if (openFile.ShowDialog() == true)
            {
                filelist.Visibility = Visibility.Visible;
                if (mainView.Filenames.Count > 0)
                {
                    if (MessageBox.Show("队列里还有文件, 是覆盖,否追加?", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        mainView.Filenames.Clear();
                    }
                }
                foreach (var i in openFile.FileNames)
                {
                    if (!mainView.Filenames.Contains(i))
                        mainView.Filenames.Add(i);
                }

            }
        }

        private void Run()
        {
            loadingbar.Visibility = Visibility.Visible;
            var list = mainView.Filenames.ToList();
            Task.Factory.StartNew(() =>
            {
                int count = 0;
                foreach (var i in list)
                {
                    HandleFile(i);
                    this.Dispatcher.Invoke(() =>
                    {
                        mainView.Filenames.Remove(i);
                    });
                    count++;
                }
                this.Dispatcher.Invoke(() =>
                {
                    mainView.Filenames.Clear();
                    filelist.Visibility = Visibility.Hidden;
                    tips.Text = $"操作完成, 共完成了{count}项";
                    loadingbar.Visibility = Visibility.Hidden;
                });
                DelayUIWork(2000, delegate { tips.Text = "请选择文件"; });
            });
        }

        private void DelayUIWork(int millionseconds, Action action)
        {
            Timer timer = new Timer(millionseconds);
            timer.Elapsed += delegate
            {
                base.Dispatcher.Invoke(action);
                timer.Enabled = false;
            };
            timer.Enabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainView.Filenames.Remove((sender as Control).Tag.ToString());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (sender == addfilebtn)
            {
                OpenFile();
            }
            else if (sender == runbtn)
            {
                Run();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender == chooseFiles)
            {
                OpenFile();
            }
            else if (sender == silth)
            {
                SettingsWin settings = new SettingsWin();
                settings.ShowDialog();
            }
        }

        private void HandleFile(string filename)
        {
            try
            {
                FileInfo file = new FileInfo(filename);
                string path = System.IO.Path.Combine(file.Directory.FullName, "output");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                AudioFileReader waveFileReader = new AudioFileReader(filename);
                int bytesPerMillisecond = waveFileReader.WaveFormat.AverageBytesPerSecond / 1000;
                int startPos = GetSilenceTime(waveFileReader, SilenceLocation.Start, config.Config.SilenceThreshold);
                int endPos2 = GetSilenceTime(waveFileReader, SilenceLocation.End, config.Config.SilenceThreshold);
                endPos2 -= endPos2 % 4;
                WaveFileWriter waveFileWriter = new WaveFileWriter(filename + ".temp.wav", waveFileReader.WaveFormat);
                TrimSound(waveFileReader, waveFileWriter, startPos, endPos2);
                byte[] temp = new byte[bytesPerMillisecond * 500];
                waveFileWriter.Write(temp, 0, temp.Length);
                waveFileWriter.Close();
                waveFileReader.Close();
                WaveFileReader filereader = new WaveFileReader(filename + ".temp.wav");
                WaveFormat format = new WaveFormat(8000, 16, 1);
                MediaFoundationResampler resample = new MediaFoundationResampler(filereader, format);
                WaveFileWriter.CreateWaveFile(System.IO.Path.Combine(path, file.Name.Replace(file.Extension, ".wav")), resample);
                resample.Dispose();
                filereader.Close();
                File.Delete(filename + ".temp.wav");
            }
            catch (Exception)
            {
            }
        }

        private void TrimSound(AudioFileReader fileReader, WaveFileWriter waveFileWriter, int start, int end)
        {
            fileReader.Position = start;
            byte[] bytes = new byte[1024];
            while (fileReader.Position < end)
            {
                int bytereq = end - (int)fileReader.Position;
                if (bytereq > 0)
                {
                    int bytesToRead = Math.Min(bytereq, bytes.Length);
                    int bytesRead = fileReader.Read(bytes, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        waveFileWriter.Write(bytes, 0, bytesRead);
                    }
                }
            }
        }

        private int GetSilenceTime(AudioFileReader fileReader, SilenceLocation location, sbyte silenceThreshold = -20)
        {
            float[] buffer = new float[fileReader.WaveFormat.SampleRate * 4];
            int result = 0;
            bool volumeFound = false;
            bool eof = false;
            long oldPosition = fileReader.Position;
            while (!volumeFound && !eof)
            {
                int samplesRead = fileReader.Read(buffer, 0, buffer.Length);
                if (samplesRead == 0)
                {
                    eof = true;
                }
                for (int i = 0; i < samplesRead; i++)
                {
                    if (!IsSilence(buffer[i], silenceThreshold))
                    {
                        result = (int)fileReader.Position - (samplesRead - i) * 4;
                        if (location == SilenceLocation.Start)
                        {
                            volumeFound = true;
                            break;
                        }
                    }
                }
            }
            fileReader.Position = oldPosition;
            return result;
        }

        private bool IsSilence(float amplitude, sbyte threshold)
        {
            double dB = 20.0 * Math.Log10(Math.Abs(amplitude));
            return dB < (double)threshold;
        }
    }

    public enum SilenceLocation
    {
        Start,
        End
    }

}