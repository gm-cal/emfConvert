using System.Windows;
using System;
using Services;
using System.Windows.Forms;

public partial class MainWindow : Window{
        public MainWindow(){
            InitializeComponent();
        }

        private void OnConvertClick(object sender, RoutedEventArgs e){
            EmfGenerator generator = new EmfGenerator();
            generator.ConvertClipboardHtmlTableToEmf();
        }

        private void OnDebugClick(object sender, RoutedEventArgs e){
            if (Clipboard.ContainsText(TextDataFormat.Html)){
                string html = Clipboard.GetText(TextDataFormat.Html);
                System.Windows.MessageBox.Show(html, "Clipboard HTML");
            } else {
                System.Windows.MessageBox.Show("クリップボードにHTMLデータがありません", "Clipboard HTML");
            }
        }
    }