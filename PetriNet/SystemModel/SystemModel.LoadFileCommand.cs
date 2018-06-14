using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace PetriNet
{
    public partial class SystemModel
    {
        public ICommand TheLoadFileCommand { get; }

        private void LoadFile(SystemModel sysModel)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = @"C:\Users\SimonHoulbrooke\source\repos\PetriNet\PetriNet\bin\Debug",
                CheckFileExists = true,
            };
            if (openFileDialog.ShowDialog() == true)
            {
                DataContractSerializer ser =
                    new DataContractSerializer(typeof(Displayable[]),
                          new Type[] {
                                              typeof(Arc),
                                              typeof(Node),
                                              typeof(Transition)
                          });

                var filename2open = openFileDialog.FileName;
                Stream stream = new FileStream(filename2open, FileMode.Open, FileAccess.Read);
                XmlDictionaryReader xdw = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());

                IEnumerable<Displayable> items = new List<Displayable>();
                items = (IEnumerable<Displayable>)ser.ReadObject(xdw);
                sysModel.Items = new ObservableCollection<Displayable>(items);
                sysModel.MakeConnections();
                sysModel.HookUpEvents();
                stream.Close();
            }
            else
            {
                MessageBox.Show("Open File Cancelld");
            }

        }



    }
}
