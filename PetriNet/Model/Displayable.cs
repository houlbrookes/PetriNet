using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace PetriNet
{
    /// <summary>
    /// Adds the capability to draw but I may drop this in favour of doing the animation in XAML
    /// </summary>
    [DataContract(IsReference = true), 
        KnownType(typeof(Arc)), 
        KnownType(typeof(Transition)), 
        KnownType(typeof(Node))]
    public class Displayable : INotifyPropertyChanged
    {
        string _ref = "";
        [DataMember]
        public string Ref { get => _ref; set => UpdateProperty(ref _ref, value); }
        double x = 0D;
        [DataMember]
        public double X { get => x; set => UpdateProperty(ref x, value); }
        double y = 0D;
        [DataMember]
        public double Y { get => y; set => UpdateProperty(ref y, value); }
        bool _isSelected = false;
        [XmlIgnore]
        public bool IsSelected { get => _isSelected; set => UpdateProperty(ref _isSelected, value); }

        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateProperty<T>(ref T item, T value, [CallerMemberName] string propertyName = "")
        {
            item = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
