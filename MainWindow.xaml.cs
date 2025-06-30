using System.Windows;
using System;
using Services;
using System.Runtime.InteropServices; // 追加
using System.Windows.Interop; // 追加

public partial class MainWindow : Window{
    // Win32 API 定義
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private const int HOTKEY_ID = 9000;
    private const uint MOD_CONTROL = 0x2;
    private const uint MOD_SHIFT = 0x4;
    private const uint VK_E = 0x45;

    public MainWindow(){
        InitializeComponent();
        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e){
        var helper = new WindowInteropHelper(this);
        RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, VK_E); // Ctrl+Shift+E
        HwndSource source = HwndSource.FromHwnd(helper.Handle);
        source.AddHook(HwndHook);
    }

    private void MainWindow_Closed(object sender, EventArgs e){
        var helper = new WindowInteropHelper(this);
        UnregisterHotKey(helper.Handle, HOTKEY_ID);
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled){
        const int WM_HOTKEY = 0x0312;
        if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID){
            // グローバルホットキー押下時の処理
            Dispatcher.Invoke(() => OnConvertClick(this, null));
            handled = true;
        }
        return IntPtr.Zero;
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