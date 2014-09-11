using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;

namespace ExpenseApprovalApp.ViewModels
{
    public class ImagePageViewModel : INotifyPropertyChanged
    {
        private BitmapImage _image;

        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                _image = value; 
                OnPropertyChanged();
            }
        }

        internal void LoadImage(Windows.UI.Xaml.Media.Imaging.BitmapImage bitmapImage)
        {
            Image = bitmapImage;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
