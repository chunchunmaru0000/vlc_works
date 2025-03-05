using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Policy;
using System.Xml.Linq;
using UsbRelayNet.RelayLib;

namespace vlc_works
{
    public class DevicesSettings
    {
        private string SettingsFilePath { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        private string[] AllParametersKeys = new string[] {
            "MONEY",
            "LASER",
            "RELAY",

            "WEB_CAMERA",

            "LOCAL_PORT",

            "MACHINE_NUMBER",
            "MACHINE_IP",
            "MACHINE_PORT",
            "MACHINE_PASSWORD",
        };

        public DevicesSettings(string settingsFilePath)
        {
            SettingsFilePath = settingsFilePath;
        }

        public bool Parse()
        {
            if (!File.Exists(SettingsFilePath))
                return false;

            Dictionary<string, string> parameters = 
                File.ReadAllText(SettingsFilePath, System.Text.Encoding.UTF8)
                .Replace("\r", "")
                .HebrewTrim()
                .Trim()
                .Split('\n')
                .Select(line => {
                    string[] parts = line.Split('=');
                    if (parts.Length > 2)
                        parts[1] = string.Join("=", parts.Skip(1));
                    return parts;
                })
                .Where(line => line.Length >= 2)
                .ToDictionary(p => p[0].Trim(), p => p[1].Trim());

            return AllParametersKeys.All(k => parameters.ContainsKey(k));
        }
    }
}
