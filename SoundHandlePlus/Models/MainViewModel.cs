using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundHandlePlus.Models
{
    public class MainViewModel : ViewBase
    {
        private ObservableCollection<string> filenames;

        public ObservableCollection<string> Filenames
        {
            get { return filenames; }
        }

        public MainViewModel()
        {
            filenames = new ObservableCollection<string>();
            filenames.CollectionChanged += Filenames_CollectionChanged;
        }

        private void Filenames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Filenames));
        }

    }
}
