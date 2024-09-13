using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace KeyPressAutomation
{
    public partial class Form1 : Form
    {
        // Importar funciones para simular teclas
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        // Definir constantes para presionar y soltar teclas
        private const int KEYEVENTF_KEYDOWN = 0x0000; // Tecla presionada
        private const int KEYEVENTF_KEYUP = 0x0002;   // Tecla soltada

        private Timer timer;
        private bool isRunning = false;
        private int secondCounter = 0;  // Contador de segundos
        private int intervalInSeconds = 0;  // Almacenar el intervalo en segundos

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            InitializeCustomComponents();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Aquí puedes agregar código que quieras ejecutar cuando se cargue el formulario.
        }

        private void InitializeCustomComponents()
        {
            // Añadir teclas al ComboBox
            comboBoxKeys.Items.AddRange(new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Enter", "Escape", "Space", "Tab", "Backspace", "Delete", "Insert", "Home", "End", "PageUp", "PageDown", "Left", "Right", "Up", "Down" });
            comboBoxKeys.SelectedIndex = 0; // Seleccionar por defecto

            // Añadir modificadores al ComboBox
            comboBoxModifiers.SelectedIndex = 0; // Seleccionar por defecto

            // Inicializar el Timer
            timer = new Timer();
            timer.Tick += Timer_Tick;
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            secondCounter++;  // Incrementar el contador de segundos

            // Actualizar el label con el valor de segundos
            this.label_seconds.Text = secondCounter.ToString();

            // Solo presionar la tecla si el segundo actual es múltiplo del intervalo
            if (secondCounter % intervalInSeconds == 0)
            {
                this.label_seconds.ForeColor = System.Drawing.Color.Blue;

                // Obtener la tecla y el modificador seleccionados
                string selectedKey = comboBoxKeys.SelectedItem.ToString();
                string selectedModifier = comboBoxModifiers.SelectedItem.ToString();

                byte virtualKeyCode = GetVirtualKeyCode(selectedKey);
                byte modifierKeyCode = GetModifierKeyCode(selectedModifier);

                // Aplicar modificadores
                if (modifierKeyCode != 0)
                {
                    keybd_event(modifierKeyCode, 0, KEYEVENTF_KEYDOWN, 0); // Presionar modificador
                }

                // Simular la pulsación de la tecla
                keybd_event(virtualKeyCode, 0, KEYEVENTF_KEYDOWN, 0); // Presionar la tecla
                await Task.Delay(100); // Simular una pequeña espera sin bloquear el hilo
                keybd_event(virtualKeyCode, 0, KEYEVENTF_KEYUP, 0);   // Soltar la tecla

                // Liberar modificadores
                if (modifierKeyCode != 0)
                {
                    keybd_event(modifierKeyCode, 0, KEYEVENTF_KEYUP, 0); // Soltar modificador
                }
            }
            else
            {
                this.label_seconds.ForeColor = System.Drawing.Color.Black;
            }
        }

        private byte GetVirtualKeyCode(string key)
        {
            switch (key)
            {
                case "A": return 0x41;
                case "B": return 0x42;
                case "C": return 0x43;
                case "D": return 0x44;
                case "E": return 0x45;
                case "F": return 0x46;
                case "G": return 0x47;
                case "H": return 0x48;
                case "I": return 0x49;
                case "J": return 0x4A;
                case "K": return 0x4B;
                case "L": return 0x4C;
                case "M": return 0x4D;
                case "N": return 0x4E;
                case "O": return 0x4F;
                case "P": return 0x50;
                case "Q": return 0x51;
                case "R": return 0x52;
                case "S": return 0x53;
                case "T": return 0x54;
                case "U": return 0x55;
                case "V": return 0x56;
                case "W": return 0x57;
                case "X": return 0x58;
                case "Y": return 0x59;
                case "Z": return 0x5A;
                case "0": return 0x30;
                case "1": return 0x31;
                case "2": return 0x32;
                case "3": return 0x33;
                case "4": return 0x34;
                case "5": return 0x35;
                case "6": return 0x36;
                case "7": return 0x37;
                case "8": return 0x38;
                case "9": return 0x39;
                case "Enter": return 0x0D;
                case "Escape": return 0x1B;
                case "Space": return 0x20;
                case "Tab": return 0x09;
                case "Backspace": return 0x08;
                case "Delete": return 0x2E;
                case "Insert": return 0x2D;
                case "Home": return 0x24;
                case "End": return 0x23;
                case "PageUp": return 0x21;
                case "PageDown": return 0x22;
                case "Left": return 0x25;
                case "Right": return 0x27;
                case "Up": return 0x26;
                case "Down": return 0x28;
                default: return 0x00;
            }
        }

        private byte GetModifierKeyCode(string modifier)
        {
            switch (modifier)
            {
                case "Control": return 0x11;
                case "Alt": return 0x12;
                case "Shift": return 0x10;
                default: return 0x00;
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                int interval;
                if (int.TryParse(textBoxInterval.Text, out interval) && interval > 0)
                {
                    intervalInSeconds = interval;  // Guardar el intervalo ingresado
                    secondCounter = 0;  // Reiniciar el contador de segundos
                    timer.Interval = 1000; // Configurar el timer para ejecutar cada segundo
                    timer.Start();
                    isRunning = true;
                    this.label_running.Visible = true;
                    this.label_running.ForeColor = System.Drawing.Color.Blue;
                    this.label_running.Text = "Corriendo";
                }
                else
                {
                    MessageBox.Show("Por favor ingrese un intervalo válido mayor a 0.");
                }
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                timer.Stop();
                isRunning = false;
                this.label_running.Visible = true;
                this.label_running.ForeColor = System.Drawing.Color.Red;
                this.label_running.Text = "Parado";
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // URL que se abrirá cuando el usuario haga clic en el Label
            string url = "https://dev-melvin.com";
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }
}
}
