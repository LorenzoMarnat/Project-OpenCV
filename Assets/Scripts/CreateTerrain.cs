using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

public class CreateTerrain : MonoBehaviour
{
    [SerializeField] TerrainData terrain = null;
    [SerializeField] string fileName = null;

    private Image<Gray, byte> image;
    // Start is called before the first frame update
    void Start()
    {
        image = new Image<Gray, byte>(fileName);

        int imageHeight = image.Height;
        int imageWidth = image.Width;
        int terrainResolution = terrain.heightmapResolution;

        //terrain.size = new Vector3(imageHeight, 600, imageHeight);

        float[,] data = new float[terrainResolution, terrainResolution];
        Debug.Log("height = " + terrain.heightmapResolution + " width = "  );

        for (int y = 0; y < terrainResolution; y++)
        {
            for (int x = 0; x < terrainResolution; x++)
            {

                    data[x, y] = image.Data[x, y, 0];

                    //Debug.Log("x = " + x + " y = " + y);
            }
        }
        Debug.Log("End");
        terrain.SetHeights(0, 0, data);
        

        CvInvoke.Imshow("flux webcam", image);
        CvInvoke.WaitKey();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
