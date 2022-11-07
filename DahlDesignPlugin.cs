using DahlDesignPropertiesNG;
using GameReaderCommon;
using SimHub.Plugins;
using System;
using System.Reflection;
using System.Windows.Media;
using System.Xml.Serialization;
using IC = DahlDesignPropertiesNG.IntervalController;



namespace DahlDesignPropertiesNG
{
    [PluginDescription("Properties Plugin for the DahlDesign Trifecta")]
    [PluginAuthor("Andreas Dahl")]
    [PluginName("DahlDesignPlugin")]
    public class DahlDesignPlugin : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        public DataPluginSettings Settings;
        public PluginManager PluginManager { get; set; }
        public ImageSource PictureIcon => this.ToIcon(Properties.Resources.sdkmenuicon);
        public string LeftMenuTitle => "Dahl Design NG";
        /// <summary>
        /// Counter that updates every loop.  Lets us keep track of where we are in the 60Hz update cycle to not run too much code in the expensive data path.
        /// </summary>
        private int UpdateCounter = 0;
        private Calcs c;
        private SimHubProperties s;


        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            if (!(data.GameRunning || data.GameReplay)) {
                return; //Early return if the game isn't even running or replaying.
            }
            
            c.UpdateGameData(ref data); //Update the game data in our calcs class.  Passing by reference so its inexpensive.
            UpdateCounter = ++UpdateCounter == 60 ? 0 : UpdateCounter; //Update the counter every call.
            foreach (int Hz in IC.HzValues) // For each possible Hz value, give it a shot
            {
                if (IC.ShouldRunUpdate(CounterVal: UpdateCounter, Hz: Hz)) //If we should be running this Hz value, get started.  Otherwise move on.
                {
                    foreach (MethodInfo m in c.IntervalMethodMapping[Hz]) { // Each method - lets extract it and execute it.
                        m.Invoke(c, null); //Invoke the method against the class, with no parameters
                    }
                }
            }
        }

        public void Init(PluginManager pluginManager)
        {
            SimHub.Logging.Current.Info("Starting DahlDesignProperties NG plugin");

            // Load settings
            Settings = this.ReadCommonSettings<DataPluginSettings>("GeneralSettings", () => new DataPluginSettings());
            // Setup our properties manager
            s = new SimHubProperties(ref pluginManager, this.GetType());
            SetupProperties();
            // Setup our calculation manager
            c = new Calcs(ref pluginManager, ref Settings, ref s); // initalize the calcs class.

        }
        public void End(PluginManager pluginManager)
        {
            // Save settings
            this.SaveCommonSettings("GeneralSettings", Settings);
        }
        public System.Windows.Controls.Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return new SettingsControl(this);
        }
        private void SetupProperties()
        {
            //Add our basic properies
            s.AddProperty("Version", Assembly.GetEntryAssembly().GetName().Version);
            s.AddProperty("BitePointAdjust", false);
            s.AddProperty("FuelSaveDelta", 0);
            s.AddProperty("LEDWarnings", false);
            s.AddProperty("SpotterMode", false);
            s.AddProperty("LaunchScreen", false);
            s.AddProperty("NoBoost", false);
            s.AddProperty("Radio", false);
            s.AddProperty("RadioName", "");
            s.AddProperty("RadioPosition", 0);
            s.AddProperty("RadioIsSpectator", false);
            s.AddProperty("PaceCheck", false);
            s.AddProperty("PitScreen", false);
            s.AddProperty("TCOffTimer", new TimeSpan(0));
            s.AddProperty("PitMenu", 1);
            s.AddProperty("PitSavePaceLock", false);
            s.AddProperty("InCarMenu", 0);
            //properties.AddProperty("DDUstartLED", Settings.DDUstartLED);
            //properties.AddProperty("SW1startLED", Settings.SW1startLED);
            //properties.AddProperty("DDUEnabled", Settings.DDUEnabled);
            //properties.AddProperty("SW1Enabled", Settings.SW1Enabled);
            //properties.AddProperty("DashLEDEnabled", Settings.DashLEDEnabled);
            //properties.AddProperty("ShowMapEnabled", Settings.ShowMapEnabled);
            //properties.AddProperty("DashType", Settings.DashType);
            //properties.AddProperty("LapInfoScreen", Settings.LapInfoScreen);
            //properties.AddProperty("ShiftTimingAssist", Settings.ShiftTimingAssist);
            //properties.AddProperty("ShiftWarning", Settings.ShiftWarning);
            //properties.AddProperty("ARBswapped", Settings.SupercarSwapPosition);
            //properties.AddProperty("ARBstiffForward", Settings.SupercarARBDirection);
            //properties.AddProperty("SmallFuelIncrement", Settings.SmallFuelIncrement);
            //properties.AddProperty("LargeFuelIncrement", Settings.LargeFuelIncrement);
            //properties.AddProperty("CoupleInCarToPit", Settings.CoupleInCarToPit);

            s.AddProperty("Idle", true);
            s.AddProperty("SmoothGear", "");
            s.AddProperty("TrackEntry", false);
            s.AddProperty("LastGearMaxRPM", 0);
            s.AddProperty("LastGear", 0);
            s.AddProperty("OvertakeMode", false);

            s.AddProperty("StopWatch", TimeSpan.FromSeconds(0));
            s.AddProperty("StopWatchSplit", TimeSpan.FromSeconds(0));

            s.AddProperty("AccelerationTo100KPH", 0);
            s.AddProperty("AccelerationTo200KPH", 0);
            s.AddProperty("BrakeCurveValues", "");
            s.AddProperty("BrakeCurvePeak", 0);
            s.AddProperty("BrakeCurveAUC", 0);
            s.AddProperty("ThrottleCurveValues", "");
            s.AddProperty("ThrottleAgro", 0);

            s.AddProperty("ERSTarget", 0);
            s.AddProperty("ERSCharges", 0);
            s.AddProperty("TCActive", false);
            s.AddProperty("TCToggle", false);
            s.AddProperty("ABSToggle", false);
            s.AddProperty("HasTC", false);
            s.AddProperty("HasABS", false);
            s.AddProperty("HasDRS", false);
            s.AddProperty("DRSState", "");
            s.AddProperty("HasAntiStall", false);
            s.AddProperty("HasOvertake", false);
            s.AddProperty("MapHigh");
            s.AddProperty("MapLow");
            s.AddProperty("P2PCount", -1);
            s.AddProperty("P2PStatus", false);
            s.AddProperty("DRSCount", -1);

            s.AddProperty("SlipLF", 0);
            s.AddProperty("SlipRF", 0);
            s.AddProperty("SlipLR", 0);
            s.AddProperty("SlipRR", 0);


            s.AddProperty("AnimationType", 1);
            s.AddProperty("ShiftLightRPM", 0);
            s.AddProperty("ReactionTime", 0);

            s.AddProperty("Position", 0);
            s.AddProperty("HotLapPosition", 0);
            s.AddProperty("RaceFinished", false);
            s.AddProperty("SoF", 0);
            s.AddProperty("IRchange", 0);
            s.AddProperty("MyClassColor", "");


            s.AddProperty("OptimalShiftGear1", 0);
            s.AddProperty("OptimalShiftGear2", 0);
            s.AddProperty("OptimalShiftGear3", 0);
            s.AddProperty("OptimalShiftGear4", 0);
            s.AddProperty("OptimalShiftGear5", 0);
            s.AddProperty("OptimalShiftGear6", 0);
            s.AddProperty("OptimalShiftGear7", 0);
            s.AddProperty("OptimalShiftCurrentGear", 0);
            s.AddProperty("OptimalShiftLastGear", 0);

            s.AddProperty("TrueRevLimiter", 0);
            s.AddProperty("IdleRPM", 0);

            s.AddProperty("CenterDashType", "");
            s.AddProperty("MenuType", "");

            s.AddProperty("LaunchBitePoint", 0);
            s.AddProperty("LaunchSpin", 0);
            s.AddProperty("LaunchIdealRangeStart", 0);
            s.AddProperty("LaunchIdealRangeStop", 0);
            s.AddProperty("LaunchGearRelease", 0);
            s.AddProperty("LaunchGearReleased", 0);
            s.AddProperty("LaunchTimeRelease", 0);
            s.AddProperty("LaunchTimeReleased", 0);
            s.AddProperty("HighPower", false);
            s.AddProperty("LaunchThrottle", 0);


            s.AddProperty("ApproximateCalculations", false);
            s.AddProperty("LapsRemaining", 0);
            s.AddProperty("LapBalance", 0);

            s.AddProperty("LapStatus", 0);

            s.AddProperty("StintTimer", new TimeSpan(0));
            s.AddProperty("StintTotalTime", new TimeSpan(0));
            s.AddProperty("StintTotalHotlaps", 0);
            s.AddProperty("StintCurrentHotlap", 0);
            s.AddProperty("StintValidLaps", 0);
            s.AddProperty("StintInvalidLaps", 0);

            s.AddProperty("Pace", new TimeSpan(0));

            s.AddProperty("PitBoxPosition", 1);
            s.AddProperty("PitBoxApproach", false);
            s.AddProperty("PitEntry", false);
            s.AddProperty("PitSpeeding", false);

            s.AddProperty("SessionBestLap", new TimeSpan(0));

            s.AddProperty("HotlapLivePosition", 0);

            s.AddProperty("QualyWarmUpLap", false);
            s.AddProperty("QualyLap1Status", 0);
            s.AddProperty("QualyLap2Status", 0);
            s.AddProperty("QualyLap1Time", new TimeSpan(0));
            s.AddProperty("QualyLap2Time", new TimeSpan(0));

            s.AddProperty("Lap01Time", new TimeSpan(0));
            s.AddProperty("Lap02Time", new TimeSpan(0));
            s.AddProperty("Lap03Time", new TimeSpan(0));
            s.AddProperty("Lap04Time", new TimeSpan(0));
            s.AddProperty("Lap05Time", new TimeSpan(0));
            s.AddProperty("Lap06Time", new TimeSpan(0));
            s.AddProperty("Lap07Time", new TimeSpan(0));
            s.AddProperty("Lap08Time", new TimeSpan(0));

            s.AddProperty("Lap01Status", 0);
            s.AddProperty("Lap02Status", 0);
            s.AddProperty("Lap03Status", 0);
            s.AddProperty("Lap04Status", 0);
            s.AddProperty("Lap05Status", 0);
            s.AddProperty("Lap06Status", 0);
            s.AddProperty("Lap07Status", 0);
            s.AddProperty("Lap08Status", 0);

            s.AddProperty("Lap01Delta", 0);
            s.AddProperty("Lap02Delta", 0);
            s.AddProperty("Lap03Delta", 0);
            s.AddProperty("Lap04Delta", 0);
            s.AddProperty("Lap05Delta", 0);
            s.AddProperty("Lap06Delta", 0);
            s.AddProperty("Lap07Delta", 0);
            s.AddProperty("Lap08Delta", 0);

            s.AddProperty("CurrentSector", 0);
            s.AddProperty("CurrentSector1Time", new TimeSpan(0));
            s.AddProperty("CurrentSector2Time", new TimeSpan(0));
            s.AddProperty("CurrentSector3Time", new TimeSpan(0));
            s.AddProperty("CurrentSector1Delta", 0);
            s.AddProperty("CurrentSector2Delta", 0);
            s.AddProperty("CurrentSector3Delta", 0);
            s.AddProperty("CurrentSector1Status", 0);
            s.AddProperty("CurrentSector2Status", 0);
            s.AddProperty("CurrentSector3Status", 0);
            s.AddProperty("SessionBestSector1", new TimeSpan(0));
            s.AddProperty("SessionBestSector2", new TimeSpan(0));
            s.AddProperty("SessionBestSector3", new TimeSpan(0));
            s.AddProperty("Sector1Pace", new TimeSpan(0));
            s.AddProperty("Sector2Pace", new TimeSpan(0));
            s.AddProperty("Sector3Pace", new TimeSpan(0));
            s.AddProperty("Sector1Score", 0);
            s.AddProperty("Sector2Score", 0);
            s.AddProperty("Sector3Score", 0);

            s.AddProperty("OptimalLapTime", new TimeSpan(0));

            s.AddProperty("Lap01Sector1Time", new TimeSpan(0));
            s.AddProperty("Lap01Sector2Time", new TimeSpan(0));
            s.AddProperty("Lap01Sector3Time", new TimeSpan(0));
            s.AddProperty("Lap01Sector1Delta", 0);
            s.AddProperty("Lap01Sector2Delta", 0);
            s.AddProperty("Lap01Sector3Delta", 0);
            s.AddProperty("Lap01Sector1Status", 0);
            s.AddProperty("Lap01Sector2Status", 0);
            s.AddProperty("Lap01Sector3Status", 0);
            s.AddProperty("Lap01FuelTargetDelta", 0);

            s.AddProperty("Lap02Sector1Time", new TimeSpan(0));
            s.AddProperty("Lap02Sector2Time", new TimeSpan(0));
            s.AddProperty("Lap02Sector3Time", new TimeSpan(0));
            s.AddProperty("Lap02Sector1Delta", 0);
            s.AddProperty("Lap02Sector2Delta", 0);
            s.AddProperty("Lap02Sector3Delta", 0);
            s.AddProperty("Lap02Sector1Status", 0);
            s.AddProperty("Lap02Sector2Status", 0);
            s.AddProperty("Lap02Sector3Status", 0);
            s.AddProperty("Lap02FuelTargetDelta", 0);

            s.AddProperty("Lap03Sector1Time", new TimeSpan(0));
            s.AddProperty("Lap03Sector2Time", new TimeSpan(0));
            s.AddProperty("Lap03Sector3Time", new TimeSpan(0));
            s.AddProperty("Lap03Sector1Delta", 0);
            s.AddProperty("Lap03Sector2Delta", 0);
            s.AddProperty("Lap03Sector3Delta", 0);
            s.AddProperty("Lap03Sector1Status", 0);
            s.AddProperty("Lap03Sector2Status", 0);
            s.AddProperty("Lap03Sector3Status", 0);
            s.AddProperty("Lap03FuelTargetDelta", 0);

            s.AddProperty("Lap04Sector1Time", new TimeSpan(0));
            s.AddProperty("Lap04Sector2Time", new TimeSpan(0));
            s.AddProperty("Lap04Sector3Time", new TimeSpan(0));
            s.AddProperty("Lap04Sector1Delta", 0);
            s.AddProperty("Lap04Sector2Delta", 0);
            s.AddProperty("Lap04Sector3Delta", 0);
            s.AddProperty("Lap04Sector1Status", 0);
            s.AddProperty("Lap04Sector2Status", 0);
            s.AddProperty("Lap04Sector3Status", 0);
            s.AddProperty("Lap04FuelTargetDelta", 0);

            s.AddProperty("Lap05Sector1Time", new TimeSpan(0));
            s.AddProperty("Lap05Sector2Time", new TimeSpan(0));
            s.AddProperty("Lap05Sector3Time", new TimeSpan(0));
            s.AddProperty("Lap05Sector1Delta", 0);
            s.AddProperty("Lap05Sector2Delta", 0);
            s.AddProperty("Lap05Sector3Delta", 0);
            s.AddProperty("Lap05Sector1Status", 0);
            s.AddProperty("Lap05Sector2Status", 0);
            s.AddProperty("Lap05Sector3Status", 0);
            s.AddProperty("Lap05FuelTargetDelta", 0);

            s.AddProperty("Lap06Sector1Time", new TimeSpan(0));
            s.AddProperty("Lap06Sector2Time", new TimeSpan(0));
            s.AddProperty("Lap06Sector3Time", new TimeSpan(0));
            s.AddProperty("Lap06Sector1Delta", 0);
            s.AddProperty("Lap06Sector2Delta", 0);
            s.AddProperty("Lap06Sector3Delta", 0);
            s.AddProperty("Lap06Sector1Status", 0);
            s.AddProperty("Lap06Sector2Status", 0);
            s.AddProperty("Lap06Sector3Status", 0);
            s.AddProperty("Lap06FuelTargetDelta", 0);

            s.AddProperty("Lap07Sector1Time", new TimeSpan(0));
            s.AddProperty("Lap07Sector2Time", new TimeSpan(0));
            s.AddProperty("Lap07Sector3Time", new TimeSpan(0));
            s.AddProperty("Lap07Sector1Delta", 0);
            s.AddProperty("Lap07Sector2Delta", 0);
            s.AddProperty("Lap07Sector3Delta", 0);
            s.AddProperty("Lap07Sector1Status", 0);
            s.AddProperty("Lap07Sector2Status", 0);
            s.AddProperty("Lap07Sector3Status", 0);
            s.AddProperty("Lap07FuelTargetDelta", 0);

            s.AddProperty("Lap08Sector1Time", new TimeSpan(0));
            s.AddProperty("Lap08Sector2Time", new TimeSpan(0));
            s.AddProperty("Lap08Sector3Time", new TimeSpan(0));
            s.AddProperty("Lap08Sector1Delta", 0);
            s.AddProperty("Lap08Sector2Delta", 0);
            s.AddProperty("Lap08Sector3Delta", 0);
            s.AddProperty("Lap08Sector1Status", 0);
            s.AddProperty("Lap08Sector2Status", 0);
            s.AddProperty("Lap08Sector3Status", 0);
            s.AddProperty("Lap08FuelTargetDelta", 0);

            s.AddProperty("LapRecord", new TimeSpan(0));
            s.AddProperty("DeltaLastLap", 0);
            s.AddProperty("DeltaSessionBest", 0);
            s.AddProperty("DeltaLapRecord", 0);
            s.AddProperty("DeltaLastLapChange", "");
            s.AddProperty("DeltaSessionBestChange", "");
            s.AddProperty("DeltaLapRecordChange", "");

            s.AddProperty("P1Gap", 0);
            s.AddProperty("P1Name", "");
            s.AddProperty("P1Pace", new TimeSpan(0));
            s.AddProperty("P1Finished", false);
            s.AddProperty("P1LapBalance", 0);

            s.AddProperty("ClassP1Gap", 0);
            s.AddProperty("ClassP1Name", "");
            s.AddProperty("ClassP1Pace", new TimeSpan(0));
            s.AddProperty("ClassP1RealGap", 0);

            s.AddProperty("LuckyDogGap", 0);
            s.AddProperty("LuckyDogRealGap", 0);
            s.AddProperty("LuckyDogName", "");
            s.AddProperty("LuckyDogPositionsAhead", 0);

            s.AddProperty("AheadName", "");
            s.AddProperty("AheadGap", 0);
            s.AddProperty("AheadPace", new TimeSpan(0));
            s.AddProperty("AheadBestLap", new TimeSpan(0));
            s.AddProperty("AheadIsConnected", false);
            s.AddProperty("AheadIsInPit", false);
            s.AddProperty("AheadSlowLap", false);
            s.AddProperty("AheadPrognosis", 0);
            s.AddProperty("AheadLapsToOvertake", 0);
            s.AddProperty("AheadLapsSincePit", -1);
            s.AddProperty("AheadP2PStatus", false);
            s.AddProperty("AheadP2PCount", -1);
            s.AddProperty("AheadRealGap", 0);

            s.AddProperty("BehindName", "");
            s.AddProperty("BehindGap", 0);
            s.AddProperty("BehindPace", new TimeSpan(0));
            s.AddProperty("BehindBestLap", new TimeSpan(0));
            s.AddProperty("BehindIsConnected", false);
            s.AddProperty("BehindIsInPit", false);
            s.AddProperty("BehindSlowLap", false);
            s.AddProperty("BehindPrognosis", 0);
            s.AddProperty("BehindLapsToOvertake", 0);
            s.AddProperty("BehindLapsSincePit", -1);
            s.AddProperty("BehindP2PStatus", false);
            s.AddProperty("BehindP2PCount", -1);
            s.AddProperty("BehindRealGap", 0);

            s.AddProperty("LeftCarGap", 0);
            s.AddProperty("LeftCarName", "");
            s.AddProperty("RightCarGap", 0);
            s.AddProperty("RightCarName", "");

            s.AddProperty("CarAhead01Gap", 0);
            s.AddProperty("CarAhead01RaceGap", 0);
            s.AddProperty("CarAhead01BestLap", new TimeSpan(0));
            s.AddProperty("CarAhead01Name", "");
            s.AddProperty("CarAhead01Position", 0);
            s.AddProperty("CarAhead01IRating", 0);
            s.AddProperty("CarAhead01Licence", "");
            s.AddProperty("CarAhead01IsAhead", false);
            s.AddProperty("CarAhead01IsClassLeader", false);
            s.AddProperty("CarAhead01IsInPit", false);
            s.AddProperty("CarAhead01ClassColor", "");
            s.AddProperty("CarAhead01ClassDifference", 0);
            s.AddProperty("CarAhead01JokerLaps", 0);
            s.AddProperty("CarAhead01LapsSincePit", -1);
            s.AddProperty("CarAhead01P2PCount", -1);
            s.AddProperty("CarAhead01P2PStatus", false);
            s.AddProperty("CarAhead01RealGap", 0);
            s.AddProperty("CarAhead01RealRelative", 0);

            s.AddProperty("CarAhead02Gap", 0);
            s.AddProperty("CarAhead02RaceGap", 0);
            s.AddProperty("CarAhead02BestLap", new TimeSpan(0));
            s.AddProperty("CarAhead02Name", "");
            s.AddProperty("CarAhead02Position", 0);
            s.AddProperty("CarAhead02IRating", 0);
            s.AddProperty("CarAhead02Licence", "");
            s.AddProperty("CarAhead02IsAhead", false);
            s.AddProperty("CarAhead02IsClassLeader", false);
            s.AddProperty("CarAhead02IsInPit", false);
            s.AddProperty("CarAhead02ClassColor", "");
            s.AddProperty("CarAhead02ClassDifference", 0);
            s.AddProperty("CarAhead02JokerLaps", 0);
            s.AddProperty("CarAhead02LapsSincePit", -1);
            s.AddProperty("CarAhead02P2PCount", -1);
            s.AddProperty("CarAhead02P2PStatus", false);
            s.AddProperty("CarAhead02RealGap", 0);
            s.AddProperty("CarAhead02RealRelative", 0);

            s.AddProperty("CarAhead03Gap", 0);
            s.AddProperty("CarAhead03RaceGap", 0);
            s.AddProperty("CarAhead03BestLap", new TimeSpan(0));
            s.AddProperty("CarAhead03Name", "");
            s.AddProperty("CarAhead03Position", 0);
            s.AddProperty("CarAhead03IRating", 0);
            s.AddProperty("CarAhead03Licence", "");
            s.AddProperty("CarAhead03IsAhead", false);
            s.AddProperty("CarAhead03IsClassLeader", false);
            s.AddProperty("CarAhead03IsInPit", false);
            s.AddProperty("CarAhead03ClassColor", "");
            s.AddProperty("CarAhead03ClassDifference", 0);
            s.AddProperty("CarAhead03JokerLaps", 0);
            s.AddProperty("CarAhead03LapsSincePit", -1);
            s.AddProperty("CarAhead03P2PCount", -1);
            s.AddProperty("CarAhead03P2PStatus", false);
            s.AddProperty("CarAhead03RealGap", 0);
            s.AddProperty("CarAhead03RealRelative", 0);

            s.AddProperty("CarAhead04Gap", 0);
            s.AddProperty("CarAhead04RaceGap", 0);
            s.AddProperty("CarAhead04BestLap", new TimeSpan(0));
            s.AddProperty("CarAhead04Name", "");
            s.AddProperty("CarAhead04Position", 0);
            s.AddProperty("CarAhead04IRating", 0);
            s.AddProperty("CarAhead04Licence", "");
            s.AddProperty("CarAhead04IsAhead", false);
            s.AddProperty("CarAhead04IsClassLeader", false);
            s.AddProperty("CarAhead04IsInPit", false);
            s.AddProperty("CarAhead04ClassColor", "");
            s.AddProperty("CarAhead04ClassDifference", 0);
            s.AddProperty("CarAhead04JokerLaps", 0);
            s.AddProperty("CarAhead04LapsSincePit", -1);
            s.AddProperty("CarAhead04P2PCount", -1);
            s.AddProperty("CarAhead04P2PStatus", false);
            s.AddProperty("CarAhead04RealGap", 0);
            s.AddProperty("CarAhead04RealRelative", 0);

            s.AddProperty("CarAhead05Gap", 0);
            s.AddProperty("CarAhead05RaceGap", 0);
            s.AddProperty("CarAhead05BestLap", new TimeSpan(0));
            s.AddProperty("CarAhead05Name", "");
            s.AddProperty("CarAhead05Position", 0);
            s.AddProperty("CarAhead05IRating", 0);
            s.AddProperty("CarAhead05Licence", "");
            s.AddProperty("CarAhead05IsAhead", false);
            s.AddProperty("CarAhead05IsClassLeader", false);
            s.AddProperty("CarAhead05IsInPit", false);
            s.AddProperty("CarAhead05ClassColor", "");
            s.AddProperty("CarAhead05ClassDifference", 0);
            s.AddProperty("CarAhead05JokerLaps", 0);
            s.AddProperty("CarAhead05LapsSincePit", -1);
            s.AddProperty("CarAhead05P2PCount", -1);
            s.AddProperty("CarAhead05P2PStatus", false);
            s.AddProperty("CarAhead05RealGap", 0);
            s.AddProperty("CarAhead05RealRelative", 0);

            s.AddProperty("CarBehind01Gap", 0);
            s.AddProperty("CarBehind01RaceGap", 0);
            s.AddProperty("CarBehind01BestLap", new TimeSpan(0));
            s.AddProperty("CarBehind01Name", "");
            s.AddProperty("CarBehind01Position", 0);
            s.AddProperty("CarBehind01IRating", 0);
            s.AddProperty("CarBehind01Licence", "");
            s.AddProperty("CarBehind01IsAhead", false);
            s.AddProperty("CarBehind01IsClassLeader", false);
            s.AddProperty("CarBehind01IsInPit", false);
            s.AddProperty("CarBehind01ClassColor", "");
            s.AddProperty("CarBehind01ClassDifference", 0);
            s.AddProperty("CarBehind01JokerLaps", 0);
            s.AddProperty("CarBehind01LapsSincePit", -1);
            s.AddProperty("CarBehind01P2PCount", -1);
            s.AddProperty("CarBehind01P2PStatus", false);
            s.AddProperty("CarBehind01RealGap", 0);
            s.AddProperty("CarBehind01RealRelative", 0);

            s.AddProperty("CarBehind02Gap", 0);
            s.AddProperty("CarBehind02RaceGap", 0);
            s.AddProperty("CarBehind02BestLap", new TimeSpan(0));
            s.AddProperty("CarBehind02Name", "");
            s.AddProperty("CarBehind02Position", 0);
            s.AddProperty("CarBehind02IRating", 0);
            s.AddProperty("CarBehind02Licence", "");
            s.AddProperty("CarBehind02IsAhead", false);
            s.AddProperty("CarBehind02IsClassLeader", false);
            s.AddProperty("CarBehind02IsInPit", false);
            s.AddProperty("CarBehind02ClassColor", "");
            s.AddProperty("CarBehind02ClassDifference", 0);
            s.AddProperty("CarBehind02JokerLaps", 0);
            s.AddProperty("CarBehind02LapsSincePit", -1);
            s.AddProperty("CarBehind02P2PCount", -1);
            s.AddProperty("CarBehind02P2PStatus", false);
            s.AddProperty("CarBehind02RealGap", 0);
            s.AddProperty("CarBehind02RealRelative", 0);

            s.AddProperty("CarBehind03Gap", 0);
            s.AddProperty("CarBehind03RaceGap", 0);
            s.AddProperty("CarBehind03BestLap", new TimeSpan(0));
            s.AddProperty("CarBehind03Name", "");
            s.AddProperty("CarBehind03Position", 0);
            s.AddProperty("CarBehind03IRating", 0);
            s.AddProperty("CarBehind03Licence", "");
            s.AddProperty("CarBehind03IsAhead", false);
            s.AddProperty("CarBehind03IsClassLeader", false);
            s.AddProperty("CarBehind03IsInPit", false);
            s.AddProperty("CarBehind03ClassColor", "");
            s.AddProperty("CarBehind03ClassDifference", 0);
            s.AddProperty("CarBehind03JokerLaps", 0);
            s.AddProperty("CarBehind03LapsSincePit", -1);
            s.AddProperty("CarBehind03P2PCount", -1);
            s.AddProperty("CarBehind03P2PStatus", false);
            s.AddProperty("CarBehind03RealGap", 0);
            s.AddProperty("CarBehind03RealRelative", 0);

            s.AddProperty("CarBehind04Gap", 0);
            s.AddProperty("CarBehind04RaceGap", 0);
            s.AddProperty("CarBehind04BestLap", new TimeSpan(0));
            s.AddProperty("CarBehind04Name", "");
            s.AddProperty("CarBehind04Position", 0);
            s.AddProperty("CarBehind04IRating", 0);
            s.AddProperty("CarBehind04Licence", "");
            s.AddProperty("CarBehind04IsAhead", false);
            s.AddProperty("CarBehind04IsClassLeader", false);
            s.AddProperty("CarBehind04IsInPit", false);
            s.AddProperty("CarBehind04ClassColor", "");
            s.AddProperty("CarBehind04ClassDifference", 0);
            s.AddProperty("CarBehind04JokerLaps", 0);
            s.AddProperty("CarBehind04LapsSincePit", -1);
            s.AddProperty("CarBehind04P2PCount", -1);
            s.AddProperty("CarBehind04P2PStatus", false);
            s.AddProperty("CarBehind04RealGap", 0);
            s.AddProperty("CarBehind04RealRelative", 0);

            s.AddProperty("CarBehind05Gap", 0);
            s.AddProperty("CarBehind05RaceGap", 0);
            s.AddProperty("CarBehind05BestLap", new TimeSpan(0));
            s.AddProperty("CarBehind05Name", "");
            s.AddProperty("CarBehind05Position", 0);
            s.AddProperty("CarBehind05IRating", 0);
            s.AddProperty("CarBehind05Licence", "");
            s.AddProperty("CarBehind05IsAhead", false);
            s.AddProperty("CarBehind05IsClassLeader", false);
            s.AddProperty("CarBehind05IsInPit", false);
            s.AddProperty("CarBehind05ClassColor", "");
            s.AddProperty("CarBehind05ClassDifference", 0);
            s.AddProperty("CarBehind05JokerLaps", 0);
            s.AddProperty("CarBehind05LapsSincePit", -1);
            s.AddProperty("CarBehind05P2PCount", -1);
            s.AddProperty("CarBehind05P2PStatus", false);
            s.AddProperty("CarBehind05RealGap", 0);
            s.AddProperty("CarBehind05RealRelative", 0);

            s.AddProperty("FuelDelta", 0);
            s.AddProperty("FuelPitWindowFirst", 0);
            s.AddProperty("FuelPitWindowLast", 0);
            s.AddProperty("FuelMinimumFuelFill", 0);
            s.AddProperty("FuelMaximumFuelFill", 0);
            s.AddProperty("FuelPitStops", 0);
            s.AddProperty("FuelConserveToSaveAStop", 0);
            s.AddProperty("FuelAlert", false);

            s.AddProperty("FuelDeltaLL", 0);
            s.AddProperty("FuelPitWindowFirstLL", 0);
            s.AddProperty("FuelPitWindowLastLL", 0);
            s.AddProperty("FuelMinimumFuelFillLL", 0);
            s.AddProperty("FuelMaximumFuelFillLL", 0);
            s.AddProperty("FuelPitStopsLL", 0);
            s.AddProperty("FuelConserveToSaveAStopLL", 0);

            s.AddProperty("FuelSlowestFuelSavePace", new TimeSpan(0));
            s.AddProperty("FuelSaveDeltaValue", 0);
            s.AddProperty("FuelPerLapOffset", 0);
            s.AddProperty("FuelPerLapTarget", 0);
            s.AddProperty("FuelPerLapTargetLastLapDelta", 0);
            s.AddProperty("FuelTargetDeltaCumulative", 0);

            s.AddProperty("TrackType", 0);
            s.AddProperty("JokerThisLap", false);
            s.AddProperty("JokerCount", 0);

            s.AddProperty("MinimumCornerSpeed", 0);
            s.AddProperty("StraightLineSpeed", 0);

            s.AddProperty("PitToggleLF", false);
            s.AddProperty("PitToggleRF", false);
            s.AddProperty("PitToggleLR", false);
            s.AddProperty("PitToggleRR", false);
            s.AddProperty("PitToggleFuel", false);
            s.AddProperty("PitToggleWindscreen", false);
            s.AddProperty("PitToggleRepair", false);

            s.AddProperty("PitServiceFuelTarget", 0);
            s.AddProperty("PitServiceLFPSet", 0);
            s.AddProperty("PitServiceRFPSet", 0);
            s.AddProperty("PitServiceLRPSet", 0);
            s.AddProperty("PitServiceRRPSet", 0);
            s.AddProperty("PitServiceLFPCold", 0);
            s.AddProperty("PitServiceRFPCold", 0);
            s.AddProperty("PitServiceLRPCold", 0);
            s.AddProperty("PitServiceRRPCold", 0);

            s.AddProperty("CurrentFrontWing", 0);
            s.AddProperty("CurrentRearWing", 0);
            s.AddProperty("CurrentPowersteer", 0);
            s.AddProperty("CurrentTape", 0);

            s.AddProperty("PitCrewType", 0);
            s.AddProperty("PitTimeTires", 0);
            s.AddProperty("PitTimeFuel", 0);
            s.AddProperty("PitTimeWindscreen", 0);
            s.AddProperty("PitTimeAdjustment", 0);
            s.AddProperty("PitTimeDriveThrough", 0);
            s.AddProperty("PitTimeService", 0);
            s.AddProperty("PitTimeTotal", 0);


            s.AddProperty("PitExitPosition", 0);

            s.AddProperty("PitExitCar1Name", "");
            s.AddProperty("PitExitCar1Gap", 0);
            s.AddProperty("PitExitCar1Position", 0);
            s.AddProperty("PitExitCar1ClassDifference", 0);
            s.AddProperty("PitExitCar1IsAhead", false);
            s.AddProperty("PitExitCar1IsFaster", false);

            s.AddProperty("PitExitCar2Name", "");
            s.AddProperty("PitExitCar2Gap", 0);
            s.AddProperty("PitExitCar2Position", 0);
            s.AddProperty("PitExitCar2ClassDifference", 0);
            s.AddProperty("PitExitCar2IsAhead", false);
            s.AddProperty("PitExitCar2IsFaster", false);

            s.AddProperty("PitExitCar3Name", "");
            s.AddProperty("PitExitCar3Gap", 0);
            s.AddProperty("PitExitCar3Position", 0);
            s.AddProperty("PitExitCar3ClassDifference", 0);
            s.AddProperty("PitExitCar3IsAhead", false);
            s.AddProperty("PitExitCar3IsFaster", false);

            s.AddProperty("PitExitCar4Name", "");
            s.AddProperty("PitExitCar4Gap", 0);
            s.AddProperty("PitExitCar4Position", 0);
            s.AddProperty("PitExitCar4ClassDifference", 0);
            s.AddProperty("PitExitCar4IsAhead", false);
            s.AddProperty("PitExitCar4IsFaster", false);

            s.AddProperty("PitExitCar5Name", "");
            s.AddProperty("PitExitCar5Gap", 0);
            s.AddProperty("PitExitCar5Position", 0);
            s.AddProperty("PitExitCar5ClassDifference", 0);
            s.AddProperty("PitExitCar5IsAhead", false);
            s.AddProperty("PitExitCar5IsFaster", false);

            s.AddProperty("PitExitCar6Name", "");
            s.AddProperty("PitExitCar6Gap", 0);
            s.AddProperty("PitExitCar6Position", 0);
            s.AddProperty("PitExitCar6ClassDifference", 0);
            s.AddProperty("PitExitCar6IsAhead", false);
            s.AddProperty("PitExitCar6IsFaster", false);

            s.AddProperty("PitExitCar7Name", "");
            s.AddProperty("PitExitCar7Gap", 0);
            s.AddProperty("PitExitCar7Position", 0);
            s.AddProperty("PitExitCar7ClassDifference", 0);
            s.AddProperty("PitExitCar7IsAhead", false);
            s.AddProperty("PitExitCar7IsFaster", false);

            s.AddProperty("PitExitCar8Name", "");
            s.AddProperty("PitExitCar8Gap", 0);
            s.AddProperty("PitExitCar8Position", 0);
            s.AddProperty("PitExitCar8ClassDifference", 0);
            s.AddProperty("PitExitCar8IsAhead", false);
            s.AddProperty("PitExitCar8IsFaster", false);

            s.AddProperty("PitExitCar9Name", "");
            s.AddProperty("PitExitCar9Gap", 0);
            s.AddProperty("PitExitCar9Position", 0);
            s.AddProperty("PitExitCar9ClassDifference", 0);
            s.AddProperty("PitExitCar9IsAhead", false);
            s.AddProperty("PitExitCar9IsFaster", false);

            s.AddProperty("PitExitCar10Name", "");
            s.AddProperty("PitExitCar10Gap", 0);
            s.AddProperty("PitExitCar10Position", 0);
            s.AddProperty("PitExitCar10ClassDifference", 0);
            s.AddProperty("PitExitCar10IsAhead", false);
            s.AddProperty("PitExitCar10IsFaster", false);

            s.AddProperty("PitExitCar11Name", "");
            s.AddProperty("PitExitCar11Gap", 0);
            s.AddProperty("PitExitCar11Position", 0);
            s.AddProperty("PitExitCar11ClassDifference", 0);
            s.AddProperty("PitExitCar11IsAhead", false);
            s.AddProperty("PitExitCar11IsFaster", false);

            s.AddProperty("PitExitCar12Name", "");
            s.AddProperty("PitExitCar12Gap", 0);
            s.AddProperty("PitExitCar12Position", 0);
            s.AddProperty("PitExitCar12ClassDifference", 0);
            s.AddProperty("PitExitCar12IsAhead", false);
            s.AddProperty("PitExitCar12IsFaster", false);

            s.AddProperty("PitExitCar13Name", "");
            s.AddProperty("PitExitCar13Gap", 0);
            s.AddProperty("PitExitCar13Position", 0);
            s.AddProperty("PitExitCar13ClassDifference", 0);
            s.AddProperty("PitExitCar13IsAhead", false);
            s.AddProperty("PitExitCar13IsFaster", false);

            s.AddProperty("PitExitCar14Name", "");
            s.AddProperty("PitExitCar14Gap", 0);
            s.AddProperty("PitExitCar14Position", 0);
            s.AddProperty("PitExitCar14ClassDifference", 0);
            s.AddProperty("PitExitCar14IsAhead", false);
            s.AddProperty("PitExitCar14IsFaster", false);

            s.AddProperty("DDCclutch", 0);
            s.AddProperty("DDCbitePoint", 0);
            s.AddProperty("DDCbrake", 0);
            s.AddProperty("DDCthrottle", 0);

            s.AddProperty("DDCR1", -1);
            s.AddProperty("DDCR2", -1);
            s.AddProperty("DDCR3", -1);
            s.AddProperty("DDCR4", -1);
            s.AddProperty("DDCR5", -1);
            s.AddProperty("DDCR6", -1);
            s.AddProperty("DDCR7", -1);
            s.AddProperty("DDCR8", -1);
            s.AddProperty("DDCR15", -1);
            s.AddProperty("DDCDDSMode", -1);
            s.AddProperty("DDCDDSEnabled", false);
            s.AddProperty("DDCEnabled", false);
            s.AddProperty("DDCclutchEnabled", false);
            s.AddProperty("DDCclutchMode", -1);
            s.AddProperty("DDCbiteSetting", -1);


            s.AddProperty("DDCB1", -1);
            s.AddProperty("DDCB2", -1);
            s.AddProperty("DDCB3", -1);
            s.AddProperty("DDCB4", -1);

            s.AddProperty("DDCthrottleHoldActive", -1);
            s.AddProperty("DDCmagicActive", -1);
            s.AddProperty("DDCquickSwitchMode", -1);
            s.AddProperty("DDCquickSwitchActive", -1);
            s.AddProperty("DDChandbrakeActive", -1);

            s.AddProperty("DDCneutralMode", -1);
            s.AddProperty("DDCneutralActive", false);
            s.AddProperty("DDCPreset", -1);

            s.AddProperty("SW1DDSMode", -1);
            s.AddProperty("SW1ClutchMode", -1);
            s.AddProperty("SW1BiteSetting", -1);

            s.AddProperty("SW1QuickSwitchMode", -1);
            s.AddProperty("SW1HandbrakeActive", -1);
            s.AddProperty("SW1RadioButtonMode", -1);
            s.AddProperty("SW1RightRotaryMode", -1);
            s.AddProperty("SW1LeftRotaryMode", -1);
            s.AddProperty("SW1MagicToggleMode", -1);
            s.AddProperty("SW1RightToggleMode", -1);
            s.AddProperty("SW1LeftToggleMode", -1);
            s.AddProperty("SW1ShifterMode", -1);
            s.AddProperty("SW1QuickSwitchActive", false);
            s.AddProperty("SW1ThrottleHoldActive", false);
            s.AddProperty("SW1MagicToggleActive", false);
            s.AddProperty("SW1Preset", -1);
            s.AddProperty("SW1NeutralMode", -1);
            s.AddProperty("SW1NeutralActive", false);

            s.AddProperty("SW1Clutch", 0);
            s.AddProperty("SW1BitePoint", 0);
            s.AddProperty("SW1Brake", 0);
            s.AddProperty("SW1Throttle", 0);

        }
    }
}