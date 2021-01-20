using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageTraitement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mat matLama = new Mat("D:\\LYON 2\\M2\\Project-OpenCV\\Assets\\Images\\lama6.jpg");

        Image<Bgr, byte> imgLama = matLama.ToImage<Bgr, byte>();

        CvInvoke.Imshow("Lama", imgLama);
        CvInvoke.WaitKey();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
