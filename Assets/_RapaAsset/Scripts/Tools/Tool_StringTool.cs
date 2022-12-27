using System.Collections.Generic;
using UnityEngine;

public static class Tool_StringTool
{
    #region 讀取基礎

    //將一組字串解析為一組語言化字串
    public static List<string[]> ParsingLists(List<string> lines)
    {
        List<string> lines_new = new List<string>(lines);//複製對話
        List<string[]> ExportStrs = new List<string[]>();
        for (int i = 0; i < lines_new.Count; i++)
        {
            string[] L = ParsingLanguage(lines_new[i]);//一個對話
            if (L.Length >= 5)
            {
                ExportStrs.Add(L);
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("語言化單行資訊不足");
#endif
            }
        }

        return ExportStrs;
    }

    //語言化轉換
    public static string[] ParsingLanguage(string line)
    {
        //字串間以'\t'間隔，1:繁體 2簡體 3英文 4日文
        string[] ExportStrs = line.Split('\t');
        for (int i = 0; i < ExportStrs.Length; i++)
        {
            //換行轉換
            ExportStrs[i] = CheckLineBreak(ExportStrs[i]);
        }
        return ExportStrs;
    }

    //將'\'替換成'\n'
    public static string CheckLineBreak(string datatext)
    {
        string Str = "";
        if (datatext.Length > 0)
        {
            Str = datatext.Replace('\\', '\n');
        }
        return Str;
    }

    //移除["]
    public static string ConversionLine(string datatext)
    {
        string Str = "";
        if (datatext.Length > 0)
        {
            Str = datatext.Replace("\"", "");
        }
        return Str;
    }

    #endregion 讀取基礎
}