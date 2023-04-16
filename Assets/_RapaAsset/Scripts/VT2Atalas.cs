using System.Collections;
using System.IO;
using UnityEngine;

public class VT2Atalas : MonoBehaviour
{
    public RenderTexture RT;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        yield return new WaitForEndOfFrame();
        //RenderTexture.active = RT;
        //Texture2D texture = new Texture2D(1024, 1024, TextureFormat.RGB24, false);
        //texture.ReadPixels(new Rect(0, 0, RT.width, RT.height), 0, 0);
        //texture.Apply();
        byte[] bytes = GetRTPixels(RT).EncodeToPNG();
        var dirPath = Application.dataPath + "/SaveImages/";
        print(dirPath);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Image" + ".png", bytes);
    }

    public static Texture2D GetRTPixels(RenderTexture rt)
    {
        // Remember currently active render texture
        RenderTexture currentActiveRT = RenderTexture.active;

        // Set the supplied RenderTexture as the active one
        RenderTexture.active = rt;

        // Create a new Texture2D and read the RenderTexture image into it
        Texture2D tex = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

        // Restore previously active render texture
        RenderTexture.active = currentActiveRT;
        return tex;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}