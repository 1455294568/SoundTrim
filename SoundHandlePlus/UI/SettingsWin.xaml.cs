using SoundHandlePlus.Models;
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
        private GeneralSettingsViewMode general;
        public SettingsWin()
        {
            InitializeComponent();
            general = new GeneralSettingsViewMode();
            this.DataContext = general;
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
                    _config.Language = languagebox.SelectedValue.ToString();
                    if(config.SaveConfig(_config))
                    {
                        MessageBox.Show(Properties.Resources.SaveTip1);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.SaveTip2);
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
            languagebox.SelectedValue = config.Config.Language.ToString();
        }
    }
}
