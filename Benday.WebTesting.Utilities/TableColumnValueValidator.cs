using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.WebTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benday.WebTesting.Utilities
{
    [DisplayName("Table Column Value")]
    [Description("Validates that a value exists in a column in an HTML table.")]
    public class TableColumnValueValidator : ValidationRule
    {
        [DisplayName("Table ID")]
        [Description("Id attribute value for the table.")]
        public string TableId { get; set; }

        [DisplayName("Column Name")]
        [Description("Name of the column that should have the value.")]
        public string ColumnName { get; set; }

        [DisplayName("Expected Value")]
        [Description("Expected value that should be in the table cell.")]
        public string ExpectedValue { get; set; }

        public override void Validate(object sender, ValidationEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(TableId) == true)
            {
                Fail(e, "Table Id does not have a valid value.");
            }
            else if (String.IsNullOrWhiteSpace(ColumnName) == true)
            {
                Fail(e, "Column name does not have a valid value.");
            }
            else if (ExpectedValue == null)
            {
                Fail(e, "Expected value cannot be null.");
            }
            else
            {
                ValidateTable(e);
            }
        }

        private void ValidateTable(ValidationEventArgs e)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(e.Response.BodyString);

            var table = doc.GetElementbyId(TableId);

            if (table == null)
            {
                Fail(e, 
                    String.Format(
                    "Could not locate a table tag named '{0}'.", TableId));
            }
            else if (table.Name != "table")
            {
                Fail(e, String.Format(
                    "Found a tag named '{0}' but it wasn't a table.", TableId));
            }
            else
            {
                ValidateTable(e, table);
            }
        }

        private void ValidateTable(ValidationEventArgs e, HtmlNode table)
        {
            var columns = table.SelectNodes("//th");

            if (columns == null || columns.Count == 0)
            {
                Fail(e, "Could not locate any columns in the table.");
            }
            else
            {
                int columnIndex = FindColumnIndexByName(columns, ColumnName);

                if (columnIndex == -1)
                {
                    Fail(e, String.Format(
                        "Could not find a column named '{0}'.", ColumnName));
                }
                else
                {
                    bool foundTheValue = 
                        DoesValueExistInColumn(table, columnIndex, ExpectedValue);

                    if (foundTheValue == true)
                    {
                        Pass(e, "Found the column value.");
                    }
                    else
                    {
                        Fail(e, 
                            String.Format(
                            "Could not find a cell with the value '{0}'.", 
                            ExpectedValue));
                    }
                }
            }
        }

        private bool DoesValueExistInColumn(
            HtmlNode table, int columnIndex, string expected)
        {
            bool returnValue = false;

            var rows = table.SelectNodes("//tr");

            HtmlNode cell;

            HtmlNodeCollection cells;

            foreach (var row in rows)
            {
                cells = row.SelectNodes("./td");

                if (cells == null || columnIndex >= cells.Count)
                {
                    continue;
                }
                else
                {
                    cell = cells[columnIndex];

                    if (cell == null)
                    {
                        continue;
                    }
                    else if (cell.InnerText == expected)
                    {
                        returnValue = true;
                        break;
                    }
                }
            }

            return returnValue;
        }

        private int FindColumnIndexByName(
            HtmlNodeCollection columns, string columnName)
        {
            bool foundIt = false;
            int columnIndex = 0;

            foreach (var item in columns)
            {
                if (String.Compare(item.InnerText, columnName, true) == 0)
                {
                    foundIt = true;
                    break;
                }

                columnIndex++;
            }

            if (foundIt == true)
            {
                return columnIndex;
            }
            else
            {
                return -1;
            }
        }


        private void Fail(ValidationEventArgs e, string message)
        {
            e.IsValid = false;
            e.Message = message;
        }

        private void Pass(ValidationEventArgs e, string message)
        {
            e.IsValid = true;
            e.Message = message;
        }
    }
}
