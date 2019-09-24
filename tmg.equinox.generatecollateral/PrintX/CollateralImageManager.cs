using Hangfire.Logging;
using Newtonsoft.Json;
using Spire.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.Collateral;

namespace tmg.equinox.generatecollateral
{
    public class CollateralImageManager
    {
        private List<CollateralImage> _collateralImages;
        private ICollateralService _collateralService;
        private List<CollateralImageView> _collateralImageCollection;
        private static readonly ILog _logger = LogProvider.For<PrintXReady>();
        public CollateralImageManager(ICollateralService collateralService)
        {
            _collateralImages = new List<CollateralImage>();
            _collateralService = collateralService;
            _collateralImageCollection = _collateralService.GetCollateralImages();
        }
        public void AddImages(int orderIndex, string imageName)
        {
            var image = new CollateralImage { Name = imageName, OrderIndex = orderIndex };
            _collateralImages.Add(image);
        }

        public void UpdateImage(Image pdfImage, int orderIndex)
        {
            var image = _collateralImages.Find(m => m.OrderIndex == orderIndex);
            if (image != null)
            {
                image.Tag = pdfImage.Tag.ToString();
            }
        }
        private string GetImageName(string tag)
        {
            foreach (var im in _collateralImages)
            {
                if (im.Tag == tag)
                {
                    return im.Name;
                }
            }

            return null;
        }
        public void LogImages(Exception e)
        {
            string s = string.Format("{0}--{1}", JsonConvert.SerializeObject(_collateralImageCollection), JsonConvert.SerializeObject(_collateralImages));
            _logger.ErrorException(s, e);
        }
        private CollateralImageView GetPath(string imageName)
        {
            return _collateralImageCollection
                                        .Where(m => m.Name == imageName || m.FileName == imageName).FirstOrDefault();

        }
        public PdfImage GetCMYKImage(string tag)
        {
            //get Image Name
            var imageName = GetImageName(tag);

            var colImage = GetPath(imageName);

            if (colImage == null)
            {
                return null;
            }
            else
            {
                var directoryPath = ConfigurationManager.AppSettings["UIPath"];
                if (directoryPath==null)
                {
                    throw new Exception("UIPath not set in App.confi");
                }

                string path = string.Format(@"{0}{1}", directoryPath.ToString(), colImage.URL.Replace("/","\\").Replace("..",""), colImage.FileName);
                var img = PdfImage.FromFile(path);
                return img;
            }
        }
    }
}
