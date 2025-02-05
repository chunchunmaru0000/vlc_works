using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace vlc_works
{
	class TextSettings
	{
		public string Path { get; set; }
		public Font Font { get; set; }
		public Color ForeColor { get; set; }
		public Color BackColor { get; set; }

		public static TextSettings ReadSettings(string path = "settings.txt")
		{
			TextSettings settings = new TextSettings();
			settings.Path = path;

			try
			{
				string[] lines = File.ReadAllText(path).Split('\n').Select(s => s.Trim('r')).ToArray();

				int[] c0 = lines[2].Split(';').Select(c => Convert.ToInt32(c)).ToArray();
				int[] c1 = lines[3].Split(';').Select(c => Convert.ToInt32(c)).ToArray();

				settings.Font = new Font(lines[0], Convert.ToSingle(lines[1]));
				settings.ForeColor = Color.FromArgb(c0[0], c0[1], c0[2]);
				settings.BackColor = Color.FromArgb(c1[0], c1[1], c1[2]);
			}
			catch
			{
				FileStream fileStream = File.Create(path);
				byte[] bytes = Encoding.UTF8.GetBytes(string.Join("\r\n", new string[]
				{
					"Microsoft Sans Serif",
					"160",
					"255;0;0",
					"0;0;0"
				}));
				fileStream.Write(bytes, 0, bytes.Length);
				fileStream.Close();

				settings.Font = new Font("Microsoft Sans Serif", 160);
				settings.ForeColor = Color.Red;
				settings.BackColor = Color.Black;
			}

			return settings;
		}

		public void Save()
		{
			FileStream fileStream = File.Create(Path);
			byte[] bytes = Encoding.UTF8.GetBytes(string.Join("\r\n", new string[]
			{
					Font.Name,
					Convert.ToInt32(Font.Size).ToString(),
					string.Join(";", new int[] { ForeColor.R, ForeColor.G, ForeColor.B }.Select(c => c.ToString()).ToArray()),
					string.Join(";", new int[] { BackColor.R, BackColor.G, BackColor.B }.Select(c => c.ToString()).ToArray())
			}));

			fileStream.Write(bytes, 0, bytes.Length);
			fileStream.Close();
		}
	}
}
