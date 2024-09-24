using System;
using System.Collections.Generic;
using System.IO;
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

namespace PetShop.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        public AdminLkPage AdminPage { get; set; }
        public ManagerLkPage ManagerPage { get; set; }
        public ClientLkPage ClientPage { get; set; }
        public string FlagAddOrEdit = "default";
        public bool FlagPhoto = false;
        public Data.Product CurrentProduct = new Data.Product();
        public AddEditPage(Data.Product _product)
        {
            InitializeComponent();

            if (_product == null)
            {
                FlagAddOrEdit = "add";
            }
            else
            {
                CurrentProduct = _product;
                FlagAddOrEdit = "edit";
            }

            DataContext = CurrentProduct;

            Init();
        }

        public void Init()
        {
            try
            {
                CategoryComboBox.ItemsSource = Data.Trade2Entities.GetContext().CategoryProduct.ToList();

                if (FlagAddOrEdit == "add")
                {
                    IdTextBox.Visibility = Visibility.Hidden;
                    IdLabel.Visibility = Visibility.Hidden;
                    NameTextBox.Text = string.Empty;
                    CategoryComboBox.SelectedItem = null;
                    UnitTextBox.Text = string.Empty;
                    SupplierTextBox.Text = string.Empty;
                    CostTextBox.Text = string.Empty;
                    QuantityTextBox.Text = string.Empty;
                    DescriptionTextBox.Text = string.Empty;
                }
                else if (FlagAddOrEdit == "edit")
                {
                    IdTextBox.Visibility = Visibility.Visible;
                    IdLabel.Visibility = Visibility.Visible;

                    IdTextBox.Text = CurrentProduct.ProductArticleNumber.ToString();
                    FlagPhoto = true;
                    NameTextBox.Text = CurrentProduct.NameofSupply.Supply;
                    CategoryComboBox.SelectedItem = Data.Trade2Entities.GetContext().CategoryProduct.Where(d => d.ID == CurrentProduct.IDCategoryProduct).FirstOrDefault();
                    UnitTextBox.Text = CurrentProduct.Units.Name;
                    SupplierTextBox.Text = CurrentProduct.Producer.Producer1;
                    CostTextBox.Text = CurrentProduct.Cost.ToString();
                    QuantityTextBox.Text = CurrentProduct.CountOnStorage.ToString();
                    DescriptionTextBox.Text = CurrentProduct.Description;
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Classes.Manager.MainFrame.CanGoBack)
            {
                Classes.Manager.MainFrame.GoBack();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();

                if (string.IsNullOrEmpty(NameTextBox.Text))
                {
                    errors.AppendLine("Заполните наименование");
                }
                if (CategoryComboBox.SelectedItem == null)
                {
                    errors.AppendLine("Выберите категорию");
                }
                if (string.IsNullOrEmpty(QuantityTextBox.Text))
                {
                    errors.AppendLine("Заполните количество");
                }
                else
                {
                    var tryQuantity = Int32.TryParse(QuantityTextBox.Text, out var resultQuantity);
                    if (!tryQuantity)
                    {
                        errors.AppendLine("Количество - целое число");
                    }
                }
                if (string.IsNullOrEmpty(UnitTextBox.Text))
                {
                    errors.AppendLine("Заполните ед.измерения");
                }
                if (string.IsNullOrEmpty(SupplierTextBox.Text))
                {
                    errors.AppendLine("Заполните поставщика");
                }
                if (string.IsNullOrEmpty(CostTextBox.Text))
                {
                    errors.AppendLine("Заполните стоимость");
                }
                else
                {
                    var tryCost = Decimal.TryParse(CostTextBox.Text, out var resultCost);
                    if (!tryCost)
                    {
                        errors.AppendLine("Стоимость - дробное число");
                    }
                    else
                    {
                        var costText = CostTextBox.Text.Trim();

                        var parts = costText.Split(new[] { ',', '.' });

                        if (parts.Length > 1 && parts[1].Length > 2)
                        {
                            errors.AppendLine("Дробная часть может содержать только 2 знака после запятой");
                        }
                    }

                    if (tryCost && resultCost < 0)
                    {
                        errors.AppendLine("Стоимость не может быть отрицательной!");
                    }
                }

                if (string.IsNullOrEmpty(DescriptionTextBox.Text))
                {
                    errors.AppendLine("Заполните описание");
                }

                if (FlagPhoto == false)
                {
                    errors.AppendLine("Выберите изображение!");
                }

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                var selectedCategory = CategoryComboBox.SelectedItem as Data.CategoryProduct;
                CurrentProduct.IDCategoryProduct = selectedCategory.ID;

                if (FlagAddOrEdit == "add")
                {
                    CurrentProduct.ProductArticleNumber = RandomArticle();
                }
                CurrentProduct.Cost = Convert.ToDecimal(CostTextBox.Text);
                CurrentProduct.CountOnStorage = Convert.ToInt32(QuantityTextBox.Text);
                CurrentProduct.Description = DescriptionTextBox.Text;

                var searchName = (from obj in Data.Trade2Entities.GetContext().NameofSupply
                                  where obj.Supply == NameTextBox.Text
                                  select obj).FirstOrDefault();

                if (searchName != null)
                {
                    CurrentProduct.IDSupply = searchName.ID;
                }
                else
                {
                    Data.NameofSupply productName = new Data.NameofSupply()
                    {
                        Supply = NameTextBox.Text
                    };
                    Data.Trade2Entities.GetContext().NameofSupply.Add(productName);
                    Data.Trade2Entities.GetContext().SaveChanges();

                    CurrentProduct.IDSupply = productName.ID;
                }


                var searchUnit = (from obj in Data.Trade2Entities.GetContext().Units
                                  where obj.Name == UnitTextBox.Text
                                  select obj).FirstOrDefault();

                if (searchUnit != null)
                {
                    CurrentProduct.IDUnits = searchUnit.ID;
                }
                else
                {
                    Data.Units UnitName = new Data.Units()
                    {
                        Name = UnitTextBox.Text
                    };
                    Data.Trade2Entities.GetContext().Units.Add(UnitName);
                    Data.Trade2Entities.GetContext().SaveChanges();

                    CurrentProduct.IDUnits = UnitName.ID;
                }

                var searchProvider = (from obj in Data.Trade2Entities.GetContext().Provider
                                      where obj.Provider1 == SupplierTextBox.Text
                                      select obj).FirstOrDefault();

                if (searchProvider != null)
                {
                    CurrentProduct.IDProvider = searchProvider.ID;
                }
                else
                {
                    Data.Provider ProviderName = new Data.Provider()
                    {
                        Provider1 = SupplierTextBox.Text
                    };
                    Data.Trade2Entities.GetContext().Provider.Add(ProviderName);
                    Data.Trade2Entities.GetContext().SaveChanges();

                    CurrentProduct.IDProvider = ProviderName.ID;
                }

                var searchProducer = (from obj in Data.Trade2Entities.GetContext().Producer
                                      where obj.Producer1 == SupplierTextBox.Text
                                      select obj).FirstOrDefault();

                if (searchProducer != null)
                {
                    CurrentProduct.IDProducer = searchProducer.ID;
                }
                else
                {
                    Data.Producer ProducerName = new Data.Producer()
                    {
                        Producer1 = SupplierTextBox.Text
                    };
                    Data.Trade2Entities.GetContext().Producer.Add(ProducerName);
                    Data.Trade2Entities.GetContext().SaveChanges();

                    CurrentProduct.IDProducer = ProducerName.ID;
                }


                if (FlagAddOrEdit == "edit")
                {
                    Data.Trade2Entities.GetContext().SaveChanges();

                    MessageBox.Show("Успешно сохранено!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (FlagAddOrEdit == "add")
                {
                    Data.Trade2Entities.GetContext().Product.Add(CurrentProduct);
                    Data.Trade2Entities.GetContext().SaveChanges();

                    MessageBox.Show("Успешно добавлено!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                if (AdminPage != null)
                {
                    AdminPage.Update();
                    AdminPage.init();
                }

                if (ClientPage != null)
                {
                    ClientPage.Update();
                    ClientPage.init();
                }

                if (ManagerPage != null)
                {
                    ManagerPage.Update();
                    ManagerPage.init();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static string RandomArticle()
        {
            string allowChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string randomarticle = "";
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                randomarticle += allowChars[random.Next(allowChars.Length)];
            }
            return randomarticle;
        }


        private void ProductImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() == true)
                {
                    var imageSource = new BitmapImage(new Uri(openFileDialog.FileName));

                    if (imageSource.PixelWidth <= 300 && imageSource.PixelHeight <= 200)
                    {
                        FlagPhoto = true;
                        ProductImage.Source = imageSource;

                        byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                        string imageName = System.IO.Path.GetFileName(openFileDialog.FileName);

                        CurrentProduct.ProductName = imageName;
                        CurrentProduct.ProductPhoto = imageBytes;
                    }
                    else
                    {
                        MessageBox.Show("Выберите изображение с разрешением не более 300x200 пикселей.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выборе изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
