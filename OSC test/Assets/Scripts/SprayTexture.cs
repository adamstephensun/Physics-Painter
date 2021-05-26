using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayTexture : MonoBehaviour
{
    private int radius = 128;
    private Color col;
    private Sprite spraySprite;
    private Texture2D sprayTex;
    public string path = "C:/Users/adams/OneDrive/Documents/Projects/Unity Projects/OSC test/Assets/Resources/sprayTex.png";

    private Renderer render;

    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<Renderer>();

        sprayTex = new Texture2D(radius * 2, radius * 2, TextureFormat.RGBA32, false);
        
        for(int i = 0; i < sprayTex.width;i++)
        {
            for(int j = 0; j < sprayTex.height;j++)
            {
                sprayTex.SetPixel(i, j, new Color(0, 0, 0, 0));
            }
        }
        sprayTex.Apply();
        

        col = Color.white;

        int n = 1000;

        float[] randAngle = new float[n];
        float[] randDist = new float[n];
        int[] newX = new int[n];
        int[] newY = new int[n];

        for(int i = 0; i < n; i++)
        {
            randAngle[i] = Random.Range(0, Mathf.PI * 2);
            randDist[i] = Random.Range(0, radius);

            newX[i] = Mathf.RoundToInt(radius + (randDist[i] * Mathf.Cos(randAngle[i])));
            newY[i] = Mathf.RoundToInt(radius + (randDist[i] * Mathf.Sin(randAngle[i])));

            sprayTex.SetPixel(newX[i], newY[i], col);
            sprayTex.SetPixel(newX[i]+1, newY[i], col);
            sprayTex.SetPixel(newX[i], newY[i]+1, col);
            sprayTex.SetPixel(newX[i]+1, newY[i]+1, col);
        }
        sprayTex.Apply();

        saveTexToPng(sprayTex, path);
    }

    private void saveTexToPng(Texture2D tex, string path)
    {
        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        Debug.Log(bytes.Length / 1024 + "kb was saved as:" + path);
    }
}

