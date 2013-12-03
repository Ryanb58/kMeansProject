using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using kMeans.Model;

namespace kMeans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog op = new OpenFileDialog();

        System.Drawing.Bitmap bmap;

        BitmapImage grayBmap;

        ImageManipulation img = new ImageManipulation();

        //public List<Centroid> centroids = new List<Centroid>();
       
        public MainWindow()
        {
            InitializeComponent();

            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            if (op.ShowDialog() == true)
            {
                origImage.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void grayScaleButton_Click(object sender, RoutedEventArgs e)
        {
            bmap = new System.Drawing.Bitmap(op.FileName);

            grayBmap = img.ConvertToGrayScale(bmap);
            
            grayImage.Source = grayBmap;
        }

        private void kMeans_Click(object sender, RoutedEventArgs e)
        {
            //centroids.Clear();
            img.kMeanify(bmap);

            
            

            //Get clusters... return to user.

            //Display Cluster Of pixels in image..
            //Debug.WriteLine("Width : Height - " + bmap.Width.ToString() + " : " + bmap.Height.ToString());
            kmeansImage.Source = img.GetClusteredImage(bmap.Width, bmap.Height);
        }

        //public void kMeanify()
        //{
        //    for(int i=0; i<10; i++)
        //    {
        //        if (centroids.Count == 0)
        //        {
        //            CreateInitCentroids();
        //        }
        //        else
        //        {
        //            img.GenerateNewCentroids(centroids);
        //        }

        //        //Call Cluster method in ImageManipulation.cs
        //        img.ClusterPixels(centroids);
        //    }
        //}

        //public void CreateInitCentroids()
        //{
        //    //Gen random number = k.
        //    /*
        //     * Real Code:
            
        //     */
            
        //    //int k = randomNum(0, 255);
              
             
        //    int k = 3;
            
        //    Debug.WriteLine(k.ToString());

        //    //Loop k times and create k amount of centroids. 
        //    for (int i = 0; i <= k; i++)
        //    {
        //        Debug.WriteLine(i.ToString());
        //        int x = randomNum(0, bmap.Width);
        //        int y = randomNum(0, bmap.Height);
        //        System.Drawing.Color centroidColor = getColorOfCentroid(x,y);
        //        centroids.Add(new Centroid() { posX = x, posY = y, rgb = centroidColor });
        //    }

        //    //foreach (Centroid cen in centroids)
        //    //{
        //    //    Debug.WriteLine(cen.posX.ToString() + " - " + cen.posY.ToString());
        //    //}
        //}

        //public System.Drawing.Color getColorOfCentroid(int x, int y)
        //{
        //    return bmap.GetPixel(x, y);
        //}

        //static Random r = new Random();
        //static int randomNum(int min, int max)
        //{
        //    return r.Next(min, max);
        //}
    }
}
