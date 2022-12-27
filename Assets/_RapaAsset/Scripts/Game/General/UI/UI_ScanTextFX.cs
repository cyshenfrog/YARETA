using System.Collections;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Serialization;

public class UI_ScanTextFX : UnitySingleton_D<UI_ScanTextFX>
{
    public LeanGameObjectPool FxPool;
    private string hint;
    private int level;

    public void ShowTextFX(Obj_Info obj, Vector3 pos)
    {
        StartCoroutine(_ShowTextFX(obj, pos));
    }

    private IEnumerator _ShowTextFX(Obj_Info obj, Vector3 pos)
    {
        obj.OnScanEvent.Invoke();
        hint = GameManager.Instance.ScanSheet.dataArray[obj.ID].Name;
        if (obj.Weird)
            hint += "?";
        level = GameManager.Instance.ScanSheet.dataArray[obj.ID].Level;
        yield return new WaitForSeconds(Random.Range(0, 0.2f));
        //FxPool.Spawn(Vector3.left * -9999, Quaternion.identity, FxRoot, true).GetComponentInChildren<ObjTextFx>().Init(hint, level, obj.BigObj ? Player.Instance.transform.position + Player.Instance.transform.forward : obj.transform.position);
        FxPool.Spawn(Vector3.left * -9999, Quaternion.identity).GetComponentInChildren<ObjTextFx>().Init(hint, level, pos);

        yield return new WaitForSeconds(0.5f);
    }
}