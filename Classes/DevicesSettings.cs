using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace vlc_works
{
    public class DevicesSettings
    {
        private string SettingsFilePath { get; set; }
        public Dictionary<string, string> Parameters { get; private set; } = new Dictionary<string, string>();

        private static string[] AllParametersKeys { get; } = new string[] {
            "MONEY",
            "RELAY",
            "LASER",

            "WEB_CAMERA",

            "LOCAL_PORT",

            "MACHINE_IP",
            "MACHINE_PORT",
            "MACHINE_PASSWORD",
            "MACHINE_NUMBER",
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
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => {
                    string[] parts = line.Split('=');
                    if (parts.Length > 2)
                        parts[1] = string.Join("=", parts.Skip(1));
                    return parts;
                })
                .Where(line => line.Length >= 2)
                .ToDictionary(p => p[0].Trim(), p => p[1].Trim());

            bool successfullParse = AllParametersKeys.All(k => parameters.ContainsKey(k));
            if (successfullParse)
                Parameters = parameters;

            return successfullParse;
        }

        public void Add(string param, string value)
        {
            Parameters[param] = value;
            Utils.print($"Parameters[{param}] = {value}");

            if (AllParametersKeys.All(k => Parameters.ContainsKey(k))) {
                SaveFile();
                Utils.print($"SAVED FILE: [{SettingsFilePath}]");
            }
            else
                Utils.print(
                    $"THERE ARE NOT ALL PARAMETERS SO DO NOT SAVE FOR NOW [{SettingsFilePath}]\n" +
                    $"ALL PARAMS: \n\t{string.Join("\n\t", Parameters.Select(p => $"[{p.Key} -> {p.Value}]"))}"
                    );
        }

        private void SaveFile() =>
            File
            .WriteAllText(
                SettingsFilePath, 
                string.Join("\r\n", Parameters.Select(p => $"{p.Key} = {p.Value}")), 
                System.Text.Encoding.UTF8);
    }
}
