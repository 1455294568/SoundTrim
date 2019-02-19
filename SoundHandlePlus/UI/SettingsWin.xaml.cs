using SoundHandlePlus.Utils;
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
using System.Windows.Shapes;

namespace SoundHandlePlus.UI
{
    /// <summary>
    /// SettingsWin.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWin : Window
    {
        private ConfigUtil config;
        public SettingsWin()
        {
            InitializeComponent();
            config = ConfigUtil.GetInstance();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(sender == savebtn)
            {
                try
                {
                    var _config = config.Config;
                    _config.SilenceThreshold = sbyte.Parse(siltextbox.Text);
                    if(config.SaveConfig(_config))
                    {
                        MessageBox.Show("保存成功");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("异常");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if(sender == clsbtn)
            {
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            siltextbox.Text = config.Config.SilenceThreshold.ToString();
        }
    }
}
