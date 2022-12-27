#if UNITY_EDITOR

using UnityEditor;
using System.IO;

public class GenerateEnum
{
    [MenuItem("Tools/GenerateScanDataEnum")]
    public static void Go(string[] enumEntries, string enumName)
    {
        string filePathAndName = "Assets/_RapaAsset/Scripts/Enums/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

        using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
        {
            streamWriter.WriteLine("public enum " + enumName);
            streamWriter.WriteLine("{");
            for (int i = 0; i < enumEntries.Length; i++)
            {
                if (!string.IsNullOrEmpty(enumEntries[i]))
                    streamWriter.WriteLine("\t" + RemoveSpecialChar(enumEntries[i]) + ",");
            }
            streamWriter.WriteLine("}");
        }
        string RemoveSpecialChar(string str)
        {
            string pattern = "[\\[\\]\\^\\ -*×―(^)$%~!@#$…&%￥—+=<>《》!！??？:：•'`·、。，；,;\"‘’“”-]";

            str = System.Text.RegularExpressions.Regex.Replace(str, pattern, "");
            return str;
        }
        AssetDatabase.Refresh();
    }
}

#endif