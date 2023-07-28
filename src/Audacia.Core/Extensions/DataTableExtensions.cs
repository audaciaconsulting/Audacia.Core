using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Audacia.Core.Extensions;

/// <summary>
/// Extension methods for the type <see cref="DataTable"/>.
/// </summary>
public static class DataTableExtensions
{
    /// <summary>
    /// Converts a DataTable to a CSV string.
    /// </summary>
    /// <param name="dataTable">The DataTable to convert.</param>
    /// <param name="delimiter">The delimiter to use in the converted string (defaults to ',').</param>
    /// <returns>The converted string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dataTable"/> is null.</exception>
    /// <exception cref="NotSupportedException">Delimiter is invalid.</exception>
    public static string ToCsv(this DataTable dataTable, string delimiter = ",")
    {
        ValidateArguments(dataTable, delimiter);

        var outputBuilder = new StringBuilder();

        var headers = GetHeaders(dataTable);

        outputBuilder.AppendJoin(delimiter, headers).AppendLine();

        return GetRows(dataTable, delimiter, outputBuilder);
    }

    private static void ValidateArguments(DataTable dataTable, string delimiter)
    {
        ArgumentNullException.ThrowIfNull(dataTable);

        if (delimiter == "\"")
        {
            throw new NotSupportedException("Cannot use \" as a delimiter as it is used in CSV qualification");
        }
    }

    private static HashSet<string> GetHeaders(DataTable dataTable)
    {
        var headers = new HashSet<string>();
        foreach (DataColumn column in dataTable.Columns)
        {
            var header = column.Caption ?? column.ColumnName;

            header = header.Replace("\"", "\"\"", StringComparison.InvariantCulture);

            headers.Add($"\"{header}\"");
        }

        return headers;
    }

    private static string GetRows(DataTable dataTable, string delimiter, StringBuilder outputBuilder)
    {
        foreach (DataRow row in dataTable.Rows)
        {
            var cells = new string[row.ItemArray.Length];

            for (var i = 0; i < row.ItemArray.Length; i++)
            {
                var cellValue = row.ItemArray.ElementAt(i);

                // If we have a format string, we can apply it here using string.Format();
                var cellValueString = cellValue?.ToString();
                cellValueString = cellValueString?.Replace("\"", "\"\"", StringComparison.InvariantCulture);

                cells[i] = $"\"{cellValueString}\"";
            }

            outputBuilder.AppendJoin(delimiter, cells).AppendLine();
        }

        return outputBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
    }
}