using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Cuda;
using Emgu.CV.WPF;
using System.IO;


namespace ModernUIApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        Capture capture = null;
        bool captureInProgress;
        private Emgu.CV.UI.ImageBox captureImageBox = new Emgu.CV.UI.ImageBox();

        public MainWindow()
        {
            InitializeComponent();
            VideoPanel.Controls.Add(captureImageBox);
            captureImageBox.Dock = DockStyle.Fill;
        // Emgu.CV.UI.ImageBox ImgBox = new ImageBox();
        CvInvoke.UseOpenCL = false;
            try
            {
                capture = new Capture();
                capture.ImageGrabbed += ProcessFrame;
            }
            catch (NullReferenceException excpt)
            {
                System.Windows.MessageBox.Show(excpt.Message);
            }
        }


        private void Button_Click(object sender, EventArgs e)
        {
            if (capture != null)
            {
                if (captureInProgress)
                {  //stop the capture
                   
                    capture.Pause();
                }
                else
                {
                    //start the capture
                  
                    capture.Start();

                }

                captureInProgress = !captureInProgress;
            }
        }

        private void ReleaseData()
        {
            if (capture != null)
                capture.Dispose();
        }

        private void ProcessFrame (object sender, EventArgs e)
        {

            Mat grayimage = new Mat(); //Read the files as an 8-bit Bgr image  
            capture.Retrieve(grayimage, 0);

            long detectionTime;
            List<Rectangle> faces = new List<Rectangle>();
            List<Rectangle> eyes = new List<Rectangle>();

            //The cuda cascade classifier doesn't seem to be able to load "haarcascade_frontalface_default.xml" file in this release
            //disabling CUDA module for now
            bool tryUseCuda = false;

            DetectFace.Detect(
               grayimage, "haarcascade_frontalface_default.xml", "haarcascade_eye.xml",
              faces, eyes,
              tryUseCuda,
              out detectionTime);

            foreach (Rectangle face in faces)
                CvInvoke.Rectangle(grayimage, face, new Bgr(Color.Red).MCvScalar, 2);
            foreach (Rectangle eye in eyes)
                CvInvoke.Rectangle(grayimage, eye, new Bgr(Color.Blue).MCvScalar, 2);


            //display the image

            //ImageViewer.Show(image, String.Format(
            //   "Completed face and eye detection using {0} in {1} milliseconds",
            //   (tryUseCuda && CudaInvoke.HasCuda) ? "GPU"
            //   : CvInvoke.UseOpenCL ? "OpenCL"
            //   : "CPU",
            //   detectionTime));


            //  Image_BOX.UriSource = BitmapSourceConvert.ToBitmapSource(grayimage);

            captureImageBox.Image = grayimage;

        }

        //private void ModernWindow_Loaded(object sender, RoutedEventArgs e)
        //{
        //    // Create the interop host control.
        //    System.Windows.Forms.Integration.WindowsFormsHost host =
        //        new System.Windows.Forms.Integration.WindowsFormsHost();

        //    // Create the MaskedTextBox control.
        //    MaskedTextBox mtbDate = new MaskedTextBox("00/00/0000");

        //    // Assign the MaskedTextBox control as the host control's child.
        //    host.Child = mtbDate;

        //    // Add the interop host control to the Grid
        //    // control's collection of child controls.
        //    this.grid1.Children.Add(host);
        //}
    }
}
