using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Utils;
using Utils.HTML;
using Utils.HTML.Interfaces;
using Utils.HTML.IO;
using Utils.HTML.Layout;

namespace Services{
    public class EmfGenerator{
        private readonly IParser _parser;
        private readonly ITableLayoutCalculator _tableLayoutCalculator;

        public EmfGenerator(IParser parser, ITableLayoutCalculator tableLayoutCalculator){
            _parser = parser;
            _tableLayoutCalculator = tableLayoutCalculator;
        }

        // クリップボードのHTMLテーブルをEMF画像に変換してクリップボードへ格納する
        public void ConvertClipboardHtmlTableToEmf(){
            string html = GetClipboardHtml();
            if (html == null) return;
            List<List<Cell>> table = ParseTable(html);
            if (table.Count == 0){
                MessageBox.Show("テーブルが見つかりません。", "エラー");
                return;
            }
            Font font = GetDefaultFont();
            float padding = GetDefaultPadding();
            SizeF tableSize = _tableLayoutCalculator.ComputeCellSizes(table, font, padding);
            List<float> rowMaxHeights = CalculateRowMaxHeights(table);
            Rectangle frame = new Rectangle(0, 0, (int)Math.Ceiling(tableSize.Width), (int)Math.Ceiling(tableSize.Height));
            using (MemoryStream stream = new MemoryStream())
            using (Graphics referenceGraphics = Graphics.FromHwnd(IntPtr.Zero)){
                Metafile metafile = CreateMetafile(stream, referenceGraphics, frame);
                using (Graphics graphics = Graphics.FromImage(metafile)){
                    DrawTable(graphics, table, font, padding, tableSize, rowMaxHeights);
                }
                ClipboardMetafile.PutEnhMetafileOnClipboard(metafile);
            }
        }

        // クリップボードからHTMLテーブル文字列を取得する
        private string GetClipboardHtml(){
            if (!Clipboard.ContainsText(TextDataFormat.Html)){
                MessageBox.Show("HTML形式のクリップボードデータが見つかりません。", "エラー");
                return null;
            }
            return Clipboard.GetText(TextDataFormat.Html);
        }

        // HTML文字列をパースしてCellの2次元リストに変換する
        private List<List<Cell>> ParseTable(string html){
            return _parser.ParseHtmlTable(html);
        }

        // デフォルトのフォントを取得する
        private Font GetDefaultFont(){
            return new Font("MS Gothic", 9);
        }

        // デフォルトのパディング値を取得する
        private float GetDefaultPadding(){
            return 8.0f;
        }

        // 各行の最大高さを計算する
        private List<float> CalculateRowMaxHeights(List<List<Cell>> table){
            List<float> rowMaxHeights = new List<float>();
            for (int rowIndex = 0; rowIndex < table.Count; rowIndex++){
                float maxHeight = 0.0f;
                for (int cellIndex = 0; cellIndex < table[rowIndex].Count; cellIndex++){
                    Cell cell = table[rowIndex][cellIndex];
                    if (cell.Height > maxHeight) maxHeight = cell.Height;
                }
                rowMaxHeights.Add(maxHeight);
            }
            return rowMaxHeights;
        }

        // EMF用のMetafileオブジェクトを生成する
        private Metafile CreateMetafile(MemoryStream stream, Graphics referenceGraphics, Rectangle frame){
            IntPtr hdc = referenceGraphics.GetHdc();
            Metafile metafile = new Metafile(stream, hdc, frame, MetafileFrameUnit.Pixel);
            referenceGraphics.ReleaseHdc(hdc);
            return metafile;
        }

        // テーブル全体と各セルを描画する
        private void DrawTable(Graphics graphics, List<List<Cell>> table, Font font, float padding, SizeF tableSize, List<float> rowMaxHeights){
            // テーブル全体の背景と外枠を描画
            RectangleF tableRect = new RectangleF(0, 0, tableSize.Width, tableSize.Height);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            graphics.FillRectangle(Brushes.White, tableRect);
            Pen outerPen = new Pen(Color.Gray, 1.5f) { DashStyle = DashStyle.Solid };
            graphics.DrawRectangle(outerPen, tableRect.X, tableRect.Y, tableRect.Width, tableRect.Height);
            // 行ごとに処理を分離
            float y = 0.0f;
            for (int rowIndex = 0; rowIndex < table.Count; rowIndex++){
                List<Cell> row = table[rowIndex];
                float rowHeight = rowMaxHeights[rowIndex];
                DrawRow(graphics, row, font, padding, y, rowHeight);
                y += rowHeight;
            }
        }

        // 1行分のセルをすべて描画する
        private void DrawRow(Graphics graphics, List<Cell> row, Font font, float padding, float y, float rowHeight){
            float x = 0.0f;
            for (int colIndex = 0; colIndex < row.Count; colIndex++){
                Cell cell = row[colIndex];
                if (cell.IsDrawn) continue;
                float width = CalculateCellWidth(row, colIndex, cell.ColSpan);
                float height = rowHeight;
                RectangleF rect = new RectangleF(x, y, width, height);
                DrawCell(graphics, cell, font, padding, rect);
                x += width;
            }
        }

        // 指定セルの幅（colspan対応）を計算する
        private float CalculateCellWidth(List<Cell> row, int colIndex, int colSpan){
            float width = 0.0f;
            for (int i = 0; i < colSpan && colIndex + i < row.Count; i++){
                width += row[colIndex + i].Width;
            }
            return width;
        }

        // 1セル分の背景・枠線・テキストを描画する
        private void DrawCell(Graphics graphics, Cell cell, Font font, float padding, RectangleF rect){
            Pen pen = new Pen(Color.Black, 1.0f);
            Brush backgroundBrush = new SolidBrush(cell.BackgroundColor);
            graphics.FillRectangle(backgroundBrush, rect);
            graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
            graphics.DrawString(cell.Text, font, Brushes.Black, rect.X + padding, rect.Y + 4);
            backgroundBrush.Dispose();
            pen.Dispose();
        }
    }
}
