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
    public GameObject coinPrefab;

    private List<int> seenIndexes;

    // Start is called before the first frame update
    void Start()
    {
        seenIndexes = new List<int>();

        // Get the terrain's data
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

        Image<Gray, byte> ws = WaterShed.TestWaterShed(fileName);
        CvInvoke.Imshow("markers", ws * 10);

        if (ws.Rows >= terrainResolution - 1 && ws.Cols >= terrainResolution - 1)
        {
            for (int i = 0; i < terrainResolution - 1; i += 10)
            {
                for (int j = 0; j < terrainResolution - 1; j += 10)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(new Vector3(i,1000,j), Vector3.down,out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
                    {
                        Vector3 spawnPoint = hit.point + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
                        GameObject go = Instantiate(PickPrefab(ws.Data[i,j,0]), spawnPoint, Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0f,360f),0)));

                        if(!seenIndexes.Contains(ws.Data[i, j, 0]))
                        {
                            seenIndexes.Add(ws.Data[i, j, 0]);
                            Instantiate(coinPrefab, spawnPoint + new Vector3(UnityEngine.Random.Range(-5f, 5f), 5, UnityEngine.Random.Range(-5f, 5f)), Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0f, 360f), 90)));
                        }
                    }
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
