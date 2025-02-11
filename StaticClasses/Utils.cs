using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace vlc_works
{
	// i want here to be only "pure" functions in means of NOT USING external data
	public static class Utils
	{
		#region READ_FILE
		private static string ReadFileToEnd(string fileName)
		{
			using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (var reader = new StreamReader(stream, Encoding.UTF8))
				return reader.ReadToEnd();
		}

		private static string[] GetTextLines(string text) =>
			text.Split('\n')
				.Where(l => l != "" && l != "\r" && l != "\n" && l != "\r\n" && !string.IsNullOrEmpty(l))
				.Select(l =>
					string.Join("=", 
						l
							.Trim('\r')
							.Split('=')
							.Skip(1))
						.HebrewTrim()) // he is from Israel so there is a possibility of him using this symbol
				.ToArray();

		public static string[] GetVideoNames(string videonamestxt)
		{
			try
			{
				string[] lines = GetTextLines(ReadFileToEnd(videonamestxt));
				Console.WriteLine(string.Join("\n", lines));
				return lines;
			}
			catch (Exception exception)
			{
				return LogMessageException(exception, videonamestxt);
			}
		}

		private static string[] LogMessageException(Exception exception, string videonamestxt)
		{
			if (exception is FileNotFoundException)
				MessageBox.Show(
					$"FILE {videonamestxt} WASNT FOUND \n" +
					$"ФАЙЛ {videonamestxt} НЕ БЫЛ НАЙДЕН");
			else if (exception is UnauthorizedAccessException)
				MessageBox.Show(
					$"INSUFFICIENT PERMISSIONS TO READ THE FILE {videonamestxt} \n" +
					$"НЕДОСТАТОЧНО ПРАВ ДЛЯ ЧТЕНИЯ ФАЙЛА {videonamestxt}");
			else if (exception is IOException)
				MessageBox.Show(
					$"UNKNOWN ERROR WHILE READING FILE {videonamestxt} \n" +
					$"НЕИЗВЕСТНАЯ ОШИБКА ПРИ ЧТЕНИИ ФАЙЛА {videonamestxt}");
			else
				MessageBox.Show($"ОШИБКА:\n{exception.Message}");
			Environment.Exit(0);
			return null;
		}
		#endregion
		#region COMMAND_LINE
		public static string[] ParseCommandLineArguments(string commandLine) =>
			commandLine is null || commandLine == "" ?
				new string[] { } :
				commandLine.Split('"');

		public static string GetCodeFromName(string codename, int strFrom, int strTo)
		{
			try
			{
				// "51012345 only 5 nums matter eng.mp4"
				return codename.Substring(strFrom, strTo);
			}
			catch
			{
				return "#";
			}
		}

		public static string GetSafeFileName(string path)
		{
			try { return Path.GetFileName(path); } // sometimes errors
			catch { return path.Split('\\').Last().Split('/').Last(); }
		}
		#endregion
		#region CONVERTERS
		public static readonly Dictionary<Keys, string> ktos = new Dictionary<Keys, string>()
		{
			{ Keys.D0, "0" }, { Keys.D1, "1" },
			{ Keys.D2, "2" }, { Keys.D3, "3" },
			{ Keys.D4, "4" }, { Keys.D5, "5" },
			{ Keys.D6, "6" }, { Keys.D7, "7" },
			{ Keys.D8, "8" }, { Keys.D9, "9" },
			{ Keys.Enter, "E" }
		}; // keys to string
		public static readonly Dictionary<Keys, Langs> ktol = new Dictionary<Keys, Langs>()
		{
			{ Keys.D1, Langs.HEBREW },
			{ Keys.D2, Langs.ENGLISH },
			{ Keys.D3, Langs.RUSSIAN },
		}; // select lang stage nums to lang
		#endregion
	}
}
