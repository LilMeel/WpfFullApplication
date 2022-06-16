using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.ComponentModel;
using System.Linq;

namespace Wpf5Laba
{

    public class UserData
    {
        private string data;
        public UserData(string _data)
        {
            data = _data;
        }

        public string GetData()
        {
            return data;
        }
    }
    
    public partial class MainWindow : Window
    {
        private List<UserData> UDs_ProductList = new List<UserData>();
        private List<UserData> UDs_FavouriteList = new List<UserData>();

        private List<UserData> last_UDs_ProductList = new List<UserData>();
        private List<UserData> last_UDs_FavouriteList = new List<UserData>();

        private List<UserData> new_UDs_ProductList = new List<UserData>();
        private List<UserData> new_UDs_FavouriteList = new List<UserData>();

        private bool isDataSaved = false;
        private bool record = false;
        private bool isAdded = false;

        string EXPORT_PATH = "export data.txt";
        string INPUT_PATH = "input.txt";

        public MainWindow()
        {
            InitializeComponent();
            SaveStartData();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RestoreData();
        }

        public bool IsAccpetByUser(string toDo, string receiveFunc)
        {
            if (MessageBox.Show($"{toDo}", $"{receiveFunc}", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void RestoreData()
        {
            string dir = Directory.GetCurrentDirectory();
            if (MessageBox.Show($"Восстановить данные последней сессии?","Restore Data", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DeletingAllData(null, null);
                var sortedFiles = new DirectoryInfo($"{dir}").GetFiles()
                                      .OrderBy(f => f.LastWriteTime)
                                      .ToList();
                sortedFiles.Reverse();
                string currentFile;
                List<string> logStrings = new List<string>();

                for (int i = 0; i < sortedFiles.Count; i++)
                {
                    currentFile = sortedFiles[i].ToString();
                    if (currentFile.IndexOf("log") == -1)
                    {
                        continue;
                    }
                    logStrings.Add(currentFile);
                }

                if (!IsAccpetByUser($"Вы хотите импортировать из этого пути: {Path.GetFullPath(EXPORT_PATH)} ?", "CheckDirectory"))
                {
                    InputBox inputBox = new InputBox();
                    string tempExportPath = inputBox.Show("Укажите путь для экспорта:", true, logStrings);
                    if (tempExportPath != null)
                    {
                        EXPORT_PATH = tempExportPath;
                    }
                }

                StreamReader sr = new StreamReader(EXPORT_PATH);

                ReadFromFile(sr, UDs_ProductList);
                ReadFromFile(sr, UDs_FavouriteList);

                FillTheArea(UDs_ProductList, UDs_FavouriteList);

            }
        }

        private void SaveStartData()
        {
            FillList(new_UDs_ProductList, new_UDs_FavouriteList);
        }

        private void ExitingProgramm(object sender, RoutedEventArgs e)
        {
            if (!IsAccpetByUser("Вы точно хотите выйти из приложения?", "Exit Program"))
            {
                return;
            }

            this.isDataSaved = true;
            this.Close();
        }

        private void DataWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            try
            {
                MainWindow CloseSender = (MainWindow)sender;
                if(CloseSender.isDataSaved)
                {
                    e.Cancel = false;
                }
                else 
                {
                    throw new Exception();
                }

                SavingData(null, null);
                if (record)
                {
                    var dateTime = DateTime.Now.ToString().Replace(":", ".");
                    FileInfo fileForExtraSave = new FileInfo($"log-{dateTime}.txt");
                    if (!fileForExtraSave.Exists)
                    {
                        FileStream fs = File.Create($"log-{dateTime}.txt");
                        fs.Close();
                    }

                    WriteDataInFile(fileForExtraSave, last_UDs_ProductList, last_UDs_FavouriteList);
                }
                
            }
            catch
            {
                return;
            }
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

        private void ReturnCheck(int ret)
        {
            if (ret != 0)
            {
                record = true;
            }
            else
            {
                record = false;
            }
        }

        private void ViewValue_Click(object sender, RoutedEventArgs e)
        {
            return;
        }

        private int FillList(List<UserData> products, List<UserData> favourites)
        {
            int i = 0;
            foreach (ComboBoxItem item in ProductList.Items)
            {
                UserData tempData = new UserData(item.Content.ToString());
                products.Add(tempData);
                last_UDs_ProductList.Add(tempData);
                i++;
            }

            int j = 0;
            foreach (ListBoxItem item in FavouriteBoard.Items)
            {
                UserData tempData = new UserData(item.Content.ToString());
                favourites.Add(tempData);
                last_UDs_FavouriteList.Add(tempData);
                j++;
            }

            return i + j;
        }

        private void SavingData(object sender, RoutedEventArgs e)
        {
            last_UDs_ProductList.Clear();
            last_UDs_FavouriteList.Clear();

            int ret = FillList(UDs_ProductList, UDs_FavouriteList);
            ReturnCheck(ret);

            if (last_UDs_FavouriteList.Count != 0 || last_UDs_ProductList.Count != 0)
            {
                LoadLastData.IsEnabled = true;
            }
            else
            {
                LoadLastData.IsEnabled = false;
            }

            DeleteAllItems(DeleteAllBoxItems, null);
            DeleteAllItems(DeleteAllFavouriteItems, null);

            if (sender != null)
            {
                MessageBox.Show($"Данные сохранены! ");
            }
        }

        private void AddAProductToList(object sender, RoutedEventArgs e)
        {
            isAdded = true;

            ListBoxItem new_cat = new ListBoxItem();
            new_cat.Content = InputNewProduct.Text;
            if (FavouriteBoard.Items.Count % 2 == 0)
            {
                new_cat.Background = (Brush)FavouriteBoard.Resources["ListItemStyle"];
            }
            FavouriteBoard.Items.Add(new_cat);

        }

        private void AddAProductToComboBox(object sender, RoutedEventArgs e)
        {
            string name;
            isAdded = true;
            Button checkForDouble = new Button();
            try
            {
                checkForDouble = (Button)sender;
            }
            catch{}

            ComboBoxItem newComboItem = new ComboBoxItem();

            if (checkForDouble.Name.IndexOf("Double") != -1)
            {
                name = ProductList.SelectedItem.ToString();
            }
            else
            {
                name = InputNewProduct.Text;
            }

            if (name.Length < 2)
            {
                return;
            }

           int iter = name.IndexOf(" ");
           name = name.Substring(iter + 1);
           newComboItem.Content = name;
           ProductList.Items.Add(newComboItem);
        }

        private void SelctedComboBox(object sender, RoutedEventArgs e)
        {
            ComboBoxItem currentItem = (ComboBoxItem)sender;
            currentItem.FontFamily = (FontFamily)this.Resources["SelectedItemFontFamily"];
            Style style = (Style)this.Resources["SelectedItemFontSize"];
            foreach(var setter in style.Setters.OfType<Setter>())
            {
                currentItem.FontSize = (double)setter.Value;
            }
            currentItem.FontWeight = (FontWeight)this.Resources["SelectedItemFontWeight"];
        }

        private void ComboSelectionChaging(object sender, RoutedEventArgs e)
        {
            ComboBox currentItem = (ComboBox)sender;
            foreach(ComboBoxItem box in currentItem.Items)
            {
                if (!box.IsSelected)
                {
                    box.FontFamily = (FontFamily)this.Resources["CommonItemFontFamily"];
                    Style style = (Style)this.Resources["CommonItemFontSize"];
                    foreach (var setter in style.Setters.OfType<Setter>())
                    {
                        box.FontSize = (double)setter.Value;
                    }
                    box.FontWeight = (FontWeight)this.Resources["CommonItemFontWeight"];
                }
            }
        }

        private void DoubleItem(object sender, RoutedEventArgs e)
        {
            Button temp_but = (Button)sender;
            if (temp_but.Name.IndexOf("Box") != -1)
            {
                AddAProductToComboBox(sender, null);
            }
            else
            {
                ListBoxItem newPanel = (ListBoxItem)FavouriteBoard.SelectedItem;
                ListBoxItem new_cat = new ListBoxItem();
                try
                {
                    if(newPanel == null)
                    {
                        MessageBox.Show("Не выбран элемент для дублирования");
                        return;
                    }
                    string name = newPanel.ToString();
                    int iter = name.IndexOf(" ");
                    name = name.Substring(iter);

                    new_cat.Content = name;
                    if (FavouriteBoard.Items.Count % 2 == 0)
                    {
                        new_cat.Background = Brushes.LightGray;
                    }
                    FavouriteBoard.Items.Add(new_cat);
                }
                catch (Exception ex)
                {
                    return;
                }
                
            }
        }


        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            Button temp_but = (Button)sender;
            if (temp_but.Name.IndexOf("Box") != -1)
            {
                ProductList.Items.Remove(ProductList.SelectedItem);
            }
            else
            {
                FavouriteBoard.Items.Remove(FavouriteBoard.SelectedItem);
            }
        }

        private void DeleteAllItems(object sender, RoutedEventArgs e)
        {
            Button temp_but = (Button)sender;
            if (temp_but.Name.IndexOf("Box") != -1)
            {
                int count = ProductList.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    ProductList.Items.Remove(ProductList.Items[0]);
                }
            }
            else
            {
                int count = FavouriteBoard.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    FavouriteBoard.Items.Remove(FavouriteBoard.Items[0]);
                }
            }
        }

        private void WriteDataInFile(FileInfo fhndl, List<UserData> products, List<UserData> favourites)
        {
            using (StreamWriter sw = new StreamWriter(fhndl.Name, false))
            {
                sw.WriteLine("Product list items: ");

                foreach (UserData user in products)
                {
                    sw.WriteLine(user.GetData());
                }

                sw.WriteLine();

                sw.WriteLine("Favourite list items: ");

                foreach (UserData user in favourites)
                {
                    sw.WriteLine(user.GetData());
                }
            }
        }

        private void ExportingData(object sender, RoutedEventArgs e)
        {
            //SavingData(null, null);
            if (!IsAccpetByUser($"Вы хотите импортировать из этого пути: {Path.GetFullPath(EXPORT_PATH)} ?", "CheckDirectory"))
            {
                InputBox inputBox = new InputBox();
                string tempExportPath = inputBox.Show("Укажите путь для экспорта:", false);
                if (tempExportPath != null)
                {
                    EXPORT_PATH = tempExportPath;
                }

            }

            FileInfo fileForExport = new FileInfo(EXPORT_PATH);

            //if (!fileForExport.Exists)
            //{
            //    FileStream fp = fileForExport.Create();
            //    fp.Close();
            //}

            WriteDataInFile(fileForExport, UDs_ProductList, UDs_FavouriteList);
        }

        private void ReadFromFile(StreamReader sr, List<UserData> buffer)
        {
            sr.ReadLine();
            string tempData = sr.ReadLine();

            while (tempData != null && tempData != "")
            {
                buffer.Add(new UserData(tempData));
                tempData = sr.ReadLine();
                if(tempData != null)
                {
                    if (tempData.IndexOf("Example Input") != -1)
                    {
                        break;
                    }
                }
            }
        }

        private int FillTheArea(List<UserData> products, List<UserData> favourites)
        {
            int i = 0;
            foreach (UserData data in products)
            {
                ComboBoxItem tempComboItem = new ComboBoxItem();
                tempComboItem.Content = data.GetData();
                ProductList.Items.Add(tempComboItem);
                i++;
            }

            ProductList.SelectedItem = ProductList.Items[0];

            int j = 0;
            foreach (UserData data in favourites)
            {
                ListBoxItem tempListItem = new ListBoxItem();
                if (FavouriteBoard.Items.Count % 2 == 0)
                {
                    tempListItem.Background = Brushes.LightGray;
                }
                tempListItem.Content = data.GetData();
                FavouriteBoard.Items.Add(tempListItem);
                j++;
            }

            return i + j;
        }

        private void ImportingData(object sender, RoutedEventArgs e)
        {
            DeletingAllData(null, null);
            if (!IsAccpetByUser("Вы точно хотите импортировать данные?", "Import Data") && isAdded)
            {
                return;
            }

            if (!IsAccpetByUser($"Вы хотите импортировать из этого пути: {Path.GetFullPath(EXPORT_PATH)} ?", "CheckDirectory"))
            {
                InputBox inputBox = new InputBox();
                string tempExportPath = inputBox.Show("Укажите путь для экспорта:", false);
                if(tempExportPath != null)
                {
                    EXPORT_PATH = tempExportPath;
                }
            }

            UDs_ProductList.Clear();
            UDs_FavouriteList.Clear();

            FileInfo fileForExport = new FileInfo(EXPORT_PATH);

            if (!fileForExport.Exists)
            {
                return;
            }

            using(StreamReader sr = new StreamReader(fileForExport.Name))
            {
                ReadFromFile(sr, UDs_ProductList);
                ReadFromFile(sr, UDs_FavouriteList);
            }

            FillTheArea(UDs_ProductList, UDs_FavouriteList);

            UDs_FavouriteList.Clear();

            LoadLastData.IsEnabled = false;

        }

        private void CountAllData(object sender, RoutedEventArgs e)
        {
            int lengthAllData = UDs_FavouriteList.Count + UDs_ProductList.Count;
            MessageBox.Show($"Всего записей {lengthAllData}", "Счётчик элементов");
            //winMain.Content = $"Всего записей {lengthAllData}";
            //winMain.Width = 200;
            //winMain.Height = 200;
            //winMain.HorizontalAlignment = HorizontalAlignment.Center;
            //Button close = new Button();
            //winMain.AddChild();
            //close.Click += new RoutedEventArgs(this.CloseAdditionalWindow);

            //winMain.Show();

        }

        private void LoadingLastData(object sender, RoutedEventArgs e)
        {
            int ret = FillTheArea(last_UDs_ProductList, last_UDs_FavouriteList);
            ReturnCheck(ret);
        }

        private void ClearingData(object sender, RoutedEventArgs e)
        {
            if (!IsAccpetByUser("Вы точно хотите отчистить данные?", "Clear Data"))
            {
                return;
            }
            DeletingAllData(null, null);
            FillTheArea(new_UDs_ProductList, new_UDs_FavouriteList);
        }

        private void DeletingAllData(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                if (!IsAccpetByUser("Вы точно хотите почистить все данные", "Delete all Data"))
                {
                    return;
                }
                DeleteAllItems(DeleteAllBoxItems, null);
                DeleteAllItems(DeleteAllFavouriteItems, null);
            }
        }

