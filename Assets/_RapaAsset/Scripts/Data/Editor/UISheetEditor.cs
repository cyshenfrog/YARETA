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
[CustomEditor(typeof(UISheet))]
public class UISheetEditor : BaseExcelEditor<UISheet>
{
    public override bool Load()
    {
        UISheet targetData = target as UISheet;

        string path = targetData.SheetName;
        if (!File.Exists(path))
            return false;

        string sheet = targetData.WorksheetName;

        ExcelQuery query = new ExcelQuery(path, sheet);
        if (query != null && query.IsValid())
        {
            targetData.dataArray = query.Deserialize<UISheetData>().ToArray();
            EditorUtility.SetDirty(targetData);
            AssetDatabase.SaveAssets();
            GenerateEnum.Go(EditorTool_ScriptableObject.FindInstance<UISheet>().GetNameArray(), "UIDataEnum");
            return true;
        }
        else
            return false;
    }
}