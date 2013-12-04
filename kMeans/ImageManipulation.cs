using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using kMeans.Model;
using System.ComponentModel;

namespace kMeans
{
    public class ImageManipulation
    {
        //Properties

        public List<Pixel> pixels = new List<Pixel>();

        public List<Centroid> centroids = new List<Centroid>();

        public bool keepGoing;

        //Functions

        #region Grayscale

        public System.Windows.Media.Imaging.BitmapImage ConvertToGrayScale(Bitmap bmap)
        {
            pixels.Clear();

            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    Color pixColor = bmap.GetPixel(i, j);

                    pixels.Add(new Pixel { posX = i, posY = j, cluster = 0, rgb = ConvertPixelColorToGrayScale(pixColor) });
                    
                    bmap.SetPixel(i, j, ConvertPixelColorToGrayScale(pixColor));
                }
            }

            /*
            foreach (Pixel pix in Pixels)
            {
                Debug.WriteLine(pix.cluster.ToString() + " -- " + pix.posX.ToString() + " - " + pix.posY.ToString());
            }
             * */

            return BitmapToBitMapImage(bmap);
        }

        public Color ConvertPixelColorToGrayScale(Color pix)
        {
            int natural = (pix.R + pix.G + pix.B) / 3;
            Color newColor = Color.FromArgb(255, natural, natural, natural);
            return newColor;
        }

        #endregion

        #region Simple kMeans

        //Main kMeans function. Bitmap needed.
        public void kMeanify(Bitmap bmap)
        {
            centroids.Clear();

            keepGoing = true;
            int i = 0;
            while(keepGoing)
            {
                Debug.WriteLine(" **************** " + i.ToString() + " **************** ");
                i++;

                if (centroids.Count == 0)
                {
                    CreateInitCentroids(bmap);
                }
                else
                {
                    GenerateNewCentroids();
                }

                //Call Cluster method in ImageManipulation.cs
                ClusterPixels();

                //Percent++;
            }
        
            //for (int i = 0; i < 20; i++)
            //{
            //    Debug.WriteLine(" **************** " + i.ToString() + " **************** ");

            //    if (centroids.Count == 0)
            //    {
            //        CreateInitCentroids(bmap);
            //    }
            //    else
            //    {
            //        GenerateNewCentroids();
            //    }

            //    //Call Cluster method in ImageManipulation.cs
            //    ClusterPixels();

            //    //Percent++;
            //}
        }

        //Function to gen random initial centroids.
        public void CreateInitCentroids(Bitmap bmap)
        {
            //Gen random number = k.

            //int k = randomNum(0, 255);


            int k = 2;

            Debug.WriteLine(k.ToString());

            //Loop k times and create k amount of centroids. 
            for (int i = 1; i <= k; i++)
            {
                
                int x = randomNum(0, bmap.Width);
                int y = randomNum(0, bmap.Height);
                System.Drawing.Color centroidColor = getColorOfCentroid(bmap, x, y);
                centroids.Add(new Centroid() { posX = x, posY = y, rgb = centroidColor });
            }

            //int cenIndex = 0;
            //foreach (Centroid cen in centroids)
            //{
            //    Debug.WriteLine(cenIndex.ToString() + ") " + cen.rgb.ToString());
            //}
        }

        //Function to place each pixel in the corresponding.
        public void ClusterPixels()
        {
            //Do distance formula on each pixel vs each centroid... associate with shortest distance..

            foreach (Pixel pix in pixels)
            {
                double[] dis = new double[centroids.Count];
                //Check the distance formula on each centroid...
                int i = 0;
                foreach (Centroid cen in centroids)
                {
                    dis[i] = getDistanceBetweenPoints(pix.rgb.R, cen);
                    i++;
                    //Do distance fromula on each centroid to each pizel
                }

                //Set Cluster
                pix.cluster = Array.IndexOf(dis, dis.Min());

                //Change to cluster color.
                //pix.rgb = centroids[pix.cluster].rgb;

            }
        }

        //Function returning the distance between the point and the centroid. (really just color based, not actual x and y.)
        public double getDistanceBetweenPoints(int pixColor, Centroid cen)
        {
            /*
            int x;
            if (posX > cen.posX)
            {
                x = posX - cen.posX;
            }
            else
            {
                x = cen.posX - posX;
            }

            int y;
            if (posY > cen.posY)
            {
                y = posY - cen.posY;
            }
            else
            {
                y = cen.posY - posY;
            }

            double distance = Math.Sqrt((x * x) + (y * y));

            */
            int x;
            if (pixColor > cen.rgb.R)
            {
                x = pixColor - cen.rgb.R;
            }
            else
            {
                x = cen.rgb.R - pixColor;
            }

            double distance = Math.Sqrt(x * x);

            return distance;
        }

        //Function to generate new centroids. This is not for the initial run.
        public void GenerateNewCentroids()
        {
            /*
             * Add up all the grayscale values and divide by the total amount according to that cluster.
             * 
             * This gives you the new color for the distances to be compared to via the distance alg.
             * 
             * Compare the distances.
             * 
             * Re-cluster
             * 
             */

            //Console.WriteLine("Re-Cluster!");

            int cenIndex = 0;

            //Keep Copy of Old Centroid to check for convergence against.
            List<Centroid> oldCentroids = new List<Centroid>(centroids.Count);
            //Copy the centroids into the old.
            oldCentroids = centroids.ToList();


            foreach (Centroid cen in centroids)
            {
                IEnumerable<Pixel> pixelsInCluster =
                from pix in pixels
                where pix.cluster == cenIndex
                select pix;

                int total = 0;
                int amt = 0;
                foreach (Pixel pix in pixelsInCluster)
                {
                    total += pix.rgb.R;
                    amt++;
                }

                //Debug.WriteLine(total + " / " + amt);
                if (amt != 0)
                {
                    total = total / amt;

                    Color clr = Color.FromArgb(255, total, total, total);

                    cen.rgb = clr;
                }

                //newClusterRGBs[cenIndex] = total;

                cenIndex++;

                //Debug.WriteLine(cenIndex.ToString() + ") " + cen.rgb.ToString());
            }

            //Compare New centroids vs. Old ones..
            int count = 0;
            int changes = 0;
            foreach (Centroid cen in centroids)
            {
                if (oldCentroids[count].rgb.R == cen.rgb.R)
                {
                    changes++;
                }
                count++;
            }

            if (changes == centroids.Count)
            {
                keepGoing = false;
            }

            /*
            //int total = 0;
            int[] totals = new int[centroids.Count];
            int[] amounts = new int[centroids.Count];
            foreach (Pixel pix in pixels)
            {
                totals[pix.cluster] += pix.rgb.R;
                amounts[pix.cluster] += 1;
            }

            int j = 0;
            foreach (int i in amounts)
            {
                totals[j] = totals[j] / amounts[j];
                j++;
            }
            Debug.WriteLine(totals.ToString());

             */
            //foreach (Pixel pix in pixels)
            //{
            //    getDistanceBetweenPoints(pix.rgb.R, total);
            //}
        }

        //Function to return an Image from the list of pixels.
        public System.Windows.Media.Imaging.BitmapImage GetClusteredImage(int width, int height)
        {
            Bitmap bmap = new Bitmap(width, height);

            foreach (Pixel pix in pixels)
            {
                //pix.rgb = centroids[pix.cluster].rgb;
                bmap.SetPixel(pix.posX, pix.posY, centroids[pix.cluster].rgb);
            }

            return BitmapToBitMapImage(bmap);
        }

        //Simple helper function to get the color or any specific centroid.
        public System.Drawing.Color getColorOfCentroid(Bitmap bmap, int x, int y)
        {
            return bmap.GetPixel(x, y);
        }

        //Function to return a random number between a max and min.
        static Random r = new Random();
        static int randomNum(int min, int max)
        {
            return r.Next(min, max);
        }

        #endregion

        #region Util

        public System.Windows.Media.Imaging.BitmapImage BitmapToBitMapImage(Bitmap bitmap)
        {

            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            stream.Position = 0;
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, Convert.ToInt32(stream.Length));
            BitmapImage bmapImage = new BitmapImage();
            bmapImage.BeginInit();
            bmapImage.StreamSource = stream;
            bmapImage.EndInit();

            return bmapImage;
        }

        #endregion
    }
}