        private void DeletingLastData(object sender, RoutedEventArgs e)
        {
            var dir = Directory.GetCurrentDirectory();

            foreach(string file in Directory.GetFiles(dir))
            {
                if (file.IndexOf("log") != -1)
                {
                    File.Delete(file);
                }
            }
        }

        private void PollPage(object sender, RoutedEventArgs e)
        {
            PollWindow pollWindow = new PollWindow();
            pollWindow.Show();
        }

        private void TestingInput(object sender, RoutedEventArgs e)
        {
            StreamReader sr = new StreamReader(INPUT_PATH);
            sr.ReadLine();
            string tempData = sr.ReadLine();
            int i = 0;
            while (tempData != null)
            {
                if (tempData.IndexOf("Example Input") != -1)
                {
                    i = 1;
                    tempData = sr.ReadLine();
                }

                switch (i)
                {
                    case 1:
                        InputNewProduct.Text = tempData;
                        i++;
                        break;
                    case 2:
                        DoubleItem(DoubleBoxItem, null);
                        i++;
                        break;
                    case 3:
                        DoubleItem(DoubleListItem, null);
                        i++;
                        break;
                }
                tempData = sr.ReadLine();
            }
        }

        private void DataFromResourses(object sender, RoutedEventArgs e)
        {
            StreamReader sr = new StreamReader(INPUT_PATH);
            new_UDs_ProductList.Clear();
            new_UDs_FavouriteList.Clear();

            ReadFromFile(sr, new_UDs_ProductList);
            ReadFromFile(sr, new_UDs_FavouriteList);

            ClearingData(null, null);
        }
    }
}
