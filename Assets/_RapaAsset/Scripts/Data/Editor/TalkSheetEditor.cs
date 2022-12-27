using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
///
[CustomEditor(typeof(TalkSheet))]
public class TalkSheetEditor : BaseExcelEditor<TalkSheet>
{
    public override bool Load()
    {
        TalkSheet targetData = target as TalkSheet;

        string path = targetData.SheetName;
        if (!File.Exists(path))
            return false;

        string sheet = targetData.WorksheetName;

        ExcelQuery query = new ExcelQuery(path, sheet);
        if (query != null && query.IsValid())
        {
            targetData.dataArray = query.Deserialize<TalkSheetData>().ToArray();
            EditorUtility.SetDirty(targetData);
            AssetDatabase.SaveAssets();
            GenerateEnum.Go(EditorTool_ScriptableObject.FindInstance<TalkSheet>().GetNameArray(), "TalkDataEnum");
            return true;
        }
        else
            return false;
    }
}