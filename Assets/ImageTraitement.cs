using Emgu.CV;
using Emgu.CV.Structure;
using System;
using UnityEngine;

public class ImageTraitement : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        Mat matLama = new Mat(".\\Assets\\Images\\Jaguar.jpg");

        Image<Bgr, byte> imgLama = matLama.ToImage<Bgr, byte>();

        CvInvoke.Imshow("Lama", imgLama);

        Image<Bgr, byte> imgLamaTresholded = new Image<Bgr, byte>(imgLama.Width, imgLama.Height);
        thresholdWhite(imgLama, ref imgLamaTresholded, 200);

        CvInvoke.Imshow("ImageThresholded", imgLamaTresholded);

        Image<Bgr, byte> imgLamaWhitetoBlack = new Image<Bgr, byte>(imgLama.Width, imgLama.Height);
        whiteToBlack(imgLama, ref imgLamaWhitetoBlack, 200);

        // Show output image
        CvInvoke.Imshow("Black Background Image", imgLamaWhitetoBlack);
    }

    private void thresholdWhite(Image<Bgr, byte> imgInput, ref Image<Bgr, byte> imgOutput, float minValue)
    {
        for (int i = 0; i < imgInput.Rows; i++)
        {
            for (int j = 0; j < imgInput.Cols; j++)
            {
                Vector3 pixelRGB = new Vector3(imgInput.Data[i, j, 0], imgInput.Data[i, j, 1], imgInput.Data[i, j, 2]);
                if (pixelRGB.x >= minValue && pixelRGB.y >= minValue && pixelRGB.z >= minValue)
                {
                    imgOutput.Data[i, j, 0] = 255;
                    imgOutput.Data[i, j, 1] = 255;
                    imgOutput.Data[i, j, 2] = 255;
                }
                else
                {
                    imgOutput.Data[i, j, 0] = Convert.ToByte(pixelRGB.x);
                    imgOutput.Data[i, j, 1] = Convert.ToByte(pixelRGB.y);
                    imgOutput.Data[i, j, 2] = Convert.ToByte(pixelRGB.z);
                }
            }
        }
    }

    private void whiteToBlack(Image<Bgr, byte> imgInput, ref Image<Bgr, byte> imgOutput, float minValue)
    {
        Image<Bgr, byte> imgTresholded = new Image<Bgr, byte>(imgInput.Width, imgInput.Height);
        thresholdWhite(imgInput, ref imgTresholded, minValue);
        // Change the background from white to black, since that will help later to extract
        // better results during the use of Distance Transform
        for (int i = 0; i < imgTresholded.Rows; i++)
        {
            for (int j = 0; j < imgTresholded.Cols; j++)
            {
                Vector3 pixelRGB = new Vector3(imgTresholded.Data[i, j, 0], imgTresholded.Data[i, j, 1], imgTresholded.Data[i, j, 2]);
                if (pixelRGB == new Vector3(255, 255, 255))
                {
                    imgOutput.Data[i, j, 0] = 0;
                    imgOutput.Data[i, j, 1] = 0;
                    imgOutput.Data[i, j, 2] = 0;
                }
                else
                {
                    imgOutput.Data[i, j, 0] = Convert.ToByte(pixelRGB.x);
                    imgOutput.Data[i, j, 1] = Convert.ToByte(pixelRGB.y);
                    imgOutput.Data[i, j, 2] = Convert.ToByte(pixelRGB.z);
                }
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}