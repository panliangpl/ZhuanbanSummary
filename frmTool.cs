using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using NPOI.XSSF.UserModel;
using System.Threading.Tasks;
using System.Threading;

namespace ALLCheck
{
	public partial class frmTool : Form
	{
		public frmTool()
		{
			InitializeComponent();
		}

		public const int age_index = 3;
		public const int community_index = 8;


		private void ClearMomery()
        {
			GC.Collect();
			GC.WaitForFullGCComplete();
        }

		private ICellStyle cellStyle = null;
		private IFont font = null;

		private void CreateCell(XSSFWorkbook wkWrite, ISheet stWrite, int iRow, int iColumn, string iValue)
		{
			ICell cell = stWrite.GetRow(iRow).CreateCell(iColumn, CellType.String);
			cell.SetCellValue(iValue);

			cellStyle.SetFont(font);
			cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
			cellStyle.VerticalAlignment = VerticalAlignment.Center;
			cell.CellStyle = cellStyle;
		}

		public const string MessageFormat = @"“日清日结”系统XX月XX日XX:00-YY月YY日YY:00下发的数据共15人（参密5人，次密10人）闭环情况进行核查，各街道管控情况如下：
江溪街道：共5人，
        参密2人，均已闭环;
        次密3人，均已闭环;
新安街道：共8人，
        参密2人，均已闭环;
        次密6人，均已闭环;
梅村街道：共2人，
        参密1人，已闭环;
        次密1人，已闭环;
以上人员管控情况均已电话核实。";

		private void hebing_btn_Click(object sender, EventArgs e)
		{
			//string retMsg = "“日清日结”系统XX月XX日XX:00-YY月YY日YY:00下发的数据共{0}人（参密{1}人，次密{2}人,密接{3}）闭环情况进行核查，各街道管控情况如下：\r\n";
			string retMsg = "“日清日结”系统XX月XX日XX:00-YY月YY日YY:00下发的数据为密接{0}人，其中xx人外省，yy人下发到街道，各街道管控情况如下：\r\n";

			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Multiselect = true;//该值确定是否可以选择多个文件
			dialog.Title = "请选择文件夹";
			dialog.Filter = "所有文件(*.*)|*.*";
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				string strMsg = "";
				try
				{
					var dataRowList = ExcelToDataSet(dialog.FileName, 0, out strMsg);
					int count = dataRowList.Count;
					//int canmiCount = dataRowList.Where(x => x.Level == "参密").Count();
					//int cimiCount = dataRowList.Where(x => x.Level == "次密").Count();
					int mijieCount = dataRowList.Where(x => x.Level == "密接").Count();

					//retMsg = string.Format(retMsg, count, canmiCount, cimiCount, mijieCount);
					retMsg = string.Format(retMsg, count, mijieCount);
					List<string> streetStrList = dataRowList.Select(x=>x.Street).Distinct().ToList();

					foreach (string street in streetStrList)
                    {
						var streetList = dataRowList.Where(x => x.Street == street).ToList();
						retMsg = retMsg + street + "：共" + streetList.Count() + "人,";

						List<string> levelStrList = streetList.Select(x => x.Level).Distinct().ToList();
						foreach (string level in levelStrList)
                        {
							var levelList = streetList.Where(x => x.Level == level);
							List<string> statusStrList = levelList.Select(x => x.Status).Distinct().ToList();
							//就只有密接了，所以这一层不显示
							//retMsg = retMsg + "" + level + levelList.Count() + "人：";

							string statusStr = "";
							foreach (string status in statusStrList)
                            {
								//var statusList = levelList.Where(x => x.Status == status).ToList();
								statusStr = statusStr + levelList.Where(x=>x.Status == status).Count() + "人" + status + ",";

							}
							retMsg = retMsg + statusStr + "\r\n";
						}
					}

					retMsg = retMsg + "\r\n以上人员管控情况均已电话核实。";

					this.richTextBox1.Text = retMsg;
				}
				catch (Exception ex)
				{
					return;
				}
			}




		}

