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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Win32;

namespace Telegram_Bot_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TelegramMessageClient client;

        /// <summary>
        /// Main Window INITIALIZATION
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            client = new TelegramMessageClient(this);

            MessageBox.ItemsSource = client.BotMessageLog;
        }

        /// <summary>
        /// SEND BUTTON method (if there's chosen user and text in Input Fielde)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (InputField?.Text != "" && SenderID?.Text != "")
                SendMessage();
        }

        /// <summary>
        /// SEND Message by pressing Enter (if there's chosen user and text in Input Fielde)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEnterKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && InputField?.Text != "" && SenderID?.Text != "")
                SendMessage();
        }

        /// <summary>
        /// Method to SEND Message to chosen user
        /// </summary>
        private void SendMessage()
        {
            client.SendMessage(InputField.Text, SenderID.Text);
            InputField.Clear();
        }

        /// <summary>
        /// Method to Initiate History SAVE when closing Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            client.SaveHistory();
        }

        /// <summary>
        /// Method to ATTACH button (to attach chosen Document and send it to chosen User)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AttachButton_Click(object sender, RoutedEventArgs e)
        {
            if (SenderID.Text != "")
            {
                OpenFileDialog openFildeDialog = new OpenFileDialog();

                if (openFildeDialog.ShowDialog() == true)
                    TelegramMessageClient.UploadDocument(Convert.ToInt64(SenderID.Text), openFildeDialog.FileName);
            }            
        }

        /// <summary>
        /// Method to OPEN History Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            SubWindow subWindow = new SubWindow();
            subWindow.Show();
        }
    }
}
