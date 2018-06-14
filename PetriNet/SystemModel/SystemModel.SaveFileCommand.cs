using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        public ICommand TheSaveFileCommand { get; }

        private void SaveFile(SystemModel sysModel)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                InitialDirectory = @"C:\Users\SimonHoulbrooke\source\repos\PetriNet\PetriNet\bin\Debug",
                AddExtension = true,
                DefaultExt = "xml",
                Filter = "*.xml(XML)|*.*",
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                DataContractSerializer ser =
                new DataContractSerializer(typeof(Displayable[]),
                      new Type[] {
                          typeof(Arc),
                          typeof(Node),
                          typeof(Transition)
                      });

                using (Stream stream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
                {
                    XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8);
                    ser.WriteObject(stream, Items.ToArray());
                }
            }
            else
            {
                MessageBox.Show("Save File Cancelled");
            }
        }

   }
}
