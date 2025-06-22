using System.Windows;
using System;
using Services;

public partial class MainWindow : Window{
        public MainWindow(){
            InitializeComponent();
        }

        private void OnConvertClick(object sender, RoutedEventArgs e){
            EmfGenerator generator = new EmfGenerator();
            generator.ConvertClipboardHtmlTableToEmf();
        }
    }