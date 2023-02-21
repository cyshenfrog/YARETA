using UnityEngine;

public static class GameData
{
    private const string ROOT_PATH = "GameData/";
    private const string CONTROLLER_SPRITES_PATH = ROOT_PATH + "ControllerSprites";
    private const string TALK_SHEET_PATH = ROOT_PATH + "TalkSheet";
    private const string SCAN_SHEET_PATH = ROOT_PATH + "ScanSheet";
    private const string UI_SHEET_PATH = ROOT_PATH + "UISheet";
    private const string EN_FONT_PATH = ROOT_PATH + "Font_en";
    private const string CHT_FONT_PATH = ROOT_PATH + "Font_cht";

    public static ControllerSprites ControllerSprites;
    public static TalkSheet TalkSheet;
    public static ScanSheet ScanSheet;
    public static UISheet UISheet;
    [SerializeField] private static Font EnFont;
    [SerializeField] private static Font ChtFont;

    public static Font Font
    {
        get
        {
            switch (SaveDataManager.Language)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseTraditional:
                case SystemLanguage.ChineseSimplified:
                    return ChtFont;

                case SystemLanguage.English:
                default:
                    return EnFont;
            }
        }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        ControllerSprites = (ControllerSprites)Resources.Load(CONTROLLER_SPRITES_PATH);
        TalkSheet = (TalkSheet)Resources.Load(TALK_SHEET_PATH);
        ScanSheet = (ScanSheet)Resources.Load(SCAN_SHEET_PATH);
        UISheet = (UISheet)Resources.Load(UI_SHEET_PATH);
        EnFont = (Font)Resources.Load(EN_FONT_PATH);
        ChtFont = (Font)Resources.Load(CHT_FONT_PATH);
    }
}