using System.Drawing;

namespace Utils.HTML.IO{
    public class Cell{
        public string Text { get; set; } = string.Empty;
        public int ColSpan { get; set; } = 1;
        public int RowSpan { get; set; } = 1;
        public bool IsDrawn { get; set; } = false; // rowspan/colspanによる重複描画防止用

        public float Height { get; set; } = 0; // セルの高さ（ピクセル単位）
        public float Width { get; set; } = 0; // セルの幅（ピクセル単位）

        public Color BackgroundColor { get; set; } = Color.White;
    }
}
