using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using UnityEngine;

public class WaterShed
{


    public static Image<Gray, byte> TestWaterShed(string file)
    {
        //Load Image into Mat
        Mat matImage = new Mat(file);

        //Convert Mat Bgr to Gray
        Mat matGray = new Mat(matImage.Rows, matImage.Cols, DepthType.Cv8U, 1);
        CvInvoke.CvtColor(matImage, matGray, ColorConversion.Bgr2Gray);

        //Implements Image Objects from Mats
        Image<Bgr, byte> img = matImage.ToImage<Bgr, byte>();
        Image<Gray, byte> imgGray = matGray.ToImage<Gray, byte>();

        //Binarize imgGray with Otsu's binarization
        Image<Gray, byte> imgBinarize = new Image<Gray, byte>(imgGray.Width, imgGray.Height, new Gray(0));
        CvInvoke.Threshold(imgGray, imgBinarize, 80, 255, ThresholdType.ToZero | ThresholdType.Otsu);

        // Opening to clean the image
        // first define the anchor and then the structuring element
        Point anchor = new Point(-1, -1);
        Mat structuringElement = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(2, 2), anchor);

        //noise removal
        Image<Bgr, byte> opening = new Image<Bgr, byte>(imgBinarize.Width, imgBinarize.Height);
        CvInvoke.MorphologyEx(imgBinarize, opening, MorphOp.Open, structuringElement, new Point(-1, -1), 2, BorderType.Constant, new MCvScalar(0));

        // sure background area
        Image<Bgr, byte> dilate = new Image<Bgr, byte>(opening.Width, opening.Height);
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


        Image<Bgr, byte> sure_fg = new Image<Bgr, byte>(distTransform.Width, distTransform.Height);
        CvInvoke.Threshold(distTransform, sure_fg, 0.2 * maxVal, 255, ThresholdType.Binary);

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
        Mat markers = Mat.Zeros(distTransform_8u.Rows, distTransform_8u.Cols, DepthType.Cv8S, 3);

        matSure_fg.ConvertTo(matSure_fg, DepthType.Cv8U);


        // Get labels of regions
        int nLabels = CvInvoke.ConnectedComponents(matSure_fg, markers);
        Debug.Log("nLabels : " +nLabels);
        

        markers += 1;

        Image<Gray, byte> imgUnknown = matUnknown.ToImage<Gray, byte>();

        // Initialise unknown regions datas to zero
        Image<Gray, byte> imgMarkers = markers.ToImage<Gray, byte>();
        for (int i = 0; i < markers.Rows; i++)
        {
            for (int j = 0; j < markers.Cols; j++)
            {
                if(imgUnknown.Data[i,j,0] ==255)
                {
                    imgMarkers.Data[i, j, 0] = 0;
                }
            }
        }


        // Watershed part
        //Debug.Log(img.Mat.Depth + img.NumberOfChannels + " " + imgMarkers.Mat.Depth + imgMarkers.NumberOfChannels);
        matImage.ConvertTo(matImage, DepthType.Cv8U);
        markers = imgMarkers.Mat;
        markers.ConvertTo(markers, DepthType.Cv32S);
        CvInvoke.Watershed(matImage, markers);
        markers.ConvertTo(markers, DepthType.Cv8U);
        imgMarkers = markers.ToImage<Gray, byte>();

        // Show output image
        //CvInvoke.Imshow("Image", img);
        //CvInvoke.Imshow("imgGray", imgGray);
        //CvInvoke.Imshow("imgBinarize", imgBinarize);
        //CvInvoke.Imshow("opening", opening);
        //CvInvoke.Imshow("dilate", dilate);
        //CvInvoke.Imshow("matUnknown",  imgUnknown*10000);
        //CvInvoke.Imshow("distTransform", distTransform);
        //CvInvoke.Imshow("sure_fg", sure_fg);
        //CvInvoke.Imshow("matSure_fg", matSure_fg.ToImage<Bgr, byte>());
        //CvInvoke.Imshow("imgMarkers", imgMarkers*10);
        //CvInvoke.Imshow("markers", imgMarkers*10);

        return imgMarkers;
    }
}