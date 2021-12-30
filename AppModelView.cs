using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace grafs
{
    public class AppModelView : INotifyPropertyChanged
    {
        public GraphModel Graph { get; set; }

        public DelegateCommand ShowSmth { get; set; }
        
        public DelegateCommand OpenAjMatFile { get; set; }
        public DelegateCommand OpenIncMatFile { get; set; }
        public DelegateCommand OpenGraphCodeFile { get; set; }

        public string testText = "test";

        public AppModelView()
        {
            Graph = new GraphModel();
            ShowSmth = new DelegateCommand(ShowIt);

            OpenAjMatFile = new DelegateCommand(Graph.openAjMatFile);
            OpenIncMatFile = new DelegateCommand(Graph.openIncMatFile);
            OpenGraphCodeFile = new DelegateCommand(Graph.openGraphCodeFile);
        }

        public void ShowIt(object param) 
        {
            MessageBox.Show("sssss");

        }

        public void mouse_down_on_vert( object sender, MouseButtonEventArgs e) 
        {
            MessageBox.Show("AAAAA");
        }

        public void mouse_down() 
        {
            MessageBox.Show("TEST!");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
