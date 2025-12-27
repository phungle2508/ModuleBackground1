using System.Collections;

namespace ModuleBackground;

public class AlphanumComparatorFast : IComparer
{
	public int Compare(object x, object y)
	{
		if (!(x is string text))
		{
			return 0;
		}
		if (!(y is string text2))
		{
			return 0;
		}
		int length = text.Length;
		int length2 = text2.Length;
		int num = 0;
		int num2 = 0;
		while (num < length && num2 < length2)
		{
			char c = text[num];
			char c2 = text2[num2];
			char[] array = new char[length];
			int num3 = 0;
			char[] array2 = new char[length2];
			int num4 = 0;
			do
			{
				array[num3++] = c;
				num++;
				if (num >= length)
				{
					break;
				}
				c = text[num];
			}
			while (char.IsDigit(c) == char.IsDigit(array[0]));
			do
			{
				array2[num4++] = c2;
				num2++;
				if (num2 >= length2)
				{
					break;
				}
				c2 = text2[num2];
			}
			while (char.IsDigit(c2) == char.IsDigit(array2[0]));
			string text3 = new string(array);
			string text4 = new string(array2);
			int num6;
			if (char.IsDigit(array[0]) && char.IsDigit(array2[0]))
			{
				int num5 = int.Parse(text3);
				int value = int.Parse(text4);
				num6 = num5.CompareTo(value);
			}
			else
			{
				num6 = text3.CompareTo(text4);
			}
			if (num6 != 0)
			{
				return num6;
			}
		}
		return length - length2;
	}
}
