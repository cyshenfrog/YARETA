using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Tool_SwtichDayNight : MonoBehaviour
{
    [Button]
    public void Switch()
    {
        isDay = !isDay;
        info = isDay ? "Now is Day" : "Now is Night";
        RenderSettings.skybox = isDay ? SkyDay : SkyNight;
        RenderSettings.ambientSkyColor = isDay ? DayAmbient : NightAmbient;
        foreach (var item in DayGroup)
        {
            if (item)
                item.SetActive(isDay);
        }
        foreach (var item in NightGroup)
        {
            if (item)
                item.SetActive(!isDay);
        }
        NightGroup[3].SetActive(false);
        SkyNight.SetFloat("Vector1_166C34A9", 0);
        Mimosa.CopyPropertiesFromMaterial(isDay ? MimosaDay : MimosaNight);
    }

    public void EndArea()
    {
        SkyNight.SetFloat("Vector1_166C34A9", 0.24f);
        NightGroup[3].SetActive(true);
    }

    [SerializeField]
    [ReadOnly]
    private string info = "Now is Day";

    public static bool isDay = true;
    public Material SkyDay;
    public Material SkyNight;
    public Material Mimosa;
    public Material MimosaDay;
    public Material MimosaNight;
    public Color DayAmbient;
    public Color NightAmbient;
    public GameObject[] DayGroup;
    public GameObject[] NightGroup;

    [ExecuteInEditMode]
    private void Awake()
    {
        isDay = DayGroup[0].activeSelf;
    }
}