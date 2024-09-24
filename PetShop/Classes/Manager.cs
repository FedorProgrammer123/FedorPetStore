using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PetShop.Classes
{
    public static class Manager
    {
        public static Data.User User { get; set; }
        public static Frame MainFrame { get; set; }

        public static void GetImageData()
        {
            try
            {
                var list = Data.Trade2Entities.GetContext().Product.ToList();
                foreach (var item in list)
                {
                    string path = Directory.GetCurrentDirectory() + @"\img\" + item.ProductName;
                    if (File.Exists(path))
                    {
                        item.ProductPhoto = File.ReadAllBytes(path);
                    }
                }
                Data.Trade2Entities.GetContext().SaveChanges();
            }
            catch (Exception) 
            {
                
            }
        }
    }
}
