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

        private void OnDebugClick(object sender, RoutedEventArgs e){
            if (Clipboard.ContainsText(TextDataFormat.Html)){
                string html = Clipboard.GetText(TextDataFormat.Html);
                // カスタムウィンドウでHTMLを表示（スクロール・サイズ指定）
                Window htmlWindow = new Window{
                    Title = "Clipboard HTML",
                    Width = 800,
                    Height = 600,
                    Content = new System.Windows.Controls.ScrollViewer{
                        Content = new System.Windows.Controls.TextBox{
                            Text = html,
                            IsReadOnly = true,
                            AcceptsReturn = true,
                            AcceptsTab = true,
                            VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
                            HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
                            TextWrapping = System.Windows.TextWrapping.NoWrap
                        }
                    }
                };
                htmlWindow.ShowDialog();
            } else {
                System.Windows.MessageBox.Show("クリップボードにHTMLデータがありません", "Clipboard HTML");
            }
        }
    }