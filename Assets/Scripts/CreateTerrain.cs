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
    [SerializeField] bool inverse = false;

    private Image<Gray, byte> image;
    private Terrain terrain;

    public GameObject[] prefabs;

    // Start is called before the first frame update
    void Start()
    {
        terrain = Terrain.activeTerrain;

        TerrainData terrainData = terrain.terrainData;

        image = new Image<Gray, byte>(fileName);

        int imageHeight = image.Height;
        int imageWidth = image.Width;

        int terrainResolution = terrainData.heightmapResolution;

        if (imageHeight < terrainResolution - 1 || imageWidth < terrainResolution - 1)
            Debug.LogError("Image to small");
        else
        {
            terrainData.size = new Vector3(terrainResolution - 1, 100, terrainResolution - 1);

            float[,] data = new float[terrainResolution - 1, terrainResolution - 1];

            for (int y = 0; y < terrainResolution - 1; y++)
            {
                for (int x = 0; x < terrainResolution - 1; x++)
                {
                    if (!inverse)
                        data[x, y] = image.Data[x, y, 0] / 255f;
                    else
                        data[x, y] = (255 - image.Data[x, y, 0]) / 255f;
                }
            }
            terrainData.SetHeights(0, 0, data);
        }

        Image<Gray, byte> ws = WaterShed.TestWaterShed();
        //CvInvoke.Imshow("markers", ws);

        int k = 0;
        if (ws.Rows >= terrainResolution - 1 && ws.Cols >= terrainResolution - 1)
        {
            for (int i = 0; i < terrainResolution - 1; i += 10)
            {
                for (int j = 0; j < terrainResolution - 1; j += 10)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(new Vector3(i,1000,j), Vector3.down,out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
                    {
                        Instantiate(PickPrefab(ws.Data[i,j,0]), hit.point, Quaternion.identity);
                    }
                    k++;
                }
            }
        }
    }
    private GameObject PickPrefab(int index)
    {
        return prefabs[index % prefabs.Length];
    }
    // Update is called once per frame
    void Update()
    {

    }
}
