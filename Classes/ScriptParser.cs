using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vlc_works.Classes;

namespace vlc_works
{
    public class ScriptParser
    {
        private string ScriptFilePath { get; set; }

        public ScriptParser(string scriptFilePath)
        {
            ScriptFilePath = scriptFilePath;
        }

        public List<GameScript> Parse()
        {
            List<GameScript> gameScripts = new List<GameScript>();



            return gameScripts;
        }
    }
}
