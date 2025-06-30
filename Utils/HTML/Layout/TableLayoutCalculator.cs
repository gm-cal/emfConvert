using System.Collections.Generic;
using System.Drawing;
using Utils.HTML.IO;

namespace Utils.HTML.Layout{
    public interface ITableLayoutCalculator{
        SizeF ComputeCellSizes(List<List<Cell>> table, Font font, float padding);
    }

    public class TableLayoutCalculator : ITableLayoutCalculator{
        // テーブル全体のセルサイズを計算し、合計サイズを返す
        public SizeF ComputeCellSizes(List<List<Cell>> table, Font font, float padding){
            int columnCount = GetMaxColumnCount(table);
            float[] maxColumnWidths = CreateColumnWidthArray(columnCount);
            float[] rowHeights = CreateRowHeightArray(table.Count);

            using (Bitmap bitmap = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(bitmap)){
                CalculateCellSizes(table, font, padding, graphics, maxColumnWidths, rowHeights);
                SetMaxColumnWidthsToCells(table, maxColumnWidths);
            }

            float totalWidth = CalculateTotal(maxColumnWidths);
            float totalHeight = CalculateTotal(rowHeights);

            return new SizeF(totalWidth, totalHeight);
        }

        // テーブルの最大列数を取得する
        private static int GetMaxColumnCount(List<List<Cell>> table){
            int columnCount = 0;
            foreach (List<Cell> row in table){
                if (row.Count > columnCount){
                    columnCount = row.Count;
                }
            }
            return columnCount;
        }

        // 列幅配列を生成する
        private static float[] CreateColumnWidthArray(int columnCount){
            return new float[columnCount];
        }

        // 行高さ配列を生成する
        private static float[] CreateRowHeightArray(int rowCount){
            return new float[rowCount];
        }

        // 各セルのサイズを計算し、最大幅・高さを記録する
        private static void CalculateCellSizes(List<List<Cell>> table, Font font, float padding, Graphics graphics, float[] maxColumnWidths, float[] rowHeights){
            for (int rowIndex = 0; rowIndex < table.Count; rowIndex++){
                List<Cell> row = table[rowIndex];
                float maxHeight = 0.0f;
                for (int colIndex = 0; colIndex < row.Count; colIndex++){
                    Cell cell = row[colIndex];
                    if (cell.IsDrawn) continue;
                    SizeF size = graphics.MeasureString(cell.Text, font);
                    float width = size.Width + padding * 2;
                    float height = size.Height + padding;
                    if (width > maxColumnWidths[colIndex]){
                        maxColumnWidths[colIndex] = width;
                    }
                    if (height > maxHeight){
                        maxHeight = height;
                    }
                    cell.Width = width;
                    cell.Height = height;
                }
                rowHeights[rowIndex] = maxHeight;
            }
        }

        // 各セルに列の最大幅を反映する
        private static void SetMaxColumnWidthsToCells(List<List<Cell>> table, float[] maxColumnWidths){
            for (int rowIndex = 0; rowIndex < table.Count; rowIndex++){
                List<Cell> row = table[rowIndex];
                for (int colIndex = 0; colIndex < row.Count; colIndex++){
                    Cell cell = row[colIndex];
                    if (!cell.IsDrawn){
                        cell.Width = maxColumnWidths[colIndex];
                    }
                }
            }
        }

        // 配列の合計値を計算する
        private static float CalculateTotal(float[] values){
            float total = 0.0f;
            foreach (float v in values){
                total += v;
            }
            return total;
        }
    }
}
