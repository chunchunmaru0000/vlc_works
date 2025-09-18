using SDKLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.Json;

namespace vlc_works015
{
    public enum TabloText
    {
        IdleWelcome,
        GuideToStart,
        IdleWelcomeAndGuideToStart,
        GameInProcess,
        Win,
        NotWorking,
    }

    public static class TabloPlayer
    {
        #region SERVER_INFO
        private const string SERVER_INFO_JSON_NAME = "ServerInfo.json";
        private static ServerInfo ServerInfo { get; set; } = new ServerInfo()
        {
            port = 10001,
            host = "169.254.255.254"
            //host = "192.168.1.254"
        };

        private static void WriteDefaultJson() => File.WriteAllText(
            SERVER_INFO_JSON_NAME,
            JsonSerializer.Serialize(ServerInfo),
            System.Text.Encoding.UTF8
        );

        private static void InitServerInfo()
        {
            if (!File.Exists(SERVER_INFO_JSON_NAME)) {
                Console.WriteLine($"#INFO. device info |{JsonSerializer.Serialize(ServerInfo)}|");
                WriteDefaultJson();
                return;
            }

            try {
                string jsonText = File
                    .ReadAllText(SERVER_INFO_JSON_NAME, System.Text.Encoding.UTF8)
                    .HebrewTrim();
                ServerInfo = JsonSerializer.Deserialize<ServerInfo>(jsonText);
            } catch { WriteDefaultJson(); }
        }
        #endregion SERVER_INFO
        #region INTERFACE
        public static HDCommunicationManager CommManager { get; set; }
        public static Device SelectedDevice { get; private set; }
        public static DeviceInfo DeviceInfo { get; set; }

        public static (bool, string) Init(MsgReportEventHandler msgReport)
        {
            InitServerInfo();

            CommManager = new HDCommunicationManager();
            CommManager.MsgReport += msgReport;
            CommManager.ResolvedInfoReport += ResolvedInfoReport;

            SelectedDevice = CommManager.AddDevice(ServerInfo.host, out string ex);

            if (ex.Length > 0)
                return (false, $"Ошибка:\n - {ex}");
            if (SelectedDevice == null)
                return (false, $"Ошибка: устройство null");

            DeviceInfo = SelectedDevice.GetDeviceInfo();
            return (true, ex);
        }

        public static void Write(TabloText text) 
        {
            Console.WriteLine($"#INFO. try TabloPlayer.Write|{text}|");
            if (SelectedDevice == null || DeviceInfo == null)
                return;

            string xml = File
                .ReadAllText($"TabloXml\\{text}.xml.txt", System.Text.Encoding.UTF8)
                .HebrewTrim();
            PlayProgram(xml);

            if (text == TabloText.GuideToStart)
                StartGuideToStartTimer();
            else if (IdleTextOffTimer != null) {
                IdleTextOffTimer?.Dispose();
                IdleTextOffTimer = null;
            }
        }

        private static void ResolvedInfoReport(Device device, ResolveInfo ri)
        {
            if (ri.errorCode != ErrorCode.kSuccess)
                return;

            if (ri.method == "GetAllProgram") {
                AllProgramXml = ri.srcXml;
            }
        }
        #endregion INTERFACE
        #region PLAY_LOGIC
        private static readonly object TimerLock = new object();
        private static System.Threading.Timer IdleTextOffTimer { get; set; } = null;
        private static TimeSpan IdleTextOffDelay { get; } = TimeSpan.FromMinutes(5);

        private static void StartGuideToStartTimer()
        {
            lock (TimerLock) {
                IdleTextOffTimer?.Dispose();
                IdleTextOffTimer = new System.Threading.Timer(
                    (s) => {
                        Write(TabloText.IdleWelcomeAndGuideToStart);

                        IdleTextOffTimer?.Dispose();
                        IdleTextOffTimer = null;
                    },
                    null,
                    IdleTextOffDelay,
                    InputKey.MinusOneMilisecond
                );
            }
        }

        private static void Report(string msg) => CommManager.ReportMsg(SelectedDevice, msg);

        private static string GET_ALL_PROGRAM_STRING { get; } = @"<?xml version = ""1.0"" encoding = ""utf-8"" ?>
<sdk guid=""##GUID"">
    <in method=""GetAllProgram"" />
</sdk>";

