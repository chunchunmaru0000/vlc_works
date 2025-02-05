using System;

namespace vlc_works
{
	public class PathUri
	{
		public string Path { get; set; }
		public Uri Uri { get; set; }

		public PathUri(string path)
		{
			Path = path;
			Uri = new Uri(path);
		}
	}
}
