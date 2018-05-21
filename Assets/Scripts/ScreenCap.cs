using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCap : MonoBehaviour
{
    [SerializeField]
    string savePath = "test.png";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture tempRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
            Camera.main.targetTexture = tempRT;
            RenderTexture.active = tempRT;
            Camera.main.Render();
            Texture2D image = new Texture2D(tempRT.width, tempRT.height, TextureFormat.ARGB32, false);
            image.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
            image.Apply();
            RenderTexture.active = currentRT;
            Camera.main.targetTexture = null;
            System.IO.File.WriteAllBytes(savePath, image.EncodeToPNG());
        }
    }
}