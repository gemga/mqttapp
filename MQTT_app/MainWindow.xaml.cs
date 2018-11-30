using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace MQTT_app
{
    /// <summary>
    /// Lógica de MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
     
        // Directorio donde se busca el programa de mosquitto (donde se instala por default)
        string mosquitto_path = @"C:\Program Files\mosquitto\";

        public MainWindow()
        {
            InitializeComponent();

            // Si la aplicación no detecta el directorio...
            if (!Directory.Exists(mosquitto_path))
            {
                // Le pide al usuario que especifique su lugar de instalación
                MessageBox.Show("La aplicación no encontró el programa de mosquitto en la carpeta esperada. Por favor selecciona la carpeta donde está instalado mosquitto.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    mosquitto_path = dialog.FileName + "\\";
                    Debug.WriteLine("Mosquitto installation path: " + mosquitto_path);
                } else
                {
                    // Si no existe el directorio, cierra la aplicación
                    Debug.WriteLine("Mosquitto installation path not found. Closing application...");
                    Application.Current.Shutdown();

                    return;
                }
            }
            // Si encuentra el archivo, se ejecuta mosquitto.exe
            if (File.Exists(mosquitto_path + "mosquitto.exe")) { 
                ProcessStartInfo mosquitto = new ProcessStartInfo(mosquitto_path + "mosquitto.exe");
                mosquitto.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(mosquitto);
            } else
            {
                // Si no encuentra el programa, cierra la aplicación
                MessageBox.Show("mosquitto.exe no encontrado. Asegúrate de tener instalado mosquitto y haber seleccionado la carpeta correcta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine("Mosquitto executable not found. Closing application...");
                Application.Current.Shutdown();

                return;
            }
        }

        // Listener del botón Publicar. Cada vez que presionamos Publicar se ejecuta esta función
        public void Button_Click_Publish(object sender, RoutedEventArgs e)
        {
            // Toma los datos de los campos en la interfaz
            string broker = Broker.Text;
            string topic = Topic.Text;
            string message = Message.Text;

            // Verificar que ninguno de los campos se encuentre vacío
            if (String.IsNullOrEmpty(broker) || String.IsNullOrEmpty(topic) || String.IsNullOrEmpty(message)) {
                MessageBox.Show("Asegúrate que los campos de Broker, Topic y Message no estén vacíos", "Error de publicación", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Creamos un string con el path de mosquitto_pub.exe
            string program = mosquitto_path + "mosquitto_pub.exe";
            // Y preparamos los parámetros
            string parameters = $"-h {broker} -t {topic} -m \"{message}\"";

            // Enviamos los strings a una función que ejecuta el programa
            Start_Process(program, parameters, false);

            Debug.WriteLine("Publication parameters: " + parameters);
        }

        // Listener del botón Suscribir. Cada vez que presionamos Suscribir se ejecuta esta función
        public void Button_Click_Subscribe(object sender, RoutedEventArgs e)
        {
            // Toma los datos de los campos en la interfaz
            string broker = Broker.Text;
            string topic = Topic.Text;

            // Verificar que ninguno de los campos se encuentre vacío
            if (String.IsNullOrEmpty(broker) || String.IsNullOrEmpty(topic)) { 
                MessageBox.Show("Asegúrate que los campos de Broker y Topic no estén vacíos", "Error de suscripción", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Creamos un string con el path de mosquitto_sub.exe
            string program = mosquitto_path + "mosquitto_sub.exe";
            // Preparamos los parámetros
            string parameters = $"-h {broker} -t {topic}";

            // Enviamos los strings a una función que ejecuta el programa
            Start_Process(program, parameters);

            Debug.WriteLine("Subscription parameters: " + parameters);
        }

        // Programa que ejecuta un programa en un proceso nuevo
        private void Start_Process(string program, string parameters, bool showWindow = true)
        {
            ProcessStartInfo process = new ProcessStartInfo(program, parameters);

            // mosquitto_pub.exe crea una ventana bastante rápido y no es muy estético.
            // Asignamos este parámetro para que no se vea.
            if (!showWindow)
                process.WindowStyle = ProcessWindowStyle.Hidden;

            Process.Start(process);
        }
    }
}
