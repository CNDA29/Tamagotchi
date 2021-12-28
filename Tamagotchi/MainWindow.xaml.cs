using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Tamagotchi
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer t1;
        double decrementoEnergia = 1.0;
        double decrementoAlimento = 1.0;
        double decrementoDiversion = 1.0;
        string nombreTamagotchi;
        double puntuacion = 0;
        int contadorVecesAlimento = 0;
        int contadorVecesJuego = 0;
        int contadorVecesDescanso = 0;
        string[] ranking1, ranking2, ranking3, ranking4, ranking5;

        SoundPlayer sonidoComer = new SoundPlayer();
        SoundPlayer sonidoDescansar = new SoundPlayer();
        SoundPlayer sonidoJugar = new SoundPlayer();
        SoundPlayer sonidoGameOver = new SoundPlayer();
        SoundPlayer sonidoLogro = new SoundPlayer();

        public MainWindow()
        {
            InitializeComponent();
            //Ventana Bienvenida
            VentanaBienvenida pantallaInicio = new VentanaBienvenida(this);
            pantallaInicio.ShowDialog();
            //Declarar location sonidos (Cambiarlo)
            sonidoComer.SoundLocation = Environment.CurrentDirectory.Replace("\\bin\\Debug", "") + "\\SonidoComer.wav";
            sonidoJugar.SoundLocation = Environment.CurrentDirectory.Replace("\\bin\\Debug", "") + "\\SonidoJugar.wav";
            sonidoDescansar.SoundLocation = Environment.CurrentDirectory.Replace("\\bin\\Debug", "") + "\\SonidoDescansar.wav";
            sonidoGameOver.SoundLocation = Environment.CurrentDirectory.Replace("\\bin\\Debug", "") + "\\SonidoGameOver.wav";
            sonidoLogro.SoundLocation = Environment.CurrentDirectory.Replace("\\bin\\Debug", "") + "\\SonidoLogro.wav";
            leerRanking();
            t1 = new DispatcherTimer();
            t1.Interval = TimeSpan.FromMilliseconds(1500);
            t1.Tick += new EventHandler(reloj);
            t1.Start();
        }

        private void reloj(object sender, EventArgs e)
        {
            puntuacion += 1;
            tbPuntuacionActual.Text = "Puntuación actual: " + puntuacion;
            comprobarLogrosPuntuacion();
            this.pbEnergía.Value -= decrementoEnergia;
            this.pbAlimento.Value -= decrementoAlimento;
            this.pbDiversion.Value -= decrementoDiversion;
            comprobarBarras();
            decrementoEnergia += 0.10;
            decrementoAlimento += 0.10;
            decrementoDiversion += 0.11;
        }

        private void gameOver()
        {
            t1.Stop();
            sonidoGameOver.Play();
            btnComer.IsEnabled = false;
            btnDescansar.IsEnabled = false;
            btnJugar.IsEnabled = false;
            MessageBoxResult result = MessageBox.Show(
           "Game Over\nPuntuación final: " +puntuacion+ "\n¿Desea volver a jugar?", "IPO2 Tamagotchi",
           MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.No:
                    actualizarRanking();
                    this.Close();
                    break;
                case MessageBoxResult.Yes:
                    actualizarRanking();
                    leerRanking();
                    reiniciar();
                    break;
            }
        }

        private void btnJugar_Click(object sender, RoutedEventArgs e)
        {
            this.pbDiversion.Value += 10;
            contadorVecesJuego += 1;
            //LOGRO 5
            if(contadorVecesDescanso > 0 && contadorVecesAlimento > 0 && contadorVecesJuego > 0 && imLogro5.Visibility == Visibility.Hidden){
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 5 completado\n'Este logro consiste en realizar al menos 1 vez cada una de las acciones que hay.'\n\nBonificación: +5 puntos", "IPO2 Tamagotchi");
                t1.Start();
                imLogro5.Visibility = Visibility.Visible;
                txtLogro5.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 5;
            }
            //LOGRO 6
            if ((contadorVecesJuego+contadorVecesDescanso+contadorVecesAlimento > 14) && imLogro6.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 6 completado\n'Este logro consiste en alcanzar 14 interacciones sumando las tres acciones que hay.'\n\nBonificación: +15 puntos", "IPO2 Tamagotchi");
                t1.Start();
                imLogro6.Visibility = Visibility.Visible;
                txtLogro6.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 15;
            }
            //LOGRO 12
            if (contadorVecesJuego > 20 && imLogro12.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 12 completado\n'Este logro consiste en hacer jugar al personaje más de 20 veces.'\n\nBonificación: +25 puntos", "IPO2 Tamagotchi");
                t1.Start();
                imLogro12.Visibility = Visibility.Visible;
                txtLogro12.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 25;
            }
            //Animacion jugar
            btnJugar.IsEnabled = false;
            btnComer.IsEnabled = false;
            btnDescansar.IsEnabled = false;
            sonidoJugar.Play();
            Storyboard sbJugar = (Storyboard)this.Resources["animacionJugar"];
            sbJugar.Completed += new EventHandler(finJugar);
            sbJugar.Begin();
        }

        private void btnComer_Click(object sender, RoutedEventArgs e)
        {
            this.pbAlimento.Value += 10;
            contadorVecesAlimento += 1;
            //LOGRO 5
            if (contadorVecesDescanso > 0 && contadorVecesAlimento > 0 && contadorVecesJuego > 0 && imLogro5.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 5 completado\n'Este logro consiste en realizar al menos 1 vez cada una de las acciones que hay.'\n\nBonificación: +5 puntos", "IPO2 Tamagotchi");
                t1.Start();
                imLogro5.Visibility = Visibility.Visible;
                txtLogro5.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 5;
            }
            //LOGRO 6
            if ((contadorVecesJuego + contadorVecesDescanso + contadorVecesAlimento > 14) && imLogro6.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 6 completado\n'Este logro consiste en alcanzar 15 interacciones sumando las tres acciones que hay.'\n\nBonificación: +15 puntos", "IPO2 Tamagotchi");
                t1.Start();
                imLogro6.Visibility = Visibility.Visible;
                txtLogro6.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 15;
            }
            //LOGRO 10
            if (contadorVecesAlimento > 20 && imLogro10.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 10 completado\n'Este logro consiste en dar de comer al personaje más de 20 veces.'\n\nBonificación: +25 puntos", "IPO2 Tamagotchi");
                t1.Start();
                imLogro10.Visibility = Visibility.Visible;
                txtLogro10.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 25;
            }
            //Animacion comer
            btnJugar.IsEnabled = false;
            btnComer.IsEnabled = false;
            btnDescansar.IsEnabled = false;
            sonidoComer.Play();
            Storyboard sbComer = (Storyboard)this.Resources["animacionComer"];
            sbComer.Completed += new EventHandler(finComer);
            sbComer.Begin();
        }

        private void btnDescansar_Click(object sender, RoutedEventArgs e)
        {
            this.pbEnergía.Value += 10;
            contadorVecesDescanso += 1;
            //LOGRO 5
            if (contadorVecesDescanso > 0 && contadorVecesAlimento > 0 && contadorVecesJuego > 0 && imLogro5.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 5 completado\n'Este logro consiste en realizar al menos 1 vez cada una de las acciones que hay.'\n\nBonificación: +5 puntos", "IPO2 Tamagotchi");
                t1.Start();
                imLogro5.Visibility = Visibility.Visible;
                txtLogro5.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 5;
            }
            //LOGRO 6
            if ((contadorVecesJuego + contadorVecesDescanso + contadorVecesAlimento > 14) && imLogro6.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 6 completado\n'Este logro consiste en alcanzar 15 interacciones sumando las tres acciones que hay.'\n\nBonificación: +15 puntos", "IPO2 Tamagotchi");
                t1.Start();
                imLogro6.Visibility = Visibility.Visible;
                txtLogro6.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 15;
            }
            //LOGRO 11
            if (contadorVecesDescanso > 20 && imLogro11.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 11 completado\n'Este logro consiste en hacer que descanse el personaje más de 20 veces.'\n\nBonificación: +25 puntos", "IPO2 Tamagotchi");
                t1.Start();
                imLogro11.Visibility = Visibility.Visible;
                txtLogro11.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 25;
            }
            //Animacion descansar
            btnJugar.IsEnabled = false;
            btnComer.IsEnabled = false;
            btnDescansar.IsEnabled = false;
            sonidoDescansar.Play();
            Storyboard sbDescansar = (Storyboard)this.Resources["animacionDescansar"];
            sbDescansar.Completed += new EventHandler(finDescansar);
            sbDescansar.Begin();
        }

        private void finComer(Object sender, EventArgs e)
        {
            btnComer.IsEnabled = true;
            btnDescansar.IsEnabled = true;
            btnJugar.IsEnabled = true;
            sonidoComer.Stop();
        }

        private void finDescansar(Object sender, EventArgs e)
        {
            btnComer.IsEnabled = true;
            btnDescansar.IsEnabled = true;
            btnJugar.IsEnabled = true;
            sonidoDescansar.Stop();
        }

        private void finJugar(Object sender, EventArgs e)
        {
            btnComer.IsEnabled = true;
            btnDescansar.IsEnabled = true;
            btnJugar.IsEnabled = true;
            sonidoJugar.Stop();
        }

        private void acercaDe(object sender, MouseButtonEventArgs e)
        {
            t1.Stop();
            MessageBoxResult result = MessageBox.Show(
            "Práctica realizada por:\nClaudio Díaz\n\n ¿Desea Salir?", "IPO2 Tamagotchi",
            MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    this.Close();
                    break;
                case MessageBoxResult.No:
                    t1.Start();
                    break;
            }
        }

        public void setNombre(string nombre)
        {
            this.nombreTamagotchi = nombre;
            this.tbMensajes.Text = "Bienvenido " + nombreTamagotchi;//Mensaje abajo, poner
        }

        private void inicioArrastrarGorro(object sender, MouseButtonEventArgs e)
        {
            DataObject dataO = new DataObject(((Image)sender));
            DragDrop.DoDragDrop((Image)sender, dataO, DragDropEffects.Move);
        }

        private void inicioArrastreBigote(object sender, MouseButtonEventArgs e)
        {
            DataObject dataO = new DataObject(((Image)sender));
            DragDrop.DoDragDrop((Image)sender, dataO, DragDropEffects.Move);
        }

        private void iniciarArrastrarCorona(object sender, MouseButtonEventArgs e)
        {
            DataObject dataO = new DataObject(((Image)sender));
            DragDrop.DoDragDrop((Image)sender, dataO, DragDropEffects.Move);
        }

        private void inicioArrastrarGafas(object sender, MouseButtonEventArgs e)
        {
            DataObject dataO = new DataObject(((Image)sender));
            DragDrop.DoDragDrop((Image)sender, dataO, DragDropEffects.Move);
        }

        private void inicioArrastrarSombrero(object sender, MouseButtonEventArgs e)
        {
            DataObject dataO = new DataObject(((Image)sender));
            DragDrop.DoDragDrop((Image)sender, dataO, DragDropEffects.Move);
        }

        private void añadirObjeto(object sender, DragEventArgs e)
        {
            Image aux = (Image)e.Data.GetData(typeof(Image));
            //LOGRO 3
            if (imLogro3.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 3 completado\n'Este logro consiste en cambiar al personaje con algún coleccionable.'\n\nBonificación: +5 puntos", "IPO2 Tamagotchi");
                t1.Start();
                imLogro3.Visibility = Visibility.Visible;
                txtLogro3.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 5;
            }
            switch (aux.Name)
            {
                case "imGorroMini":
                    imGorro.Visibility = Visibility.Visible;
                    imCascoVikingo.Visibility = Visibility.Hidden;
                    imGorroEstadio.Visibility = Visibility.Hidden;
                    imCorona.Visibility = Visibility.Hidden;
                    imSombreroVaquero.Visibility = Visibility.Hidden;
                    break;
                case "imBigoteMini":
                    imBigote.Visibility = Visibility.Visible;
                    break;
                case "imCoronaMini":
                    imCorona.Visibility = Visibility.Visible;
                    imGorro.Visibility = Visibility.Hidden;
                    imCascoVikingo.Visibility = Visibility.Hidden;
                    imGorroEstadio.Visibility = Visibility.Hidden;
                    imSombreroVaquero.Visibility = Visibility.Hidden;
                    break;
                case "imGafasMini":
                    imGafas.Visibility = Visibility.Visible;
                    break;
                case "imSombreroVaqueroMini":
                    imSombreroVaquero.Visibility = Visibility.Visible;
                    imCascoVikingo.Visibility = Visibility.Hidden;
                    imGorroEstadio.Visibility = Visibility.Hidden;
                    imGorro.Visibility = Visibility.Hidden;
                    imCorona.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void comprobarBarras()
        {
            //Energia
            if (pbEnergía.Value <= 30)
            {
                imBocadilloSueno.Visibility = Visibility.Visible;
                imSueno.Visibility = Visibility.Visible;
            }
            else
            {
                imBocadilloSueno.Visibility = Visibility.Hidden;
                imSueno.Visibility = Visibility.Hidden;
            }
            //Alimento
            if (pbAlimento.Value <= 30)
            {
                imBocadilloHambre.Visibility = Visibility.Visible;
                imHambre.Visibility = Visibility.Visible;
            }
            else
            {
                imBocadilloHambre.Visibility = Visibility.Hidden;
                imHambre.Visibility = Visibility.Hidden;
            }
            //Diversion
            if (pbDiversion.Value <= 30)
            {
                imBocadilloJuego.Visibility = Visibility.Visible;
                imJuego.Visibility = Visibility.Visible;
            }
            else
            {
                imBocadilloJuego.Visibility = Visibility.Hidden;
                imJuego.Visibility = Visibility.Hidden;
            }
            if (pbDiversion.Value == 0 && pbEnergía.Value == 0 && pbAlimento.Value == 0)
            {
                gameOver();
            }
        }

        private void CambiarFondoMarino(object sender, MouseButtonEventArgs e)
        {
            this.imFondo.Source = ((Image)sender).Source;
            imRespirador.Visibility = Visibility.Visible;
            imGorroEstadio.Visibility = Visibility.Hidden;
            imCascoVikingo.Visibility = Visibility.Hidden;
            casco.Visibility = Visibility.Hidden;
            casco2.Visibility = Visibility.Hidden;
            //LOGRO 2
            if (imLogro2.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 2 completado\n'Este logro consiste en cambiar el fondo al menos 1 vez.'\n\nBonificación: +5 puntos", "IPO2 Tamagotchi");
                t1.Start();
                imLogro2.Visibility = Visibility.Visible;
                txtLogro2.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 5;
            }
        }

        private void cambiarFondo(object sender, MouseButtonEventArgs e)
        {
            this.imFondo.Source = ((Image)sender).Source;
            imRespirador.Visibility = Visibility.Hidden;
            imGorroEstadio.Visibility = Visibility.Hidden;
            imCascoVikingo.Visibility = Visibility.Hidden;
            casco.Visibility = Visibility.Visible;
            casco2.Visibility = Visibility.Visible;
            //LOGRO 2
            if (imLogro2.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 2 completado\n'Este logro consiste en cambiar el fondo al menos 1 vez.'\n\nBonificación: +5 puntos", "IPO2 Tamagotchi");
                t1.Start();
                imLogro2.Visibility = Visibility.Visible;
                txtLogro2.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 5;
            }
        }

        private void CambiarFondoHelado(object sender, MouseButtonEventArgs e)
        {
            this.imFondo.Source = ((Image)sender).Source;
            imRespirador.Visibility = Visibility.Hidden;
            imGorroEstadio.Visibility = Visibility.Hidden;
            imCascoVikingo.Visibility = Visibility.Hidden;
            casco.Visibility = Visibility.Hidden;
            casco2.Visibility = Visibility.Hidden;
        }

        private void ocultarBigote(object sender, MouseButtonEventArgs e)
        {
            imBigote.Visibility = Visibility.Hidden;
        }

        private void ocultarGorro(object sender, MouseButtonEventArgs e)
        {
            imGorro.Visibility = Visibility.Hidden;
        }

        private void ocultarCorona(object sender, MouseButtonEventArgs e)
        {
            imCorona.Visibility = Visibility.Hidden;
        }

        private void ocultarGafas(object sender, MouseButtonEventArgs e)
        {
            imGafas.Visibility = Visibility.Hidden;
        }

        private void ocultarSombrero(object sender, MouseButtonEventArgs e)
        {
            imSombreroVaquero.Visibility = Visibility.Hidden;
        }

        private void btnPausa_Click(object sender, RoutedEventArgs e)
        {
            switch (btnPausa.Content)
            {
                case "Pausar":
                    pausar();
                    break;
                case "Reanudar":
                    reanudar();
                    break;
            }
        }

        private void pausar()
        {
            t1.Stop();
            btnComer.IsEnabled = false;
            btnDescansar.IsEnabled = false;
            btnJugar.IsEnabled = false;
            btnPausa.Content = "Reanudar";
        }

        private void reanudar()
        {
            t1.Start();
            btnComer.IsEnabled = true;
            btnDescansar.IsEnabled = true;
            btnJugar.IsEnabled = true;
            btnPausa.Content = "Pausar";
        }

        private void comprobarLogrosPuntuacion()
        {
            //LOGRO 1
            if(puntuacion >= 20 && imLogro1.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 1 completado\n'Este logro consiste en alcanzar 20 puntos.'\n\nBonificación: +5 puntos\nPremio: Gafas", "IPO2 Tamagotchi");
                t1.Start();
                imLogro1.Visibility = Visibility.Visible;
                txtLogro1.Foreground = new SolidColorBrush(Colors.Green);
                imGafasMini.IsEnabled = true;
                puntuacion += 5;
            }
            //LOGRO 4
            if (puntuacion >= 50 && imLogro4.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 4 completado\n'Este logro consiste en alcanzar 50 puntos.'\n\nBonificación: +10 puntos\nPremio: Sombrero vaquero", "IPO2 Tamagotchi");
                t1.Start();
                imLogro4.Visibility = Visibility.Visible;
                imSombreroVaqueroMini.IsEnabled = true;
                txtLogro4.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 10;
            }
            //LOGRO 7
            if (puntuacion >= 100 && imLogro7.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 7 completado\n'Este logro consiste en alcanzar 100 puntos.'\n\nBonificación: +20 puntos\nPremio: Color traje azul", "IPO2 Tamagotchi");
                t1.Start();
                imLogro7.Visibility = Visibility.Visible;
                imAmericanaMini.IsEnabled = true;
                txtLogro7.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 20;
            }
            //LOGRO 8
            if (puntuacion >= 150 && imLogro8.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 8 completado\n'Este logro consiste en alcanzar 150 puntos.'\n\nBonificación: +50 puntos\nPremio: Color traje granate", "IPO2 Tamagotchi");
                t1.Start();
                imLogro8.Visibility = Visibility.Visible;
                imTrajeGranateMini.IsEnabled = true;
                txtLogro8.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 50;
            }
            //LOGRO 9
            if (puntuacion >= 250 && imLogro9.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 9 completado\n'Este logro consiste en alcanzar 250 puntos.'\n\nBonificación: +100 puntos\nPremio: Ambiente estadio", "IPO2 Tamagotchi");
                t1.Start();
                imLogro9.Visibility = Visibility.Visible;
                imEstadioMini.IsEnabled = true;
                txtLogro9.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 100;
            }
            //LOGRO 13
            if (puntuacion >= 425 && imLogro13.Visibility == Visibility.Hidden)
            {
                t1.Stop();
                sonidoLogro.Play();
                MessageBox.Show("¡¡ENHORABUENA!!\n\nLogro 13 completado\n'Este logro consiste en alcanzar 425 puntos.'\n\nBonificación: +250 puntos\nPremio: Ambiente vikingo", "IPO2 Tamagotchi");
                t1.Start();
                imLogro13.Visibility = Visibility.Visible;
                imAmbienteVikingoMini.IsEnabled = true;
                txtLogro13.Foreground = new SolidColorBrush(Colors.Green);
                puntuacion += 250;
            }
        }

        //Este método sirve para reiniciar todo el programa sin necesidad de cerrarlo.
        private void reiniciar()
        {
            pbAlimento.Value = 100;
            pbEnergía.Value = 100;
            pbDiversion.Value = 100;
            puntuacion = 0;
            tbPuntuacionActual.Text = "Puntuación actual: " + puntuacion;
            btnComer.IsEnabled = true;
            btnDescansar.IsEnabled = true;
            btnJugar.IsEnabled = true;
            decrementoEnergia = 1.0;
            decrementoAlimento = 1.0;
            decrementoDiversion = 1.0;
            imBocadilloHambre.Visibility = Visibility.Hidden;
            imHambre.Visibility = Visibility.Hidden;
            imBocadilloSueno.Visibility = Visibility.Hidden;
            imSueno.Visibility = Visibility.Hidden;
            imBocadilloJuego.Visibility = Visibility.Hidden;
            imJuego.Visibility = Visibility.Hidden;
            imBigote.Visibility = Visibility.Hidden;
            imGorroEstadio.Visibility = Visibility.Hidden;
            imCascoVikingo.Visibility = Visibility.Hidden;
            imCorona.Visibility = Visibility.Hidden;
            imGorro.Visibility = Visibility.Hidden;
            imSombreroVaquero.Visibility = Visibility.Hidden;
            imGafas.Visibility = Visibility.Hidden;
            imRespirador.Visibility = Visibility.Hidden;
            casco.Visibility = Visibility.Hidden;
            casco2.Visibility = Visibility.Hidden;
            traje.Fill = new SolidColorBrush(Colors.Black);
            mangaDer.Fill = new SolidColorBrush(Colors.Black);
            mangaIzq.Fill = new SolidColorBrush(Colors.Black);
            imGafasMini.IsEnabled = false;
            imSombreroVaqueroMini.IsEnabled = false;
            imAmericanaMini.IsEnabled = false;
            imTrajeGranateMini.IsEnabled = false;
            imEstadioMini.IsEnabled = false;
            imAmbienteVikingoMini.IsEnabled = false;
            contadorVecesAlimento = 0;
            contadorVecesDescanso = 0;
            contadorVecesJuego = 0;
            imLogro1.Visibility = Visibility.Hidden;
            txtLogro1.Foreground = new SolidColorBrush(Colors.White);
            imLogro2.Visibility = Visibility.Hidden;
            txtLogro2.Foreground = new SolidColorBrush(Colors.White);
            imLogro3.Visibility = Visibility.Hidden;
            txtLogro3.Foreground = new SolidColorBrush(Colors.White);
            imLogro4.Visibility = Visibility.Hidden;
            txtLogro4.Foreground = new SolidColorBrush(Colors.White);
            imLogro5.Visibility = Visibility.Hidden;
            txtLogro5.Foreground = new SolidColorBrush(Colors.White);
            imLogro6.Visibility = Visibility.Hidden;
            txtLogro6.Foreground = new SolidColorBrush(Colors.White);
            imLogro7.Visibility = Visibility.Hidden;
            txtLogro7.Foreground = new SolidColorBrush(Colors.White);
            imLogro8.Visibility = Visibility.Hidden;
            txtLogro8.Foreground = new SolidColorBrush(Colors.White);
            imLogro9.Visibility = Visibility.Hidden;
            txtLogro9.Foreground = new SolidColorBrush(Colors.White);
            imLogro10.Visibility = Visibility.Hidden;
            txtLogro10.Foreground = new SolidColorBrush(Colors.White);
            imLogro11.Visibility = Visibility.Hidden;
            txtLogro11.Foreground = new SolidColorBrush(Colors.White);
            imLogro12.Visibility = Visibility.Hidden;
            txtLogro12.Foreground = new SolidColorBrush(Colors.White);
            imLogro13.Visibility = Visibility.Hidden;
            txtLogro13.Foreground = new SolidColorBrush(Colors.White);
            this.imFondo.Source = ((Image)imFondoHelado).Source;
            t1.Start();
        }

        private void cambiarTrajeAzul(object sender, MouseButtonEventArgs e)
        {
            traje.Fill = new SolidColorBrush(Colors.CadetBlue);
            mangaDer.Fill = new SolidColorBrush(Colors.CadetBlue);
            mangaIzq.Fill = new SolidColorBrush(Colors.CadetBlue);
        }

        private void cambiarTrajeGranate(object sender, MouseButtonEventArgs e)
        {
            var bc = new BrushConverter();
            traje.Fill = (Brush)bc.ConvertFrom("#FF6E0D0D");
            mangaDer.Fill = (Brush)bc.ConvertFrom("#FF6E0D0D");
            mangaIzq.Fill = (Brush)bc.ConvertFrom("#FF6E0D0D");
        }

        private void ocultarTraje(object sender, MouseButtonEventArgs e)
        {
            traje.Fill = new SolidColorBrush(Colors.Black);
            mangaDer.Fill = new SolidColorBrush(Colors.Black);
            mangaIzq.Fill = new SolidColorBrush(Colors.Black);
        }

        private void CambiarFondoEstadio(object sender, MouseButtonEventArgs e)
        {
            this.imFondo.Source = ((Image)sender).Source;
            imGorroEstadio.Visibility = Visibility.Visible;
            imRespirador.Visibility = Visibility.Hidden;
            imCascoVikingo.Visibility = Visibility.Hidden;
            imGorro.Visibility = Visibility.Hidden;
            imCorona.Visibility = Visibility.Hidden;
            imSombreroVaquero.Visibility = Visibility.Hidden;
            casco.Visibility = Visibility.Hidden;
            casco2.Visibility = Visibility.Hidden;
        }

        private void CambiarFondoVikingo(object sender, MouseButtonEventArgs e)
        {
            this.imFondo.Source = ((Image)sender).Source;
            imCascoVikingo.Visibility = Visibility.Visible;
            imGorroEstadio.Visibility = Visibility.Hidden;
            imRespirador.Visibility = Visibility.Hidden;
            imGorro.Visibility = Visibility.Hidden;
            imCorona.Visibility = Visibility.Hidden;
            imSombreroVaquero.Visibility = Visibility.Hidden;
            casco.Visibility = Visibility.Hidden;
            casco2.Visibility = Visibility.Hidden;
        }

        private void leerRanking()
        {
            string[] ranking = System.IO.File.ReadAllText(Environment.CurrentDirectory.Replace("\\bin\\Debug", "") + "\\Ranking.txt").Split(';');
            ranking1 = ranking[0].Split('-'); ranking2 = ranking[1].Split('-'); ranking3 = ranking[2].Split('-');
            ranking4 = ranking[3].Split('-'); ranking5 = ranking[4].Split('-');
            tbRanking1.Text = "1. " +ranking1[0]+ "-" +ranking1[1]+ "pts";
            tbRanking2.Text = "2. " + ranking2[0] + "-" + ranking2[1] + "pts";
            tbRanking3.Text = "3. " + ranking3[0] + "-" + ranking3[1] + "pts";
            tbRanking4.Text = "4. " + ranking4[0] + "-" + ranking4[1] + "pts";
            tbRanking5.Text = "5. " + ranking5[0] + "-" + ranking5[1] + "pts";
        }

        private void actualizarRanking()
        {
            string rankingAux;
            if (puntuacion > Int16.Parse(ranking5[1]))
            {
                if(puntuacion > Int16.Parse(ranking5[1]) && puntuacion < Int16.Parse(ranking4[1]))
                {
                    ranking5[0] = nombreTamagotchi; ranking5[1] = "" + puntuacion;
                } else if(puntuacion > Int16.Parse(ranking4[1]) && puntuacion < Int16.Parse(ranking3[1]))
                {
                    ranking5[0] = ranking4[0]; ranking5[1] = ranking4[1];
                    ranking4[0] = nombreTamagotchi; ranking4[1] = "" + puntuacion;
                } else if (puntuacion > Int16.Parse(ranking3[1]) && puntuacion < Int16.Parse(ranking2[1]))
                {
                    ranking5[0] = ranking4[0]; ranking5[1] = ranking4[1];
                    ranking4[0] = ranking3[0]; ranking4[1] = ranking3[1];
                    ranking3[0] = nombreTamagotchi; ranking3[1] = "" + puntuacion;
                } else if (puntuacion > Int16.Parse(ranking2[1]) && puntuacion < Int16.Parse(ranking1[1]))
                {
                    ranking5[0] = ranking4[0]; ranking5[1] = ranking4[1];
                    ranking4[0] = ranking3[0]; ranking4[1] = ranking3[1];
                    ranking3[0] = ranking2[0]; ranking3[1] = ranking2[1];
                    ranking2[0] = nombreTamagotchi; ranking2[1] = "" + puntuacion;
                } else
                {
                    ranking5[0] = ranking4[0]; ranking5[1] = ranking4[1];
                    ranking4[0] = ranking3[0]; ranking4[1] = ranking3[1];
                    ranking3[0] = ranking2[0]; ranking3[1] = ranking2[1];
                    ranking2[0] = ranking1[0]; ranking2[1] = ranking1[1];
                    ranking1[0] = nombreTamagotchi; ranking1[1] = "" + puntuacion;
                }
            }
            rankingAux = ranking1[0] + "-" + ranking1[1] + ";" + ranking2[0] + "-" + ranking2[1] + ";" + ranking3[0] + "-" + ranking3[1] + ";" + ranking4[0] + "-" + ranking4[1] + ";" + ranking5[0] + "-" + ranking5[1];
            System.IO.File.WriteAllText(Environment.CurrentDirectory.Replace("\\bin\\Debug", "") + "\\Ranking.txt", rankingAux);
        }
    }
}
