using SoundHandlePlus.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundHandlePlus.Utils
{
    public class ConfigUtil
    {
        private static ConfigUtil _instance = new ConfigUtil();

        public ConfigModel Config { get; set; }

        private string confpath = "Config.json";

        private ConfigUtil()
        {
            try
            {
                if(File.Exists(confpath))
                {
                    var json = File.ReadAllText(confpath);
                    Config = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigModel>(json);
                    
                }
                else
                {
                    Config = new ConfigModel();
                    Config.SilenceThreshold = -20;
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(Config, Newtonsoft.Json.Formatting.Indented);
                    StreamWriter sw = new StreamWriter(confpath);
                    sw.WriteLine(json);
                    sw.Close();
                }
            }
            catch(Exception ex)
            {
                Config = new ConfigModel();
                Config.SilenceThreshold = -20;
            }
        }

        public static ConfigUtil GetInstance()
        {
            return _instance;
        }

        public bool SaveConfig(ConfigModel _config)
        {
            try
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(_config, Newtonsoft.Json.Formatting.Indented);
                StreamWriter sw = new StreamWriter(confpath);
                sw.WriteLine(json);
                sw.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
