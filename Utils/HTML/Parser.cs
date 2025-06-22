using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Utils.HTML.IO;

namespace Utils.HTML{
    public class Parser{
        public List<List<Cell>> ParseHtmlTable(string html){
            List<List<Cell>> rows = new List<List<Cell>>();

            WebBrowser browser = new WebBrowser{
                ScriptErrorsSuppressed = true,
                DocumentText = html
            };

            while (browser.ReadyState != WebBrowserReadyState.Complete){
                Application.DoEvents();
            }

            HtmlElementCollection tables = browser.Document.GetElementsByTagName("table");
            if (tables.Count == 0){
                return rows;
            }

            HtmlElement table = tables[0];
            HtmlElementCollection trElements = table.GetElementsByTagName("tr");

            int maxCols = 0;
            List<List<Cell>> grid = new List<List<Cell>>();

            foreach (HtmlElement tr in trElements){
                HtmlElementCollection tdElements = tr.GetElementsByTagName("td");
                List<Cell> currentRow = new List<Cell>();
                int colIndex = 0;

                if (grid.Count == 0) grid.Add(new List<Cell>());

                while (grid.Count < rows.Count + 1)
                    grid.Add(new List<Cell>());

                for (int i = 0; i < grid[rows.Count].Count; i++){
                    if (grid[rows.Count][i] != null) colIndex++;
                }

                foreach (HtmlElement td in tdElements){
                    int colspan = ParseSpan(td.GetAttribute("colspan"));
                    int rowspan = ParseSpan(td.GetAttribute("rowspan"));
                    string text = td.InnerText ?? string.Empty;

                    while (colIndex >= grid[rows.Count].Count)
                        grid[rows.Count].Add(null);

                    Cell cell = new Cell{
                        Text = text,
                        ColSpan = colspan,
                        RowSpan = rowspan
                    };

                    string bgcolor = td.GetAttribute("bgcolor");
                    string style = td.GetAttribute("style");

                    string cssColor = ExtractCssBackgroundColor(style);
                    string htmlColor = !string.IsNullOrEmpty(cssColor) ? cssColor : bgcolor;

                    if (!string.IsNullOrEmpty(htmlColor)){
                        try{
                            cell.BackgroundColor = ColorTranslator.FromHtml(htmlColor);
                        } catch {
                            cell.BackgroundColor = Color.White;
                        }
                    }

                    // 配置する範囲すべてにマークを入れる
                    for (int r = 0; r < rowspan; r++){
                        while (grid.Count <= rows.Count + r)
                            grid.Add(new List<Cell>());

                        for (int c = 0; c < colspan; c++){
                            while (grid[rows.Count + r].Count <= colIndex + c)
                                grid[rows.Count + r].Add(null);

                            grid[rows.Count + r][colIndex + c] = (r == 0 && c == 0) ? cell : new Cell { IsDrawn = true };
                        }
                    }

                    currentRow.Add(cell);
                    colIndex += colspan;
                }

                rows.Add(currentRow);
                if (colIndex > maxCols) maxCols = colIndex;
            }

            return grid.ConvertAll(row => row.FindAll(cell => cell != null)!);
        }

        private int ParseSpan(string value){
            return int.TryParse(value, out int span) ? Math.Max(1, span) : 1;
        }

        private string ExtractCssBackgroundColor(string style){
            if (string.IsNullOrEmpty(style)){
                return String.Empty;
            }

            string[] styles = style.Split(';');
            foreach (string item in styles){
                string[] parts = item.Split(':');
                if (parts.Length == 2){
                    string key = parts[0].Trim().ToLower();
                    string value = parts[1].Trim();
                    if (key == "background-color" || key == "background"){
                        // "background" may contain multiple values; first token is color
                        return value.Split(' ')[0];
                    }
                }
            }

            return String.Empty;
        }
    }
}
