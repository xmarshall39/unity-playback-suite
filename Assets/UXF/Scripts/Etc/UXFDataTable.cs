using System;
using System.Linq;
using System.Collections.Generic;

namespace UXF
{
    /// <summary>
    /// Represents a table of data. That is, a series of named columns, each column representing a list of data. The lists of data are always the same length.
    /// </summary>
    public class UXFDataTable
    {
        public string[] Headers { get { return dict.Keys.ToArray(); } }
        private Dictionary<string, List<object>> dict;

        public UXFDataTable(params string[] columnNames)
        {
            dict = new Dictionary<string, List<object>>();
            foreach (string colName in columnNames)
            {
                dict.Add(colName, new List<object>());
            }
        }

        public void AddCompleteRow(UXFDataRow newRow)
        {
            bool sameKeys = (dict
                .Keys
                .All(
                    newRow
                    .Select(item => item.columnName)
                    .Contains
                    ))
                &&
                (newRow.Count == dict.Keys.Count);

            if (!sameKeys) throw new InvalidOperationException("The row does not contain values for the same columns as the columns in the table!");

            foreach (var item in newRow)
            {
                dict[item.columnName].Add(item.value);
            }
        }

        public int CountRows()
        {
            string[] keyArray = dict.Keys.ToArray();
            if (keyArray.Length == 0) return 0;

            return dict[keyArray[0]].Count();
        }

        public string[] GetCSVLines()
        {
            string[] headers = Headers;
            string[] lines = new string[CountRows() + 1];
            lines[0] = string.Join(",", headers);
            for (int i = 1; i < lines.Length; i++)
            {
                lines[i] = string.Join(",", 
                    headers
                    .Select(h => dict[h][i - 1])
                );
            }

            return lines;
        }
    }

    /// <summary>
    /// Represents a single row of data. That is, a series of named columns, each column representing a single value.
    /// The row hold a list of named Tuples (columnName and value). To add values, create a new UXFDataRow then add Tuples with the Add method.
    /// </summary>
    public class UXFDataRow : List<(string columnName, object value)> { }

}