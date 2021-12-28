using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tamagotchi
{
    /// <summary>
    /// Lógica de interacción para VentanaBienvenida.xaml
    /// </summary>
    public partial class VentanaBienvenida : Window
    {
        MainWindow padre;

        public VentanaBienvenida(MainWindow padre)
        {
            InitializeComponent();
            this.padre = padre;
        }

        private void eventoEmpezar(object sender, RoutedEventArgs e)
        {
            padre.setNombre(this.tbNombre.Text);
            this.Close();
        }
    }
}
