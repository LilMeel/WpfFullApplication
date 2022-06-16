using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace Wpf5Laba
{

    public partial class PollWindow : Window
    {
        Dictionary<int, string> results = new Dictionary<int, string>();
        RadioButton checkedRadio;
        int iteration;
        int sended_result = 0;

        public PollWindow()
        {
            InitializeComponent();
        }

        private void ViewValue_Click(object sender, RoutedEventArgs e)
        {
            return;
        }

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            CheckBox chBox = (CheckBox)sender;
            if (chBox.IsChecked.Value)
            {
                chBox.IsChecked = true;
                MenuCheckBox2.IsChecked = true;
            }
            else
            {
                chBox.IsChecked = false;
                MenuCheckBox2.IsChecked = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Всё готово!!!");
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radBut = (RadioButton)sender;
            radBut.IsChecked = true;
            checkedRadio = (RadioButton)sender;
        }

        private void Send_Info(object sender, RoutedEventArgs e)
        {
            sended_result++;
            results.Add(iteration + 1, (string)checkedRadio.Content);
            string payload_text = "Отправлено! \n\n";
            foreach (var kvp in results)
            {
                payload_text += kvp.Key.ToString() + ". " + kvp.Value + '\n';
            }
            MessageBox.Show(payload_text);
            iteration++;
        }
        private void WomenChecked(object sender, RoutedEventArgs e)
        {
            WomenPreSurname.Visibility = Visibility.Visible;
        }

        private void UnWomenChecked(object sender, RoutedEventArgs e)
        {
            WomenPreSurname.Visibility = Visibility.Hidden;
        }

        private void MenChecked(object sender, RoutedEventArgs e)
        {
            Army.Visibility = Visibility.Visible;
        }

        private void UnMenChecked(object sender, RoutedEventArgs e)
        {
            Army.Visibility = Visibility.Hidden;
        }

        private void EnableButton(object sender, RoutedEventArgs e)
        {
            if (Age.Text != "" && (MenRadBut.IsChecked == true || WomenRadBut.IsChecked == true) && (sended_result != 0))
            {
                MyButton.IsEnabled = true;
            }
        }

        private void OpenTextProfession(object sender, RoutedEventArgs e)
        {
            Profession.IsEnabled = true;
        }
    }

}
