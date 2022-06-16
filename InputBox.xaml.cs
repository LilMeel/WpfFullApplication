using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Wpf5Laba
{
    /// <summary>
    /// Логика взаимодействия для InputBox.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        // строка, которая будет возвращена форме запроса
        private static string path;
        private bool State;

        public InputBox()
        {
            InitializeComponent();
        }
        public string Show(string inputBoxText, bool state, List<string> items = null)
        {
            State = state;
            MainLabel.Content = inputBoxText;
            if (!state)
            {
                DateList.Visibility = Visibility.Hidden;
                ShowDialog();
            }
            else
            {
                MainTextBox.Visibility = Visibility.Hidden;
                AdditionsList.Visibility = Visibility.Hidden;
                MainTextBox.Visibility = Visibility.Hidden;
                foreach(string item in items)
                {
                    ComboBoxItem newItem = new ComboBoxItem();
                    newItem.Content = item;
                    DateList.Items.Add(newItem);
                }
                ShowDialog();
            }
            return path;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (!State)
            {
                foreach (ComboBoxItem item in AdditionsList.Items)
                {
                    if (item.IsSelected)
                    {
                        path = item.Content.ToString();
                        break;
                    }
                }
                int checkUserFolder = MainTextBox.Text.IndexOf("%userprofile%");

                if (checkUserFolder != -1) 
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + MainTextBox.Text.Substring(checkUserFolder + 13) + path;
                }
                else if (File.Exists(MainTextBox.Text+path))
                {
                    path = MainTextBox.Text+path;
                }
                else
                {
                    MessageBox.Show("Такого пути не существует");
                }
            }
            else
            {
                foreach (ComboBoxItem item in DateList.Items)
                {
                    if (item.IsSelected)
                    {
                        if (File.Exists(item.Content.ToString()))
                        {
                            path = item.Content.ToString();
                            break;
                        }
                    }
                }
            }
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            path = string.Empty;
            Close();
        }
    }
}
