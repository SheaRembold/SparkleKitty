using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraUI : MonoBehaviour
{
    public void TakePhoto()
    {
        List<Placable> cats = PlacementManager.Instance.GetPlayArea().GetInArea(PlacableDataType.Cat);
        string catName = cats[cats.Count - 1].Data.name;

        RenderTexture tempRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = tempRT;
        Camera.main.targetTexture = tempRT;
        Camera.main.Render();
        Camera.main.targetTexture = null;
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();
        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(tempRT);

        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(Application.persistentDataPath + "/" + catName + ".png", bytes);
    }
}