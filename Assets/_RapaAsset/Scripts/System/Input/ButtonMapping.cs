using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ButtonMapping : ScriptableObject
{
    public List<ButtonMappingData> Mapping = new List<ButtonMappingData>();

    public void Update()
    {
        for (int i = Mapping.Count; i < (int)Actions.EnumLength; i++)
        {
            Mapping.Add(new ButtonMappingData());
        }
        for (int i = 0; i < Mapping.Count; i++)
        {
            Mapping[i].Type = (Actions)i;
            Mapping[i].name = Mapping[i].Type.ToString();
        }
    }

    public bool HasMapping(Actions Action)
    {
        ButtonMappingData mappingData = Mapping[(int)Action];
        if (GameInput.UsingJoystick)
        {
            return mappingData.Axis != Axis.None || mappingData.Button != Buttons.None;
        }
        else
        {
            return mappingData.KB != Keys.None || mappingData.Mouse != MouseClick.None;
        }
    }

    public ButtonMappingData GetMapping(Actions Action)
    {
        return Mapping[(int)Action];
    }
}