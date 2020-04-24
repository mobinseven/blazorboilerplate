using BlazorBoilerplate.Shared.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace BlazorBoilerplate.Shared.Dto.Logistique
{
    public class LocationDto : Location, INotifyPropertyChanged
    {
        private bool _selected = true;

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}