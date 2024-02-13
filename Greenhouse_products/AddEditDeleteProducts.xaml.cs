﻿using Microsoft.Win32;
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
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;

namespace Greenhouse_products
{
    /// <summary>
    /// Логика взаимодействия для AddEditDeleteProducts.xaml
    /// </summary>
    public partial class AddEditDeleteProducts : Window
    {
        public bool isLoggedIn = ((App)Application.Current).IsLoggedIn;
        public int CurrentUser = ((App)Application.Current).CurrentUser;
        public bool isAdmin = ((App)Application.Current).isAdmin;
        public greenhouse_productsEntities db;
        private byte[] _image = null;
        public int id;
        public AddEditDeleteProducts()
        {
            InitializeComponent();
            if (CurrentUser != 0)
            {
                Заказ заказ = db.Заказ.Where(x => x.Пользователь == CurrentUser).OrderByDescending(x => x.Дата_создания).FirstOrDefault();
                if (заказ.Статус != 1)
                {
                    basket.Visibility = Visibility.Collapsed;
                }
                else
                {
                    int продуция_Заказ = db.Продуция_заказ.Where(x => x.Заказ == заказ.Номер).Count();
                    counts.Text = продуция_Заказ.ToString();
                    basket.Visibility = Visibility;
                }
            }
            else
            {
                basket.Visibility = Visibility.Collapsed;
            }
            save.IsEnabled = false;
            foreach (var d in db.Каталог)
            {
                Categ.Items.Add(d.Наименование);
            }
        }
        public void ListViewLoad()
        {
            using (greenhouse_productsEntities db = new greenhouse_productsEntities())
            {
                var categories = db.Продукция.ToList();

                Categ.ItemsSource = categories;
            }
        }
        private void ChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string path;
            if ((bool)openFileDialog.ShowDialog())
            {
                path = openFileDialog.FileName;
                _image = System.IO.File.ReadAllBytes(path);
            }
            MemoryStream ms = new MemoryStream(_image);
            image.Source = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }
        private void add_Click(object sender, RoutedEventArgs e)
        {
            using (greenhouse_productsEntities db = new greenhouse_productsEntities())
            {
                if (name.Text == null || price.Text == null || weidth.Text == null || count.Text == null || catalog.SelectedItem == null)
                {
                    MessageBox.Show("Заполните все поля");
                }
                else
                {
                    Продукция продукция = new Продукция();
                    продукция.Изображение = _image;
                    продукция.Наименование = name.Text;
                    продукция.Цена = Convert.ToDecimal(price.Text);
                    продукция.Вес = Convert.ToInt32(weidth.Text);
                    продукция.Каталог = catalog.SelectedIndex;
                    db.Продукция.Add(продукция);
                    db.SaveChanges();
                }
                ListViewLoad();
            }
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            save.IsEnabled = true;
            if (Categ.SelectedIndex >= 0)
            {
                using (greenhouse_productsEntities db = new greenhouse_productsEntities())
                {
                    var item = Categ.SelectedItem as Продукция;
                    id = item.Номер;
                    Продукция продукция = db.Продукция.Where(x => x.Номер == id).FirstOrDefault();
                    MemoryStream ms = new MemoryStream(продукция.Изображение);
                    image.Source = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    name.Text = продукция.Наименование;
                    price.Text = продукция.Цена.ToString();
                    weidth.Text = продукция.Вес.ToString();
                    catalog.SelectedIndex = (int)продукция.Каталог;
                    db.SaveChanges();
                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали ни один элемент");
            }
            ListViewLoad();
        }
        public static void RemoveProduct(int id)
        {
            using (greenhouse_productsEntities db = new greenhouse_productsEntities())
            {
                Продукция meal = db.Продукция.Where(x => x.Номер == id).FirstOrDefault();
                db.Продукция.Remove(meal);
                db.SaveChanges();
            }
        }
        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (Categ.SelectedIndex >= 0)
            {
                var result = MessageBox.Show("Вы точно хотите удалить этот товар?", "Удалить", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    var item = Categ.SelectedItem as Продукция;
                    int id = item.Номер;
                    RemoveProduct(id);
                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали ни один элемент");
            }
            ListViewLoad();
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            using (greenhouse_productsEntities db = new greenhouse_productsEntities())
            {
                if (name.Text == null || price.Text == null || weidth.Text == null || count.Text == null || catalog.SelectedItem == null)
                {
                    MessageBox.Show("Заполните все поля");
                }
                else
                {
                    Продукция продукция = db.Продукция.Where(x => x.Номер == id).FirstOrDefault();
                    продукция.Изображение = _image;
                    продукция.Наименование = name.Text;
                    продукция.Цена = Convert.ToDecimal(price.Text);
                    продукция.Вес = Convert.ToInt32(weidth.Text);
                    продукция.Каталог = catalog.SelectedIndex;
                    db.SaveChanges();
                }
                ListViewLoad();
                save.IsEnabled = false;
            }
        }

        private void add_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddEditDeleteProducts addEditDeleteProducts = new AddEditDeleteProducts();
            addEditDeleteProducts.Show();
            this.Hide();
        }

        private void about_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide();
        }

        private void fruits_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Fruits fruits = new Fruits();
            fruits.Show();
            this.Hide();
        }

        private void vegetables_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Vegetables vegetables = new Vegetables();
            vegetables.Show();
            this.Hide();
        }

        private void out_Click(object sender, RoutedEventArgs e)
        {
            isLoggedIn = false;
            CurrentUser = 0;
            Authorization authorization = new Authorization();
            authorization.Show();
            this.Hide();
        }

        private void private_acc_Click(object sender, RoutedEventArgs e)
        {
            if (isLoggedIn)
            {
                PrivateAccount privateAccount = new PrivateAccount();
                privateAccount.Show();
                this.Hide();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Вы не авторизованы", "Авторизоваться", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Authorization authorization = new Authorization();
                    authorization.Show();
                    this.Hide();
                }
            }
        }
    }
}