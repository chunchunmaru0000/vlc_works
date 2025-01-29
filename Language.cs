using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace vlc_works
{
	public class Language
	{
	    public Langs Lang { get; set; }
		public string VictoryPath { get; set; }
		public string RulesPath { get; set; }
		public string ParamsPath { get; set; }
		public Uri VictoryVideoUri { get; set; }
		public Uri RulesUri { get; set; }
		public Uri ParamsUri { get; set; }

		public Language(Langs lang, string victoryPath, string rulesPath, string paramsPath)
		{
			Lang = lang;
			VictoryPath = victoryPath;
			RulesPath = rulesPath;
			ParamsPath = paramsPath;
			VictoryVideoUri = new Uri(victoryPath);
			RulesUri = new Uri(rulesPath);
			ParamsUri = new Uri(paramsPath);
		}
	}
}