		private void ChaChong()
        {
			this.hebing_btn.Invoke(new Action(() => hebing_btn.Enabled = false));

			//frmGetData frmGetData = new frmGetData();
			//frmGetData.StartPosition = FormStartPosition.CenterScreen;
			//frmGetData.ShowDialog();
			////frmGetData.lblTotal.Text = files.Length.ToString();
			//frmGetData.progressBar1.Maximum = 100;

			DirectoryInfo directoryInfo = new DirectoryInfo(Application.StartupPath + "\\要查重的文件");
			var dirArr = directoryInfo.GetDirectories();

			string strMsg = "";
			var fileDic = new Dictionary<string, List<RowInfo>>();
			string maxDate = "";
			foreach (var dir in dirArr)
			{
				FileInfo[] files = dir.GetFiles("*.xlsx", SearchOption.AllDirectories);
				List<RowInfo> dataRowList = new List<RowInfo>();
				foreach (FileInfo fileInfo in files)
				{
					try
					{
						var dt = ExcelToDataSet(fileInfo.FullName, 0, out strMsg);
						string[] types = fileInfo.Name.Replace(".xlsx", "").Split('_');

						foreach (var dr in dt)
						{
							////跳过第一行
							//if (dr.No == "序号" || string.IsNullOrEmpty(dr.No))
							//	continue;

							dataRowList.Add(dr);
						}

					}
					catch (Exception ex)
					{
						MessageBox.Show(fileInfo.FullName + " 没有关闭，请先关闭后再合并");
						return;
					}
				}
				fileDic.Add(dir.Name, dataRowList);
				ClearMomery();
				//step
				//frmGetData.progressBar1.Value = frmGetData.progressBar1.Value + 10;
				//frmGetData.progressBar1.Refresh();

				//顺便得到条数最多的一天
				if (maxDate == "" || fileDic[maxDate].Count < dataRowList.Count)
					maxDate = dir.Name;

			}

			////step
			//frmGetData.progressBar1.Value = 30;
			//frmGetData.progressBar1.Refresh();

			var maxDateData = fileDic[maxDate];
			var otherDataList = new List<RowInfo>();
			foreach (var dt in fileDic)
			{
				if (dt.Key != maxDate)
				{
					otherDataList.AddRange(dt.Value);
				}
			}
			//释放内存
			fileDic = null;

			var outputDataRowList = new List<DataRow>();
			ParallelOptions op = new ParallelOptions();
			op.MaxDegreeOfParallelism = 10;

		}

		public List<RowInfo> ExcelToDataSet(string filePath, int SheetIndex, out string strMsg)
		{
			strMsg = "";
			var dataTable = new List<RowInfo>();
			string a = Path.GetExtension(filePath).ToLower();
			string text = Path.GetFileName(filePath).ToLower();
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
						dataTable = GetSheetData(sheet, out strMsg);
					}
				}
				else
                {
					MessageBox.Show("文件格式不对:" + text);
					return null;
                }
			}
			catch (Exception ex)
			{
				strMsg = ex.Message;
				MessageBox.Show("excel加载错误:" + strMsg);
				dataTable = null;
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
			return dataTable;
		}

		private List<RowInfo> GetSheetData(ISheet sheet, out string strMsg)
		{
			strMsg = "";
			var rowInfoLsit = new List<RowInfo>();
			try
            {
				if (sheet != null)
				{
					//遍历行
					for (int j = sheet.FirstRowNum + 1; j <= sheet.LastRowNum; j++)
					{
						int index = 0;
						IRow row = sheet.GetRow(j);

						if (row == null)
							break;

						RowInfo outsouring = new RowInfo();

						////源文件去掉了序号列
						//if (!isSrc)
						//                  {
						//	outsouring.No = GetRowValue(row, index); index++;
						//	//跳过第一行
						//	if (string.IsNullOrEmpty(outsouring.No) || outsouring.No == "序号")
						//		continue;
						//}
						//新模板多一列
						index++;
						outsouring.Event = GetRowValue(row, index); index++;
						outsouring.Name = GetRowValue(row, index); index++;
						outsouring.Level = GetRowValue(row, index); index++;
						outsouring.Phone = GetRowValue(row, index); index++;
						outsouring.IdCard = GetRowValue(row, index); index++;
						outsouring.Address = GetRowValue(row, index); index++;
						outsouring.LoginTime = GetRowValue(row, index); index++;
						outsouring.ReceiveTime = GetRowValue(row, index); index++;
						outsouring.Street = GetRowValue(row, index); index++;
						outsouring.KeepAddress = GetRowValue(row, index); index++;
						outsouring.Area = GetRowValue(row, index); index++;
						outsouring.KeepShareTime = GetRowValue(row, index); index++;
						outsouring.KeepArriveTime = GetRowValue(row, index); index++;
						outsouring.Status = GetRowValue(row, index); index++;


						rowInfoLsit.Add(outsouring);
					}
				}
			}
			catch (Exception ex)
            {
				MessageBox.Show("Sheet数据获取失败，原因：" + ex.Message);
				strMsg = ex.Message;
				return null;
			}
			return rowInfoLsit;
		}

		private string GetRowValue(IRow row, int index)
		{
			return row.GetCell(index)?.ToString().Trim();
		}

	}
	public class ExcelInfo
    {
		public string CommunityName { get; set; }
		public AgeInfo OneAgeInfoList { get; set; }
		public AgeInfo TwoAgeInfoList { get; set; }
		public AgeInfo ThreeAgeInfoList { get; set; }
		public AgeInfo FourAgeInfoList { get; set; }
	}

	public class AgeInfo
    {
		public string CommunityName { get; set; }
		public List<RowInfo> DataRowList { get; set; } 
	}

	public class RowInfo
	{
		public string Event { get; set; }
		public string Name { get; set; }
		public string Level { get; set; }
		public string Phone { get; set; }
		public string IdCard { get; set; }
		public string Address { get; set; }
		public string LoginTime { get; set; }
		public string ReceiveTime { get; set; }
		public string Street { get; set; }
		public string KeepAddress { get; set; }
		public string Area { get; set; }
		public string KeepShareTime { get; set; }
		public string KeepArriveTime { get; set; }
		public string Status { get; set; }
		
	}
}
