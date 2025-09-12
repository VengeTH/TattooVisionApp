using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;
using OpenCvSharp;
using static OpenCvSharp.Unity;



public class SkinScanner : MonoBehaviour
{
    public ARCameraManager cameraManager;

    private Texture2D cameraTexture;

    void OnEnable()
    {
        if (cameraManager != null)
            cameraManager.frameReceived += OnCameraFrameReceived;
    }

    void OnDisable()
    {
        if (cameraManager != null)
            cameraManager.frameReceived -= OnCameraFrameReceived;
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if (!cameraManager.TryAcquireLatestCpuImage(out var image))
            return;

        var conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width, image.height),
            outputFormat = TextureFormat.RGBA32,
            transformation = XRCpuImage.Transformation.MirrorY
        };

        var rawTextureData = new NativeArray<byte>(image.GetConvertedDataSize(conversionParams), Allocator.Temp);
        image.Convert(conversionParams, rawTextureData);
        image.Dispose();

        if (cameraTexture == null || cameraTexture.width != image.width || cameraTexture.height != image.height)
            cameraTexture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);

        cameraTexture.LoadRawTextureData(rawTextureData);
        cameraTexture.Apply();
        rawTextureData.Dispose();

        ProcessFrame(cameraTexture);
    }

    void ProcessFrame(Texture2D frameTexture)
    {
        // Convert Unity Texture2D to OpenCV Mat (RGBA)
        Mat rgbaMat = TextureToMat(frameTexture);

        // Convert from RGBA to BGR
        Mat bgrMat = new Mat();
        Cv2.CvtColor(rgbaMat, bgrMat, ColorConversionCodes.RGBA2BGR);

        // Convert from BGR to YCrCb color space
        Mat ycrcbMat = new Mat();
        Cv2.CvtColor(bgrMat, ycrcbMat, ColorConversionCodes.BGR2YCrCb);

        // Define skin color range in YCrCb
        Scalar lower = new Scalar(0, 133, 77);
        Scalar upper = new Scalar(255, 173, 127);

        // Apply threshold to create a skin mask
        Mat skinMask = new Mat();
        Cv2.InRange(ycrcbMat, lower, upper, skinMask);

        // Optional: Clean up the mask with blur and morphology
        Cv2.GaussianBlur(skinMask, skinMask, new Size(3, 3), 0);
        Cv2.Erode(skinMask, skinMask, new Mat(), iterations: 1);
        Cv2.Dilate(skinMask, skinMask, new Mat(), iterations: 1);

        // Find contours on the skin mask
        Point[][] contours;
        HierarchyIndex[] hierarchy;
        Cv2.FindContours(skinMask, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

        // Draw rectangles around detected skin areas
        foreach (var contour in contours)
        {
            var rect = Cv2.BoundingRect(contour);
            Cv2.Rectangle(rgbaMat, rect, new Scalar(0, 255, 0), 2);
        }

        // OPTIONAL: Convert back to Texture2D and show result on screen
        Texture2D debugTexture = MatToTexture(rgbaMat);
        GetComponent<Renderer>().material.mainTexture = debugTexture;

        // TODO: Add skin pattern detection here
        // Once detected, call tattoo placement
    }
}
