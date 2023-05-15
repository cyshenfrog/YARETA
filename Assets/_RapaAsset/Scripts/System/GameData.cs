using UnityEngine;

public static class GameData
{
    private const string ROOT_PATH = "GameData/";
    private const string CONTROLLER_SPRITES_PATH = ROOT_PATH + "ControllerSprites";
    private const string EN_FONT_PATH = ROOT_PATH + "Font_en";
    private const string CHT_FONT_PATH = ROOT_PATH + "Font_cht";

    public static ControllerSprites ControllerSprites;
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
        EnFont = (Font)Resources.Load(EN_FONT_PATH);
        ChtFont = (Font)Resources.Load(CHT_FONT_PATH);
    }
}