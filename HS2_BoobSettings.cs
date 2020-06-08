using BepInEx;
using BepInEx.Configuration;
using BepInEx.Harmony;
using KKAPI.Chara;
using KKAPI.Maker;

namespace HS2_BoobSettings
{
    [BepInProcess("HoneySelect2")]
    [BepInProcess("StudioNEOV2")]
    [BepInPlugin(GUID, Name, Version)]
    public partial class HS2_BoobSettings : BaseUnityPlugin
    {
        const string GUID = "com.fairbair.hs2_boobsettings";
        const string Name = "HS2 Boob Settings";
        const string Version = "1.0.0";


        const string SECTION_GENERAL = "General";


        public static ConfigEntry<float> SliderMin { get; set; }
        public static ConfigEntry<float> SliderMax { get; set; }

        public void Awake()
        {
            SliderMin = Config.Bind(SECTION_GENERAL, "Slider Min. Value", -100f);
            SliderMax = Config.Bind(SECTION_GENERAL, "Slider Max. Value", 100f);


            MakerAPI.RegisterCustomSubCategories += MakerAPI_RegisterCustomSubCategories;
            MakerAPI.MakerFinishedLoading += MakerAPI_MakerFinishedLoading;

            CharacterApi.RegisterExtraBehaviour<BoobController>(GUID);
            HarmonyWrapper.PatchAll(typeof(HS2_BoobSettings));
        }
    }
}
