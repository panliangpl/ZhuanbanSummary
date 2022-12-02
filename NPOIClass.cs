using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ALLCheck
{
	// Token: 0x02000006 RID: 6
	public class NPOIClass
	{
		// Token: 0x06000012 RID: 18 RVA: 0x00003680 File Offset: 0x00001880
		public static DataTable ExcelToDataTable(string filePath)
		{
			DataTable dataTable = new DataTable();
			using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				XSSFWorkbook xssfworkbook = new XSSFWorkbook(fileStream);
				string str = "";
				ISheet sheet = xssfworkbook.GetSheet("合格证");
				bool flag = sheet != null;
				if (flag)
				{
					dataTable = NPOIClass.GetSheetDataTable(sheet, out str);
					bool flag2 = dataTable != null;
					if (flag2)
					{
						dataTable.TableName = "合格证";
					}
					else
					{
						MessageBox.Show("Sheet数据获取失败，原因：" + str);
					}
				}
			}
			return dataTable;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00003724 File Offset: 0x00001924
		public static DataSet ExcelToDataSet(string filePath, int SheetIndex, out string strMsg)
		{
			strMsg = "";
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			string a = Path.GetExtension(filePath).ToLower();
			string text = Path.GetFileName(filePath).ToLower();
			DataSet result;
			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				bool flag = a == ".xlsx";
				if (flag)
				{
					XSSFWorkbook xssfworkbook = new XSSFWorkbook(fileStream);
					int numberOfSheets = xssfworkbook.NumberOfSheets;
					string sheetName = xssfworkbook.GetSheetName(SheetIndex);
					ISheet sheet = xssfworkbook.GetSheet(sheetName);
					bool flag2 = sheet != null;
					if (flag2)
					{
						dataTable = NPOIClass.GetSheetDataTable(sheet, out strMsg);
						bool flag3 = dataTable != null;
						if (flag3)
						{
							dataTable.TableName = sheetName.Trim();
							dataSet.Tables.Add(dataTable);
						}
						else
						{
							MessageBox.Show("Sheet数据获取失败，原因：" + strMsg);
						}
					}
				}
				else
				{
					bool flag4 = a == ".xls";
					if (flag4)
					{
						HSSFWorkbook hssfworkbook = new HSSFWorkbook(fileStream);
						int numberOfSheets = hssfworkbook.NumberOfSheets;
						string sheetName2 = hssfworkbook.GetSheetName(SheetIndex);
						ISheet sheet = hssfworkbook.GetSheet(sheetName2);
						bool flag5 = sheet != null;
						if (flag5)
						{
							dataTable = NPOIClass.GetSheetDataTable(sheet, out strMsg);
							bool flag6 = dataTable != null;
							if (flag6)
							{
								dataTable.TableName = sheetName2.Trim();
								dataSet.Tables.Add(dataTable);
							}
							else
							{
								MessageBox.Show("Sheet数据获取失败，原因：" + strMsg);
							}
						}
					}
				}
				result = dataSet;
			}
			catch (Exception ex)
			{
				strMsg = ex.Message;
				result = null;
			}
			finally
            {
				if (fileStream != null)
                {
					fileStream.Dispose();
					fileStream.Close();
					fileStream = null;
				}

			}
			return result;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000038C4 File Offset: 0x00001AC4
		private static DataTable GetSheetDataTable(ISheet sheet, out string strMsg)
		{
			strMsg = "";
			DataTable dataTable = new DataTable();
			string sheetName = sheet.SheetName;
			int num = 0;
			int lastRowNum = sheet.LastRowNum;
			int num2 = 0;
			IRow row = sheet.GetRow(0);
			for (int i = num; i <= lastRowNum; i++)
			{
				IRow row2 = sheet.GetRow(i);
				bool flag = row2 != null && num2 < (int)row2.LastCellNum;
				if (flag)
				{
					num2 = (int)row2.LastCellNum;
					row = row2;
				}
			}
			try
			{
				for (int j = 0; j < (int)row.LastCellNum; j++)
				{
					dataTable.Columns.Add(Convert.ToChar(65 + j).ToString());
				}
			}
			catch
			{
				strMsg = "工作表" + sheetName + "中无数据";
				return null;
			}
			DataRow dataRow = null;
			for (int k = num; k <= lastRowNum; k++)
			{
				IRow row3 = sheet.GetRow(k);
				dataRow = dataTable.NewRow();
				bool flag2 = row3 != null;
				if (flag2)
				{
					for (int l = (int)row3.FirstCellNum; l < (int)row3.LastCellNum; l++)
					{
						bool flag3 = row3.GetCell(l) != null;
						if (flag3)
						{
							ICell cell = row3.GetCell(l);
							switch (cell.CellType)
							{
							case CellType.Numeric:
							{
								short dataFormat = cell.CellStyle.DataFormat;
								bool flag4 = dataFormat == 14 || dataFormat == 31 || dataFormat == 57 || dataFormat == 58;
								if (flag4)
								{
									dataRow[l] = cell.DateCellValue;
								}
								else
								{
									dataRow[l] = cell.NumericCellValue;
								}
								bool flag5 = cell.CellStyle.DataFormat == 177 || cell.CellStyle.DataFormat == 178 || cell.CellStyle.DataFormat == 188;
								if (flag5)
								{
									dataRow[l] = cell.NumericCellValue.ToString("#0");
								}
								break;
							}
							case CellType.String:
								dataRow[l] = cell.StringCellValue;
								break;
							case CellType.Formula:
								try
								{
									dataRow[l] = cell.NumericCellValue;
									bool flag6 = cell.CellStyle.DataFormat == 177 || cell.CellStyle.DataFormat == 178 || cell.CellStyle.DataFormat == 188;
									if (flag6)
									{
										dataRow[l] = cell.NumericCellValue.ToString("#0");
									}
								}
								catch
								{
									try
									{
										dataRow[l] = cell.StringCellValue;
									}
									catch
									{
									}
								}
								break;
							case CellType.Blank:
								dataRow[l] = "";
								break;
							default:
								dataRow[l] = cell.StringCellValue;
								break;
							}
						}
					}
				}
				dataTable.Rows.Add(dataRow);
			}
			return dataTable;
		}
	}
}
