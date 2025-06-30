using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Utils.HTML.Interfaces;
using Utils.HTML.IO;

namespace Utils.HTML{
    public class Parser : IParser{
        // HTMLテーブル文字列を2次元Cellリストに変換する
        public List<List<Cell>> ParseHtmlTable(string html){
            List<List<Cell>> rows = new List<List<Cell>>();
            WebBrowser browser = CreateWebBrowserWithHtml(html);
            WaitForBrowserLoad(browser);
            Dictionary<string, Color> styleMap = ParseStyleDictionary(html);
            HtmlElement table = GetFirstTableElement(browser);
            if (table == null) return rows;
            HtmlElementCollection trElements = table.GetElementsByTagName("tr");
            List<List<Cell>> grid = new List<List<Cell>>();
            int maxCols = 0;
            foreach (HtmlElement tr in trElements){
                List<Cell> currentRow = new List<Cell>();
                int colIndex = 0;
                EnsureGridRow(grid, rows.Count);
                colIndex = GetNextAvailableColIndex(grid, rows.Count);
                HtmlElementCollection tdElements = tr.GetElementsByTagName("td");
                foreach (HtmlElement td in tdElements){
                    Cell cell = CreateCellFromTd(td, styleMap);
                    int colspan = cell.ColSpan;
                    int rowspan = cell.RowSpan;
                    PlaceCellInGrid(grid, cell, rows.Count, colIndex, rowspan, colspan);
                    currentRow.Add(cell);
                    colIndex += colspan;
                }
                rows.Add(currentRow);
                if (colIndex > maxCols) maxCols = colIndex;
            }
            return ConvertGridToRows(grid);
        }

        // WebBrowserコントロールを生成しHTMLをセットする
        private WebBrowser CreateWebBrowserWithHtml(string html){
            return new WebBrowser{
                ScriptErrorsSuppressed = true,
                DocumentText = html
            };
        }

        // WebBrowserの読み込み完了まで待機する
        private void WaitForBrowserLoad(WebBrowser browser){
            while (browser.ReadyState != WebBrowserReadyState.Complete){
                Application.DoEvents();
            }
        }

        // 最初のtable要素を取得する
        private HtmlElement GetFirstTableElement(WebBrowser browser){
            HtmlElementCollection tables = browser.Document.GetElementsByTagName("table");
            if (tables.Count == 0) return null;
            return tables[0];
        }

        // gridに指定行がなければ追加する
        private void EnsureGridRow(List<List<Cell>> grid, int rowIndex){
            while (grid.Count < rowIndex + 1)
                grid.Add(new List<Cell>());
        }

        // 次にセルを配置できる列インデックスを取得する
        private int GetNextAvailableColIndex(List<List<Cell>> grid, int rowIndex){
            int colIndex = 0;
            for (int i = 0; i < grid[rowIndex].Count; i++){
                if (grid[rowIndex][i] != null) colIndex++;
            }
            return colIndex;
        }

        // td要素からCellを生成する
        private Cell CreateCellFromTd(HtmlElement td, Dictionary<string, Color> styleMap){
            string className = td.GetAttribute("className") ?? td.GetAttribute("class");
            int colspan = ParseSpan(td.GetAttribute("colspan"));
            int rowspan = ParseSpan(td.GetAttribute("rowspan"));
            string text = td.InnerText ?? string.Empty;
            Cell cell = new Cell{
                Text = text,
                ColSpan = colspan,
                RowSpan = rowspan
            };
            // クラス名から背景色を決定
            if (!string.IsNullOrEmpty(className) && styleMap.TryGetValue(className, out Color bgColor)){
                cell.BackgroundColor = bgColor;
            } else {
                cell.BackgroundColor = Color.White;
            }
            // td属性やstyleから背景色を上書き
            string bgcolor = td.GetAttribute("bgcolor");
            string style = td.GetAttribute("style");
            string cssColor = ExtractCssBackgroundColor(style);
            string htmlColor = !string.IsNullOrEmpty(cssColor) ? cssColor : bgcolor;
            if (!string.IsNullOrEmpty(htmlColor)){
                try {
                    cell.BackgroundColor = ColorTranslator.FromHtml(htmlColor);
                } catch {
                    cell.BackgroundColor = Color.White;
                }
            }
            return cell;
        }

        // セルをgridに配置し、結合セルにはIsDrawn=trueのダミーCellを配置
        private void PlaceCellInGrid(List<List<Cell>> grid, Cell cell, int baseRow, int baseCol, int rowspan, int colspan){
            for (int r = 0; r < rowspan; r++){
                EnsureGridRow(grid, baseRow + r);
                for (int c = 0; c < colspan; c++){
                    while (grid[baseRow + r].Count <= baseCol + c)
                        grid[baseRow + r].Add(null);
                    grid[baseRow + r][baseCol + c] = (r == 0 && c == 0) ? cell : new Cell { IsDrawn = true };
                }
            }
        }

        // gridからnullでないCellのみ抽出して2次元リスト化
        private List<List<Cell>> ConvertGridToRows(List<List<Cell>> grid){
            return grid.ConvertAll(row => row.FindAll(cell => cell != null)!);
        }

        // colspan/rowspan属性値をintに変換（1未満や空は1）
        private int ParseSpan(string value){
            return int.TryParse(value, out int span) ? Math.Max(1, span) : 1;
        }

        // style属性からbackground-color値を抽出
        private string ExtractCssBackgroundColor(string style){
            if (string.IsNullOrEmpty(style)){
                return string.Empty;
            }
            string[] styles = style.Split(';');
            foreach (string item in styles){
                int colonIndex = item.IndexOf(':');
                if (colonIndex < 0) continue;
                string key = item.Substring(0, colonIndex).Trim().ToLowerInvariant();
                string value = item.Substring(colonIndex + 1).Trim();
                if (key == "background-color" || key == "background"){
                    int spaceIndex = value.IndexOf(' ');
                    if (spaceIndex >= 0){
                        value = value.Substring(0, spaceIndex);
                    }
                    int importantIndex = value.IndexOf('!');
                    if (importantIndex >= 0){
                        value = value.Substring(0, importantIndex).Trim();
                    }
                    return value;
                }
            }
            return string.Empty;
        }

        // <style>内のクラス名と背景色の辞書を作成
        private Dictionary<string, Color> ParseStyleDictionary(string html){
            Dictionary<string, Color> styleMap = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase);
            int styleStart = html.IndexOf("<style", StringComparison.OrdinalIgnoreCase);
            int styleEnd = html.IndexOf("</style>", StringComparison.OrdinalIgnoreCase);
            if (styleStart == -1 || styleEnd == -1) return styleMap;
            string styleContent = html.Substring(styleStart, styleEnd - styleStart);
            Regex regex = new Regex(@"(\.[a-zA-Z0-9_-]+)\s*\{([^}]+)\}", RegexOptions.IgnoreCase);
            foreach (Match match in regex.Matches(styleContent)){
                string className = match.Groups[1].Value.Trim('.');
                string styleBody = match.Groups[2].Value;
                string color = ExtractCssBackgroundColor(styleBody);
                if (!string.IsNullOrEmpty(color)){
                    try{
                        styleMap[className] = ColorTranslator.FromHtml(color);
                    } catch {
                        styleMap[className] = Color.White;
                    }
                }
            }
            return styleMap;
        }
    }
}
