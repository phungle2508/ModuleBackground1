using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ModuleBackground;

public class CIniFile
{
	private string filePath;

	public string this[string section, string key]
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			GetPrivateProfileString(section, key, string.Empty, stringBuilder, stringBuilder.Capacity, filePath);
			return stringBuilder.ToString();
		}
		set
		{
			WritePrivateProfileString(section, key, value, filePath);
		}
	}

	[DllImport("kernel32.dll")]
	private static extern int GetPrivateProfileString(string lpApplicationName, string lpKeyName, string lpDefault, StringBuilder lpReturnedstring, int nSize, string lpFileName);

	[DllImport("kernel32.dll")]
	private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpstring, string lpFileName);

	public CIniFile(string filePath)
	{
		this.filePath = filePath;
	}

	public string GetValue(string section, string key, string defaultvalue)
	{
		StringBuilder stringBuilder = new StringBuilder(256);
		GetPrivateProfileString(section, key, defaultvalue, stringBuilder, stringBuilder.Capacity, filePath);
		return stringBuilder.ToString();
	}

	public void GetValues(string section, string key, ref List<string> lstValues)
	{
		lstValues.Clear();
		string defaultvalue = "";
		string value = GetValue(section, key, defaultvalue);
		if (!string.IsNullOrEmpty(value))
		{
			string[] separator = new string[1] { "," };
			string[] collection = value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			lstValues.AddRange(collection);
		}
	}

	public void SetValue(string section, string key, string value)
	{
		WritePrivateProfileString(section, key, value, filePath);
	}
}
