using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vlc_works
{
	public static class Utils
	{
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
						.Replace("\u202A", "")) // he is from Israel so there is a possibility of him using this symbol
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
	}
}
