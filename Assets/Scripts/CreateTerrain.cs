using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

public class CreateTerrain : MonoBehaviour
{
    //[SerializeField] TerrainData terrainData = null;
    [SerializeField] string fileName = null;

    private Image<Gray, byte> image;
    private Terrain terrain;
    // Start is called before the first frame update
    void Start()
    {
        
        terrain = Terrain.activeTerrain;

        TerrainData terrainData = terrain.terrainData;

        image = new Image<Gray, byte>(fileName);

        int imageHeight = image.Height;
        int imageWidth = image.Width;
        Debug.Log(imageWidth + " " + imageHeight);
        int terrainResolution = terrainData.heightmapResolution;

        Debug.Log(terrainResolution);

        if (imageHeight < terrainResolution - 1 || imageWidth < terrainResolution - 1)
            Debug.LogError("Image to small");
        else
        {
            terrainData.size = new Vector3(terrainResolution - 1, 100, terrainResolution - 1);

            float[,] data = new float[terrainResolution - 1, terrainResolution - 1];

            //Debug.Log("height = " + data.Length + " width = ");

            for (int y = 0; y < terrainResolution - 1; y++)
            {
                for (int x = 0; x < terrainResolution - 1; x++)
                {
                    data[x, y] = (255 - image.Data[x, y, 0]) / 255f;
                }
            }
            terrainData.SetHeights(0, 0, data);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
