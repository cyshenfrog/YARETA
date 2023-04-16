using System.Collections.Generic;
using UnityEngine;

public static class Tool
{
    #region StringTools

    //�N�@�զr��ѪR���@�ջy���Ʀr��
    public static List<string[]> ParsingLists(List<string> lines)
    {
        List<string> lines_new = new List<string>(lines);//�ƻs���
        List<string[]> ExportStrs = new List<string[]>();
        for (int i = 0; i < lines_new.Count; i++)
        {
            string[] L = ParsingLanguage(lines_new[i]);//�@�ӹ��
            if (L.Length >= 5)
            {
                ExportStrs.Add(L);
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("�y���Ƴ���T����");
#endif
            }
        }

        return ExportStrs;
    }

    //�y�����ഫ
    public static string[] ParsingLanguage(string line)
    {
        //�r�궡�H'\t'���j�A1:�c�� 2²�� 3�^�� 4���
        string[] ExportStrs = line.Split('\t');
        for (int i = 0; i < ExportStrs.Length; i++)
        {
            //�����ഫ
            ExportStrs[i] = CheckLineBreak(ExportStrs[i]);
        }
        return ExportStrs;
    }

    //�N'\'������'\n'
    public static string CheckLineBreak(string datatext)
    {
        string Str = "";
        if (datatext.Length > 0)
        {
            Str = datatext.Replace('\\', '\n');
        }
        return Str;
    }

    //����["]
    public static string ConversionLine(string datatext)
    {
        string Str = "";
        if (datatext.Length > 0)
        {
            Str = datatext.Replace("\"", "");
        }
        return Str;
    }

    #endregion StringTools

    /// <summary>
    /// Trim value between -180 ~ 180
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float GetTrimmedEular(float value)
    {
        float f = value;
        if (f > 360)
            f %= 360;
        if (f > 180)
            f -= 360;
        else if (f < -180)
            f += 360;

        return f;
    }
}