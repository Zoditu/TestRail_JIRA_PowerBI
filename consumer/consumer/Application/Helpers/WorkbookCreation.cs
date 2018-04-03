using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace ConsumerTestRail.Application.Helpers
{
	public static class WorkbookCreation
	{
		private const String Worksheet = "TestNG_JIRA";
		
		public static Task<Boolean> WriteDataTableToExcel( System.Data.DataTable source, String filename )
        {
			return Task.Run<Boolean>
			(
				() =>
				{
					Microsoft.Office.Interop.Excel.Application excel;
		            Workbook excelworkBook;
		            Worksheet excelSheet;
		            Range excelCellrange;
		
		            try
		            {
		                // Start Excel and get Application object.
		                excel = new Microsoft.Office.Interop.Excel.Application();
		
		                // for making Excel visible
		                excel.Visible = false;
		                excel.DisplayAlerts = false;
		
		                // Creation a new Workbook
		                excelworkBook = excel.Workbooks.Add(Type.Missing);
		
		                // Workk sheet
		                excelSheet = (Worksheet)excelworkBook.ActiveSheet;
		                excelSheet.Name = Worksheet;
		                
		                int rowcount = 1;
		
		                foreach (DataRow datarow in source.Rows)
		                {
		                    rowcount += 1;
		                    for (int i = 1; i <= source.Columns.Count; i++)
		                    {
		                        if (rowcount == 2)
		                        {
		                            excelSheet.Cells[1, i] = source.Columns[i - 1].ColumnName;
		                            excelSheet.Cells.Font.Color = System.Drawing.Color.Black;
		                        }
		
		                        excelSheet.Cells[rowcount, i] = datarow[i - 1].ToString();
		                    }
		
		                }
		
		                // now we resize the columns
		                excelCellrange = (Range)excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[rowcount, source.Columns.Count]];
		                excelCellrange.EntireColumn.AutoFit();
		                
		                
		                var row = source.Rows.Count + 1;
		                var col = source.Columns.Count;
		                
		                Range SourceRange = (Range)excelSheet.Range["A1","AA" + row]; // or whatever range you want here
						FormatAsTable(SourceRange, "Table1", "TableStyleMedium20");
		
		
		                excelworkBook.SaveAs( filename );
		                excelworkBook.Close();
		                excel.Quit();
		                return true;
		            }
		            catch (Exception ex)
		            {
		            	Logger.Log += ex.Message + Environment.NewLine + ex.StackTrace;
		                return false;
		            }
		            finally
		            {
		                excelSheet = null;
		                excelCellrange = null;
		                excelworkBook = null;
		            }
				}
			);
        }

        /// <summary>
        /// FUNCTION FOR FORMATTING EXCEL CELLS
        /// </summary>
        /// <param name="range"></param>
        /// <param name="HTMLcolorCode"></param>
        /// <param name="fontColor"></param>
        /// <param name="IsFontbool"></param>
        public static void FormattingExcelCells(Microsoft.Office.Interop.Excel.Range range, string HTMLcolorCode, System.Drawing.Color fontColor, bool IsFontbool)
        {
            range.Interior.Color = System.Drawing.ColorTranslator.FromHtml(HTMLcolorCode);
            range.Font.Color = System.Drawing.ColorTranslator.ToOle(fontColor);
            if (IsFontbool == true)
            {
                range.Font.Bold = IsFontbool;
            }
        }
        
        public static void FormatAsTable(Range SourceRange, string TableName, string TableStyleName)
		{
		    SourceRange.Worksheet.ListObjects.Add(XlListObjectSourceType.xlSrcRange,
		    SourceRange, System.Type.Missing, XlYesNoGuess.xlYes, System.Type.Missing).Name = TableName;
		    SourceRange.Select();
		    SourceRange.Worksheet.ListObjects[TableName].TableStyle = TableStyleName;
		}
	}
}
