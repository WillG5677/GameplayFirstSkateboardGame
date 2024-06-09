using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteSplitEditorTool : EditorWindow
{
    [MenuItem("Tools/SpriteSplit")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SpriteSplitEditorTool));
    }

    private Sprite editingSprite;
    private Texture2D noiseTexture;
    private List<HashSet<Vector2>> regions;

    private void OnGUI()
    {
        //EditorGUI.BeginChangeCheck();
        editingSprite = EditorGUILayout.ObjectField("Editing Sprite", editingSprite, typeof(Sprite), false) as Sprite;
        //if (EditorGUI.EndChangeCheck())
        //{
        //    if (editingSprite != null)
        //    {
        //        Debug.Log(editingSprite.rect);
        //    }
        //}

        EditorGUI.BeginChangeCheck();
        noiseTexture = EditorGUILayout.ObjectField("Split Texture", noiseTexture, typeof(Texture2D), false) as Texture2D;
        if (EditorGUI.EndChangeCheck())
        {
            if (noiseTexture != null)
            {
                regions = GatherRegions(noiseTexture);
            }
        }

        GUI.enabled = editingSprite != null && noiseTexture != null;
        if (GUILayout.Button("Generate split sprites"))
        {
            GenerateSplitSprites();
        }
    }

    private List<HashSet<Vector2>> GatherRegions(Texture2D noiseTexture)
    {
        List<HashSet<Vector2>> gatheredRegions = new List<HashSet<Vector2>>();

        Color[] noisePixels = noiseTexture.GetPixels();
        bool[] visitedPixels = new bool[noiseTexture.width * noiseTexture.height];
        for (int y = 0; y < noiseTexture.height; y++)
        {
            for (int x = 0; x < noiseTexture.width; x++)
            {
                if (noisePixels[y * noiseTexture.width + x] != Color.black)
                {
                    visitedPixels[y * noiseTexture.width + x] = true;
                    continue;
                }
                if (visitedPixels[y * noiseTexture.width + x])
                {
                    continue;
                }

                HashSet<Vector2> region = new HashSet<Vector2>();
                visitedPixels[y * noiseTexture.width + x] = true;
                FloodFill(x, y, noisePixels, noiseTexture, visitedPixels, region);
                gatheredRegions.Add(region);
            }
        }

        Debug.Log(gatheredRegions.Count);
        return gatheredRegions;
    }

    private void FloodFill(int x, int y, Color[] noisePixels, Texture2D noiseTexture, bool[] visitedPixels, HashSet<Vector2> regionPoints)
    {
        Queue<Vector2> queue = new Queue<Vector2>();
        queue.Enqueue(new Vector2(x, y));
        while (queue.Count > 0)
        {
            Vector2 examinePixel = queue.Dequeue();
            int eX = (int)examinePixel.x;
            int eY = (int)examinePixel.y;
            regionPoints.Add(examinePixel);

            if (eX - 1 >= 0 &&
                noisePixels[eY * noiseTexture.width + (eX - 1)] == Color.black &&
                !visitedPixels[eY * noiseTexture.width + (eX - 1)])
            {
                visitedPixels[eY * noiseTexture.width + (eX - 1)] = true;
                queue.Enqueue(new Vector2(eX - 1, eY));
            }
            if (eX - 1 >= 0 &&
                eY + 1 < noiseTexture.height &&
                noisePixels[(eY + 1) * noiseTexture.width + (eX - 1)] == Color.black &&
                !visitedPixels[(eY + 1) * noiseTexture.width + (eX - 1)])
            {
                visitedPixels[(eY + 1) * noiseTexture.width + (eX - 1)] = true;
                queue.Enqueue(new Vector2(eX - 1, eY + 1));
            }
            if (eY + 1 < noiseTexture.height &&
                noisePixels[(eY + 1) * noiseTexture.width + eX] == Color.black &&
                !visitedPixels[(eY + 1) * noiseTexture.width + eX])
            {
                visitedPixels[(eY + 1) * noiseTexture.width + eX] = true;
                queue.Enqueue(new Vector2(eX, eY + 1));
            }
            if (eX + 1 < noiseTexture.width &&
                eY + 1 < noiseTexture.height &&
                noisePixels[(eY + 1) * noiseTexture.width + (eX + 1)] == Color.black &&
                !visitedPixels[(eY + 1) * noiseTexture.width + (eX + 1)])
            {
                visitedPixels[(eY + 1) * noiseTexture.width + (eX + 1)] = true;
                queue.Enqueue(new Vector2(eX + 1, eY + 1));
            }
            if (eX + 1 < noiseTexture.width &&
                noisePixels[eY * noiseTexture.width + (eX + 1)] == Color.black &&
                !visitedPixels[eY * noiseTexture.width + (eX + 1)])
            {
                visitedPixels[eY * noiseTexture.width + (eX + 1)] = true;
                queue.Enqueue(new Vector2(eX + 1, eY));
            }
            if (eX + 1 < noiseTexture.width && 
                eY - 1 >= 0 &&
                noisePixels[(eY - 1) * noiseTexture.width + (eX + 1)] == Color.black &&
                !visitedPixels[(eY - 1) * noiseTexture.width + (eX + 1)])
            {
                visitedPixels[(eY - 1) * noiseTexture.width + (eX + 1)] = true;
                queue.Enqueue(new Vector2(eX + 1, eY - 1));
            }
            if (eY - 1 >= 0 &&
                noisePixels[(eY - 1) * noiseTexture.width + eX] == Color.black &&
                !visitedPixels[(eY - 1) * noiseTexture.width + eX])
            {
                visitedPixels[(eY - 1) * noiseTexture.width + eX] = true;
                queue.Enqueue(new Vector2(eX, eY - 1));
            }
            if (eX - 1 >= 0 &&
                eY - 1 >= 0 &&
                noisePixels[(eY - 1) * noiseTexture.width + (eX - 1)] == Color.black &&
                !visitedPixels[(eY - 1) * noiseTexture.width + (eX - 1)])
            {
                visitedPixels[(eY - 1) * noiseTexture.width + (eX - 1)] = true;
                queue.Enqueue(new Vector2(eX - 1, eY - 1));
            }
        }
    }

    private void GenerateSplitSprites()
    {
        Texture2D sourceTexture = editingSprite.texture;
        Rect transformRect = editingSprite.rect;

        Texture2D scalingTexture = new Texture2D((int)transformRect.width, (int)transformRect.height);
        Graphics.CopyTexture(sourceTexture, 0, 0, (int)transformRect.x, (int)transformRect.y, (int)transformRect.width, (int)transformRect.height,
                                scalingTexture, 0, 0, 0, 0);
        RenderTexture tempRT = RenderTexture.GetTemporary(noiseTexture.width, noiseTexture.height);
        Graphics.Blit(scalingTexture, tempRT);
        Texture2D transformSourceTexture = new Texture2D(noiseTexture.width, noiseTexture.height);

        transformSourceTexture.ReadPixels(new Rect(0f, 0f, noiseTexture.width, noiseTexture.height), 0, 0);
        transformSourceTexture.Apply();
        RenderTexture.ReleaseTemporary(tempRT);
        Color[] transformSourcePixels = transformSourceTexture.GetPixels();

        List<Texture2D> splitTextures = new List<Texture2D>();
        foreach (HashSet<Vector2> regionCoords in regions)
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;
            Color[] splitTexturePixels = new Color[noiseTexture.width * noiseTexture.height];
            for (int i = 0; i < noiseTexture.width * noiseTexture.height; i++)
            {
                splitTexturePixels[i] = new Color(0, 0, 0, 0);
            }

            foreach (Vector2 coord in regionCoords)
            {
                if (coord.x < minX)
                {
                    minX = (int)coord.x;
                }
                if (coord.y < minY)
                {
                    minY = (int)coord.y;
                }
                if (coord.x > maxX)
                {
                    maxX = (int)coord.x;
                }
                if (coord.y > maxY)
                {
                    maxY = (int)coord.y;
                }

                splitTexturePixels[(int)(coord.y * noiseTexture.width + coord.x)] =
                    transformSourcePixels[(int)(coord.y * noiseTexture.width + coord.x)];
            }

            Texture2D splitTexture = new Texture2D(noiseTexture.width, noiseTexture.height);
            splitTexture.SetPixels(splitTexturePixels);
            splitTexture.Apply();
            splitTextures.Add(splitTexture);
        }

        string path = EditorUtility.SaveFilePanel("Save texture as PNG", string.Empty, $"{editingSprite.name}Split.png", "png");
        if (!string.IsNullOrWhiteSpace(path))
        {
            for (int t = 0; t < splitTextures.Count; t++)
            {
                string filePath = path.Replace(".png", $"{t}.png");
                File.WriteAllBytes(filePath, splitTextures[t].EncodeToPNG());
            }
        }

        DestroyImmediate(scalingTexture);
        DestroyImmediate(transformSourceTexture);
    }
}