        private static string DELETE_PROGRAM_STRING(string id, string guid, string name) => $@"<?xml version=""1.0"" encoding=""utf-8""?>
<sdk guid=""##GUID"">
    <in method=""DeleteProgram"">
       <program type=""normal"" id=""{id}"" guid=""{guid}"" name=""{name}"">
        <backgroundMusic />
        <playControl count=""1"" disabled=""false"" />
      </program>
    </in>
</sdk>
";

        private static string AllProgramXml { get; set; } = null;

        private static void StartDeleteAllProgramms() => 
            SelectedDevice.SendFromXml(GET_ALL_PROGRAM_STRING);

        private static async Task PlayProgramAsync(string xml)
        {
            StartDeleteAllProgramms();
            while (AllProgramXml == null)
                await Task.Delay(33);

            Report($"\n##################### ALL PROGRAMS #########################");
            Report($"\n{AllProgramXml}");
            Report($"\n##################### START DELETE ALL PROGRAMS #########################");

            foreach (XElement e in XDocument.Parse(AllProgramXml).Descendants("Item"))
            {
                string
                    id = e.Attribute("Id")?.Value,
                    guid = e.Attribute("Guid")?.Value,
                    name = e.Attribute("Name")?.Value,
                    deleteXml = DELETE_PROGRAM_STRING(id, guid, name);
                SelectedDevice.SendFromXml(deleteXml);

                Report($"\n{deleteXml}");
            }

            AllProgramXml = null;
            SelectedDevice.SendFromXml(xml);
            Report($"\n{xml}");
        }

        private static void PlayProgram(string xml)
        {
            Task.Run(async () => {
                try {
                    await PlayProgramAsync(xml);
                } catch (Exception ex) {
                }
            });
        }
        #endregion PLAY_LOGIC
    }
}
// FROM EXAMPLE
/*
// add image
else if (tabControl_Program.SelectedIndex == 1)
{
    ImageAreaItemParam imageItem = new ImageAreaItemParam();
    if (sender == button_AddProgram)
    {
        _AreaImageItemGUID = Guid.NewGuid().ToString();
    }
    imageItem.guid = _AreaImageItemGUID;
    imageItem.file = comboBox_image.Text;
    imageItem.effect.inEffet = (EffectType)comboBox_InEffectImage.SelectedValue;
    imageItem.effect.outEffet = (EffectType)comboBox_OutEffectImage.SelectedValue;
    imageItem.effect.inSpeed = int.Parse(comboBox_InSpeedImage.Text);
    imageItem.effect.outSpeed = int.Parse(comboBox_OutSpeedImage.Text);
    imageItem.effect.duration = Int32.Parse(numericUpDown_staytimeImage.Value.ToString());

    area.AddImage(imageItem);
}
// add video
else if (tabControl_Program.SelectedIndex == 2)
{
    VideoAreaItemParam videoItem = new VideoAreaItemParam();
    if (sender == button_AddProgram)
    {
        _AreaVideoItemGUID = Guid.NewGuid().ToString();
    }
    videoItem.guid = _AreaVideoItemGUID;
    videoItem.file = comboBox_video.Text;
    area.AddVedio(videoItem);
}
// add clock
else if (tabControl_Program.SelectedIndex == 3)
{
    ClockAreaItemParam item = new ClockAreaItemParam();
    if (sender == button_AddProgram)
    {
        _AreaClockItemGUID = Guid.NewGuid().ToString();
    }
    item.guid = _AreaClockItemGUID;
    item.clockType = (ClockType)comboBox_Clocktype.SelectedIndex;
    item.date.dateDisplay = checkBox_ClockDate.Checked;
    item.date.dateFormat = comboBox_ClockDate.SelectedIndex;
    item.date.dateColor = button_ClockDateColor.ForeColor;

    item.time.timeDisplay = checkBox_ClockTime.Checked;
    item.time.timeFormat = comboBox_ClockTime.SelectedIndex;
    item.time.timeColor = button_ClockTimeColor.ForeColor;

    item.week.weekDisplay = checkBox_ClockWeek.Checked;
    item.week.weekFormat = comboBox_ClockWeek.SelectedIndex;
    item.week.weekColor = button_ClockWeekColor.ForeColor;

    item.title.titleDisplay = checkBox_ClockTitle.Checked;
    item.title.titleValue = textBox_ClockTitle.Text;
    item.title.titleColor = button_ClockTitleColor.ForeColor;

    item.lunarCalendar.lunarCalendarDisplay = checkBox_Clocklunarcalendar.Checked;
    item.lunarCalendar.lunarCalendarColor = button_ClockLunarCalendarColor.ForeColor;

    area.AddClock(item);
}
 */
