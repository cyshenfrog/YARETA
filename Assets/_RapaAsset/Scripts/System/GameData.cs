using UnityEngine;

[CreateAssetMenu]
public class GameData : SingletonSO<GameData>
{
    public Font EnFont;
    public Font ChtFont;

    public Font Font
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
}