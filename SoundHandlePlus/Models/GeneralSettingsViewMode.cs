using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundHandlePlus.Models
{
    public class GeneralSettingsViewMode : ViewBase
    {
        private List<KeyValuePair<string, string>> languages;

        public List<KeyValuePair<string, string>> Languages
        {
            get { return languages; }
        }

        public GeneralSettingsViewMode()
        {
            languages = new List<KeyValuePair<string, string>>();
            languages.Add(new KeyValuePair<string, string>("中文", "zh-CN"));
            languages.Add(new KeyValuePair<string, string>("English", "en"));
        }
    }
}
