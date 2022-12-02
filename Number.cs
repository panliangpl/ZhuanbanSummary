using System;
using System.Collections.Generic;

namespace ALLCheck
{
	// Token: 0x02000007 RID: 7
	public class Number
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000016 RID: 22 RVA: 0x00003C34 File Offset: 0x00001E34
		// (set) Token: 0x06000017 RID: 23 RVA: 0x00003C3C File Offset: 0x00001E3C
		public string Characters { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000018 RID: 24 RVA: 0x00003C48 File Offset: 0x00001E48
		public int Length
		{
			get
			{
				bool flag = this.Characters != null;
				int result;
				if (flag)
				{
					result = this.Characters.Length;
				}
				else
				{
					result = 0;
				}
				return result;
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00003C76 File Offset: 0x00001E76
		public Number()
		{
			this.Characters = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00003C8C File Offset: 0x00001E8C
		public Number(string characters)
		{
			this.Characters = characters;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00003CA0 File Offset: 0x00001EA0
		public string ToString(int number)
		{
			List<string> list = new List<string>();
			int i = number;
			while (i > 0)
			{
				int index = i % this.Length;
				i = Math.Abs(i / this.Length);
				string item = this.Characters[index].ToString();
				list.Insert(0, item);
			}
			return string.Join("", list.ToArray());
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00003D10 File Offset: 0x00001F10
		public int FromString(string str)
		{
			int num = 0;
			int num2 = str.Length - 1;
			char[] array = str.ToCharArray();
			foreach (char value in array)
			{
				bool flag = this.Characters.Contains(value.ToString());
				if (flag)
				{
					num += this.Characters.IndexOf(value) * (int)Math.Pow((double)this.Length, (double)num2);
					num2--;
				}
			}
			return num;
		}
	}
}
