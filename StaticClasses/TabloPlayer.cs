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
                WriteDefaultJson();
                return;
            }

            try {
                string jsonText = File.ReadAllText(SERVER_INFO_JSON_NAME, System.Text.Encoding.UTF8);
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

        private static Dictionary<TabloText, Action> WriteHandlers { get; } = new Dictionary<TabloText, Action>() {
            { TabloText.IdleWelcome, PlayIdleWelcome },
            { TabloText.GuideToStart, PlayGuideToStart },
            { TabloText.GameInProcess, PlayGameInProcess },
            { TabloText.Win, PlayWin },
            { TabloText.NotWorking, PlayNotWorking },
        };
        public static void Write(TabloText text) => WriteHandlers[text]();

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

        private static async Task PlayProgramAsync(HdScreen screen)
        {
            StartDeleteAllProgramms();
            while (AllProgramXml == null)
                await Task.Delay(33);

            Report($"\n##################### ALL PROGRAMS #########################");
            Report($"\n{AllProgramXml}");
            Report($"\n##################### START DELETE ALL PROGRAMS #########################");

            foreach (XElement e in XDocument.Parse(AllProgramXml).Descendants("Item")) {
                string 
                    id = e.Attribute("Id")?.Value,
                    guid = e.Attribute("Guid")?.Value,
                    name = e.Attribute("Name")?.Value,
                    deleteXml = DELETE_PROGRAM_STRING(id, guid, name);
                SelectedDevice.SendFromXml(deleteXml);

                Report($"\n{deleteXml}");
            }

            AllProgramXml = null;
            Report($"\n{SelectedDevice.SendScreen(screen)}");
        }

        private static void PlayProgram(HdScreen screen)
        {
            Task.Run(async () => {
                try {
                    await PlayProgramAsync(screen);
                } catch (Exception ex) {
                }
            });
        }
        #endregion PLAY_LOGIC
        #region PLAY_TEXT
        private static Random Rnd = new Random();
        private static Dictionary<TabloText, string> TabloTextString { get; } = new Dictionary<TabloText, string>() {
            { TabloText.IdleWelcome,   "          Welcome to the intellectual game GOLDinSAFE. In this game you can win money using your skills and attentiveness. Good luck." },
            { TabloText.GuideToStart,  "          To start the game, enter and stand in front of the machine." },
            { TabloText.GameInProcess, "          The game is in progress, do not enter." },
            { TabloText.Win,           "          You win, congratulations!!!" },
            { TabloText.NotWorking,    "          Sorry, but the skill machine is not working." },
        }; 

        private static (HdScreen, HdProgram) GetScreenAndProgram(string programName)
        {
            HdScreen screen = new HdScreen(new ScreenParam() { isNewScreen = false });
            HdProgram program = new HdProgram(new ProgramParam()
            {
                type = ProgramType.normal,
                guid = Guid.NewGuid().ToString(),
                name = programName,
            });
            screen.Programs.Add(program);
            return (screen, program);
        }

        private static void AddBg(HdProgram program, Color color, int duration)
        {
            HdArea bgarea = program.AddArea(new AreaParam {
                x = 0,
                y = 0,
                width = DeviceInfo.screenWidth,
                height = DeviceInfo.screenHeight,
                guid = Guid.NewGuid().ToString()
            });
            bgarea.AddText(new TextAreaItemParam {
                guid = Guid.NewGuid().ToString(),
                text = "",
                useBackgroundColor = true,
                color = color,
                backgroundColor = color,
                effect = new AreaItemEffect() {
                    inEffet = EffectType.LEFT_PARALLEL_MOVE,
                    outEffet = EffectType.FADE,
                    inSpeed = 1,
                    outSpeed = 1,
                    duration = duration,
                }
            });
        }

        private static void AddText(HdProgram program, string text, Color color, string fontName, AreaItemEffect effect)
        {
            HdArea area = program.AddArea(new AreaParam {
                x = 0,
                y = 0,
                width = DeviceInfo.screenWidth,
                height = DeviceInfo.screenHeight,
                guid = Guid.NewGuid().ToString(),
            });
            
            area.AddText(new TextAreaItemParam {            
                guid = Guid.NewGuid().ToString(),
                fontName = fontName,
                fontSize = 26,
                text = text,
                color = color,
                effect = effect,
            });
        }

        private static int IN_SPEED { get; } = 4;
        private static int DURATION { get; } = 8;

        private static void PlayIdleWelcome()
        {
            TabloText tabloText = TabloText.IdleWelcome;
            var (screen, program) = GetScreenAndProgram(tabloText.ToString());

            AddBg(program, Color.Black, 20);
            AddText(program,
                TabloTextString[tabloText],
                Color.Gold,
                "Arial",
                new AreaItemEffect() {
                    inEffet = EffectType.HT_LEFT_SERIES_MOVE,
                    inSpeed = IN_SPEED,
                    duration = DURATION,
                }
            );
            PlayProgram(screen);
        }

        private static void PlayGuideToStart()
        {
            TabloText tabloText = TabloText.GuideToStart;
            var (screen, program) = GetScreenAndProgram(tabloText.ToString());

            AddBg(program, Color.Black, 20);
            AddText(program,
                TabloTextString[tabloText],
                Color.Gold,
                "Arial",
                new AreaItemEffect() {
                    inEffet = EffectType.HT_LEFT_SERIES_MOVE,
                    inSpeed = IN_SPEED,
                    duration = DURATION,
                }
            );
            PlayProgram(screen);
        }

        private static void PlayGameInProcess()
        {
            TabloText tabloText = TabloText.GameInProcess;
            var (screen, program) = GetScreenAndProgram(tabloText.ToString());

            AddBg(program, Color.Black, 20);
            AddText(program,
                TabloTextString[tabloText],
                Color.Gold,
                "Arial",
                new AreaItemEffect() {
                    inEffet = EffectType.HT_LEFT_SERIES_MOVE,
                    inSpeed = IN_SPEED,
                    duration = DURATION,
                }
            );
            PlayProgram(screen);
        }

        private static void PlayWin()
        {
            TabloText tabloText = TabloText.Win;
            var (screen, program) = GetScreenAndProgram(tabloText.ToString());

            AddBg(program, Color.Black, 20);
            AddText(program,
                TabloTextString[tabloText], 
                Color.Gold,
                "Arial", 
                new AreaItemEffect() {
                    inEffet = EffectType.HT_LEFT_SERIES_MOVE,
                    inSpeed = IN_SPEED,
                    duration = DURATION,
                }
            );
            PlayProgram(screen);
        }

        private static void PlayNotWorking()
        {
            TabloText tabloText = TabloText.NotWorking;
            var (screen, program) = GetScreenAndProgram(tabloText.ToString());

            AddBg(program, Color.Black, 20);
            AddText(program,
                TabloTextString[tabloText],
                Color.Gold,
                "Arial",
                new AreaItemEffect() {
                    inEffet = EffectType.HT_LEFT_SERIES_MOVE,
                    inSpeed = IN_SPEED,
                    duration = DURATION,
                }
            );
            PlayProgram(screen);
        }
        #endregion PLAY_TEXT
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
