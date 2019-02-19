using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundHandlePlus.Models
{
    public class MainViewModel : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

    }
}
