using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
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

        //public static long count { get; set; }

        //static TextSelection rbBuffer;

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
                CultureInfo lang = cmbItem.Tag as CultureInfo;
                if (lang != null)
                {
                    App.Language = lang;
                }
            }


        }

        FileStream fileStream;

        private void fontNameComBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!richTextBox.Selection.IsEmpty)
            {
                richTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, (FontFamily)fontNameComBox.SelectedItem);
            }
        }

        private void fontSizeComBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!richTextBox.Selection.IsEmpty)
            {
                richTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, fontSizeComBox.SelectedItem);
            }
        }

        private void fontColorComBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!richTextBox.Selection.IsEmpty)
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
                richTextBox.Selection.ApplyPropertyValue(Inline.ForegroundProperty, brush);
            }
        }

        private void BoldSt_Checked(object sender, RoutedEventArgs e)
        {
            if (!richTextBox.Selection.IsEmpty)
            {
                richTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold);
            }
        }

        private void ItalicSt_Checked(object sender, RoutedEventArgs e)
        {
            if (!richTextBox.Selection.IsEmpty)
            {
                richTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Italic);
            }
        }

        private void UnderlineSt_Checked(object sender, RoutedEventArgs e)
        {
            if (!richTextBox.Selection.IsEmpty)
            {
                richTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            }
        }

        private void BoldSt_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!richTextBox.Selection.IsEmpty)
            {
                richTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Normal);
            }
        }

        private void ItalicSt_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!richTextBox.Selection.IsEmpty)
            {
                richTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Normal);
            }
        }

        private void UnderlineSt_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!richTextBox.Selection.IsEmpty)
            {
                richTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
            }
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                window.Title = dlg.FileName;
                if (History.Items.Count == 5)
                {
                    History.Items.RemoveAt(0);
                }
                History.Items.Add(dlg.FileName);
                fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(richTextBox.Document.ContentStart,
                    richTextBox.Document.ContentEnd);
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
                TextRange range = new TextRange(richTextBox.Document.ContentStart,
                    richTextBox.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
                fileStream.Close();
            }
        }

        void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string richText = new TextRange(richTextBox.Document.ContentStart,
               richTextBox.Document.ContentEnd).Text;
            //count = richText.Length - 2;
            Count.Content = Convert.ToString(richText.Length - 2);
        }

        private void richTextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                window.Title = files[0];
                fileStream = new FileStream(files[0], FileMode.Open);
                TextRange range = new TextRange(richTextBox.Document.ContentStart,
                    richTextBox.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
                fileStream.Close();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        static int windowCount = 1;

        private void New_Click(object sender, RoutedEventArgs e)
        {
            MainWindow richTextBoxWindow = new MainWindow();
            richTextBoxWindow.Title = "Window" + windowCount++;
            richTextBoxWindow.Show();
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            if (!richTextBox.Selection.IsEmpty)
            {
                richTextBox.Cut();
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (!richTextBox.Selection.IsEmpty)
            {
                richTextBox.Copy();
            }
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            richTextBox.Paste();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!richTextBox.Selection.IsEmpty)
            {
                richTextBox.Selection.Text = "";
            }
        }

        private void richTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!richTextBox.Selection.IsEmpty)
            {
                if ((richTextBox.Selection.GetPropertyValue(Run.FontWeightProperty) is FontWeight))
                {
                    BoldSt.IsChecked = (((FontWeight)
                        richTextBox.Selection.GetPropertyValue(Run.FontWeightProperty))
                        == FontWeights.Bold);
                }
                if ((richTextBox.Selection.GetPropertyValue(Run.FontStyleProperty) is FontStyle))
                {
                    ItalicSt.IsChecked = (((FontStyle)
                        richTextBox.Selection.GetPropertyValue(Run.FontStyleProperty)) 
                        == FontStyles.Italic);
                }
                if ((richTextBox.Selection.GetPropertyValue(Run.TextDecorationsProperty) is TextDecorationCollection))
                {
                    UnderlineSt.IsChecked = (((TextDecorationCollection)
                        richTextBox.Selection.GetPropertyValue(Run.TextDecorationsProperty)) 
                        == TextDecorations.Underline);
                }
                if ((richTextBox.Selection.GetPropertyValue(Run.FontFamilyProperty) is FontFamily))
                {
                    fontNameComBox.SelectedItem = (FontFamily)richTextBox.Selection.GetPropertyValue(Run.FontFamilyProperty);
                }
                if ((richTextBox.Selection.GetPropertyValue(Run.FontSizeProperty) is double))
                {
                    fontSizeComBox.SelectedItem = (double)richTextBox.Selection.GetPropertyValue(Run.FontSizeProperty);
                }
                if ((richTextBox.Selection.GetPropertyValue(Run.ForegroundProperty) is Brush))
                {
                    string color = ((Brush)richTextBox.Selection.GetPropertyValue(Run.ForegroundProperty)).ToString();
                    switch (color)
                    {
                        case "#FF000000": fontColorComBox.SelectedIndex = 0; break;
                        case "#FF008000": fontColorComBox.SelectedIndex = 1; break;
                        case "#FFFFFF00": fontColorComBox.SelectedIndex = 2; break;
                        case "#FFFF0000": fontColorComBox.SelectedIndex = 3; break;
                        case "#FF0000FF": fontColorComBox.SelectedIndex = 4; break;
                        case "#FFA52A2A": fontColorComBox.SelectedIndex = 5; break;
                    }
                }
            }
        }

        private void History_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fileStream = new FileStream(History.SelectedItem.ToString(), FileMode.Open);
            TextRange range = new TextRange(richTextBox.Document.ContentStart,
                richTextBox.Document.ContentEnd);
            range.Load(fileStream, DataFormats.Rtf);
            fileStream.Close();
        }

        private void Themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Resources = new ResourceDictionary()
            {
                Source = new Uri("pack://application:,,,/Theme" + Convert.ToString(Themes.SelectedIndex) + ".xaml")
            };
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            richTextBox.Undo();
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            richTextBox.Redo();
        }

        //private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        //{
        //    if (richTextBox) Paste.Visibility = Visibility.Visible;
        //    else Paste.Visibility = Visibility.Collapsed;
        //}
    }
}
