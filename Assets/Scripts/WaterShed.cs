using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using UnityEngine;

public class WaterShed : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        TestWaterShed();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void TestWaterShed()
    {
        //Load Image into Mat
        Mat matImage = new Mat("water_coins.jpg");

        //Convert Mat Bgr to Gray
        Mat matGray = new Mat(matImage.Rows, matImage.Cols, DepthType.Cv8U, 1);
        CvInvoke.CvtColor(matImage, matGray, ColorConversion.Bgr2Gray);

        //Implements Image Objects from Mats
        Image<Bgr, byte> img = matImage.ToImage<Bgr, byte>();
        Image<Gray, byte> imgGray = matGray.ToImage<Gray, byte>();

        //Binarize imgGray with Otsu's binarization
        Image<Gray, byte> imgBinarize = new Image<Gray, byte>(imgGray.Width, imgGray.Height, new Gray(0));
        CvInvoke.Threshold(imgGray, imgBinarize, 50, 255, ThresholdType.BinaryInv | ThresholdType.Otsu);

        // Opening to clean the image
        // first define the anchor and then the structuring element
        Point anchor = new Point(-1, -1);
        Mat structuringElement = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(5, 5), anchor);

        //noise removal
        Image<Gray, byte> opening = new Image<Gray, byte>(imgBinarize.Width, imgBinarize.Height);
        CvInvoke.MorphologyEx(imgBinarize, opening, MorphOp.Open, structuringElement, new Point(-1, -1), 2, BorderType.Constant, new MCvScalar(0));

        // sure background area
        Image<Gray, byte> dilate = new Image<Gray, byte>(opening.Width, opening.Height);
        CvInvoke.Dilate(opening, dilate, structuringElement, new Point(-1, -1), 3, BorderType.Constant, new MCvScalar(0));

        //finding sure foreground area
        Mat labels = new Mat();
        Mat distTransform = new Mat();
        CvInvoke.DistanceTransform(opening, distTransform, labels, DistType.L2, 3);
        CvInvoke.Normalize(distTransform, distTransform, 0, 1.0, NormType.MinMax);

        //Initialize m
        double minVal = 0;
        double maxVal = 0;
        Point minLoc;
        Point maxLoc;
        CvInvoke.MinMaxLoc(distTransform, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

        Image<Gray, byte> sure_fg = new Image<Gray, byte>(distTransform.Width, distTransform.Height);
        CvInvoke.Threshold(distTransform, sure_fg, 0.7 * maxVal, 255, 0);

        // Finding unknown region
        Mat matSure_fg = new Mat(sure_fg.Rows, sure_fg.Cols, DepthType.Cv8U, 3);
        sure_fg.Mat.ConvertTo(matSure_fg, DepthType.Cv8U);
        Mat matUnknown = new Mat();
        CvInvoke.Subtract(dilate.Mat, matSure_fg, matUnknown);

        Mat distTransform_8u = new Mat();
        distTransform.ConvertTo(distTransform_8u, DepthType.Cv8U);

        // Find total markers
        Mat hierarchy = new Mat();
        VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
        CvInvoke.FindContours(distTransform_8u, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);

        //Marker labelling
        Mat markers = Mat.Zeros(distTransform_8u.Rows, distTransform_8u.Cols, DepthType.Cv8S, 1);

        matSure_fg.ConvertTo(matSure_fg, DepthType.Cv8U);

        Debug.Log(matSure_fg.Depth + matSure_fg.NumberOfChannels + " " + markers.Depth + markers.NumberOfChannels);
        int nLabels = CvInvoke.ConnectedComponents(matSure_fg, markers);
        Debug.Log(nLabels);
        markers += 1;

        CvInvoke.Watershed(matImage, markers);

        for (int i = 0; i < markers.Rows; i++)
        {
            for (int j = 0; j < markers.Cols; j++)
            {

            }
        }
        
        Image<Bgr, byte> imgMarkers = markers.ToImage<Bgr, byte>();


        // Show output image
        //CvInvoke.Imshow("Image", img);
        //CvInvoke.Imshow("imgGray", imgGray);
        //CvInvoke.Imshow("imgBinarize", imgBinarize);
        //CvInvoke.Imshow("opening", opening);
        //CvInvoke.Imshow("dilate", dilate);
        CvInvoke.Imshow("matUnknown", matUnknown.ToImage<Bgr,byte>() *10000);
        CvInvoke.Imshow("distTransform", distTransform);
        CvInvoke.Imshow("sure_fg", sure_fg);
        CvInvoke.Imshow("matSure_fg", matSure_fg.ToImage<Bgr, byte>());
        CvInvoke.Imshow("imgMarkers", imgMarkers *1000);
    }
}