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
using Microsoft.Win32;
using System.IO;
using System.Globalization;

namespace Lab4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            App.LanguageChanged += LanguageChanged;

            CultureInfo currLang = App.Language;

            //Заполняем меню смены языка:
            menuLanguage.SelectionChanged += ChangeLanguageClick;
            foreach (var lang in App.Languages)
            {
                ComboBoxItem menuLang = new ComboBoxItem();
                menuLang.Content = lang.DisplayName;
                menuLang.Tag = lang;
                menuLanguage.Items.Add(menuLang);
            }
        }

        private void LanguageChanged(Object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;

            //Отмечаем нужный пункт смены языка как выбранный язык
            foreach (ComboBoxItem i in menuLanguage.Items)
            {
                CultureInfo ci = i.Tag as CultureInfo;
            }
        }

        private void ChangeLanguageClick(Object sender, EventArgs e)
        {
            ComboBox mi = sender as ComboBox;
            if (mi != null)
            {
                ComboBoxItem cmbItem = mi.SelectedItem as ComboBoxItem;
                CultureInfo lang =  cmbItem.Tag as CultureInfo;
                if (lang != null)
                {
                    App.Language = lang;
                }
            }


        }

        FileStream fileStream;

        private void fontNameComBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Main.Selection.IsEmpty)
            {
                Main.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, (FontFamily)fontNameComBox.SelectedItem);
            }
        }

        private void fontSizeComBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Main.Selection.IsEmpty)
            {
                Main.Selection.ApplyPropertyValue(Inline.FontSizeProperty, fontSizeComBox.SelectedItem);
            }
        }

        private void fontColorComBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Main.Selection.IsEmpty)
            {
                Brush brush = null;
                switch (fontColorComBox.SelectedIndex)
                {
                    case 0: brush = Brushes.Black; break;
                    case 1: brush = Brushes.Green; break;
                    case 2: brush = Brushes.Yellow; break;
                    case 3: brush = Brushes.Red; break;
                    case 4: brush = Brushes.Blue; break;
                    case 5: brush = Brushes.Brown; break;
                }
                Main.Selection.ApplyPropertyValue(Inline.ForegroundProperty, brush);
            }
        }

        private void BoldSt_Checked(object sender, RoutedEventArgs e)
        {
            if (!Main.Selection.IsEmpty)
            {
                Main.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold);
            }
        }

        private void ItalicSt_Checked(object sender, RoutedEventArgs e)
        {
            if (!Main.Selection.IsEmpty)
            {
                Main.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Italic);
            }
        }

        private void UnderlineSt_Checked(object sender, RoutedEventArgs e)
        {
            if (!Main.Selection.IsEmpty)
            {
                Main.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            }
        }

        private void BoldSt_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!Main.Selection.IsEmpty)
            {
                Main.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Normal);
            }
        }

        private void ItalicSt_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!Main.Selection.IsEmpty)
            {
                Main.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Normal);
            }
        }

        private void UnderlineSt_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!Main.Selection.IsEmpty)
            {
                Main.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
            }
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                window.Title = dlg.FileName;
                fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(Main.Document.ContentStart,
                    Main.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
                fileStream.Close();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                fileStream = new FileStream(dlg.FileName, FileMode.Create);
                TextRange range = new TextRange(Main.Document.ContentStart,
                    Main.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
                fileStream.Close();
            }
        }

        private void Main_TextChanged(object sender, TextChangedEventArgs e)
        {
            string richText = new TextRange(Main.Document.ContentStart,
               Main.Document.ContentEnd).Text;
            Count.Text = Convert.ToString(richText.Length);
        }

        private void Main_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                window.Title = files[0];
                fileStream = new FileStream(files[0], FileMode.Open);
                TextRange range = new TextRange(Main.Document.ContentStart,
                    Main.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
                fileStream.Close();
            }
        }
    }
}
