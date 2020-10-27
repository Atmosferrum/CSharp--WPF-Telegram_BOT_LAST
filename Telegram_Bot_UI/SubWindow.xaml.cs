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

namespace Telegram_Bot_UI
{
    /// <summary>
    /// Interaction logic for SubWindow.xaml
    /// </summary>
    public partial class SubWindow : Window
    {
        /// <summary>
        /// Sub Window INITIALIZATION
        /// </summary>
        public SubWindow()
        {
            InitializeComponent();

            var bc = new BrushConverter();

            foreach (MessageLog log in TelegramMessageClient.botLogCopy)
            {
                HistoryTree.Items.Add(new TextBlock
                {
                    Background = (Brush)bc.ConvertFrom("#03fcb6"),
                    Text = $"\n{log.ID}" +
                           $"\n{log.FirstName}" +
                           $"\n{log.Message}" +
                           $"\n{log.Time}",
                    HorizontalAlignment = HorizontalAlignment.Left
                });
            }
            
        }

        /// <summary>
        /// Additional CLOSE button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseHistory_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
