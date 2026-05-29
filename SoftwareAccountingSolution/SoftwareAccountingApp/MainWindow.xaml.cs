using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SoftwareLibrary.Models;
using SoftwareLibrary.Services;

namespace SoftwareAccountingApp
{
    public partial class MainWindow : Window
    {
        private JsonFileStorage _storage;
        private Software _selectedSoftware;

        public MainWindow()
        {
            InitializeComponent();
            _storage = new JsonFileStorage("software.json");
            LoadData();
            UpdateStatus("Готов к работе");
        }

        private void LoadData()
        {
            var list = _storage.GetAll();
            dgSoftware.ItemsSource = list;
            UpdateStatus($"Загружено {list.Count} записей");
        }

        private void ClearForm()
        {
            txtName.Text = "";
            txtVersion.Text = "";
            cmbLicenseType.SelectedIndex = -1;
            txtManufacturer.Text = "";
            dpPurchaseDate.SelectedDate = null;
            txtCost.Text = "";
            txtResponsible.Text = "";
            txtUserCount.Text = "1";
            dpExpirationDate.SelectedDate = null;
            txtNotes.Text = "";
            _selectedSoftware = null;
        }

        private Software GetSoftwareFromForm()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название программного обеспечения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            if (string.IsNullOrWhiteSpace(txtVersion.Text))
            {
                MessageBox.Show("Введите версию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            if (cmbLicenseType.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите тип лицензии!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            decimal cost = 0;
            decimal.TryParse(txtCost.Text, out cost);

            int userCount = 1;
            int.TryParse(txtUserCount.Text, out userCount);

            var software = new Software
            {
                Name = txtName.Text,
                Version = txtVersion.Text,
                LicenseType = (cmbLicenseType.SelectedItem as ComboBoxItem)?.Content.ToString().Replace("💰 ", "").Replace("🎁 ", "").Replace("📅 ", "").Replace("⏳ ", "").Replace("🔓 ", ""),
                Manufacturer = txtManufacturer.Text,
                PurchaseDate = dpPurchaseDate.SelectedDate ?? DateTime.Now,
                Cost = cost,
                ResponsiblePerson = txtResponsible.Text,
                UserCount = userCount,
                ExpirationDate = dpExpirationDate.SelectedDate,
                Notes = txtNotes.Text
            };

            if (_selectedSoftware != null)
                software.Id = _selectedSoftware.Id;

            return software;
        }

        private void UpdateStatus(string message)
        {
            txtStatus.Text = $"✅ {DateTime.Now:HH:mm:ss} - {message}";
        }

        // ========== КНОПКИ ==========

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var software = GetSoftwareFromForm();
            if (software == null) return;

            _storage.Add(software);
            LoadData();
            ClearForm();
            MessageBox.Show($"ПО \"{software.Name}\" добавлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateStatus($"Добавлено: {software.Name} v{software.Version}");
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSoftware == null)
            {
                MessageBox.Show("Выберите запись для обновления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var software = GetSoftwareFromForm();
            if (software == null) return;

            _storage.Update(software);
            LoadData();
            ClearForm();
            MessageBox.Show($"ПО \"{software.Name}\" обновлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateStatus($"Обновлено: {software.Name} v{software.Version}");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSoftware == null)
            {
                MessageBox.Show("Выберите запись для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить ПО \"{_selectedSoftware.Name}\"?", "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                string name = _selectedSoftware.Name;
                _storage.Delete(_selectedSoftware.Id);
                LoadData();
                ClearForm();
                MessageBox.Show($"ПО \"{name}\" удалено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateStatus($"Удалено: {name}");
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            UpdateStatus("Форма очищена");
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            ClearForm();
            UpdateStatus("Список обновлен");
        }

        private void BtnShowAll_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            LoadData();
            UpdateStatus("Показаны все записи");
        }

        // ========== ПОИСК ==========

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Search()
        {
            var results = _storage.Search(txtSearch.Text);
            dgSoftware.ItemsSource = results;
            UpdateStatus($"Найдено {results.Count} записей по запросу \"{txtSearch.Text}\"");
        }

        // ========== ВЫБОР В ТАБЛИЦЕ ==========

        private void DgSoftware_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSoftware.SelectedItem != null)
            {
                _selectedSoftware = dgSoftware.SelectedItem as Software;

                txtName.Text = _selectedSoftware.Name;
                txtVersion.Text = _selectedSoftware.Version;

                // Установка значения в ComboBox
                bool found = false;
                for (int i = 0; i < cmbLicenseType.Items.Count; i++)
                {
                    var item = cmbLicenseType.Items[i] as ComboBoxItem;
                    if (item != null)
                    {
                        string itemText = item.Content.ToString().Replace("💰 ", "").Replace("🎁 ", "").Replace("📅 ", "").Replace("⏳ ", "").Replace("🔓 ", "");
                        if (itemText == _selectedSoftware.LicenseType)
                        {
                            cmbLicenseType.SelectedIndex = i;
                            found = true;
                            break;
                        }
                    }
                }
                if (!found) cmbLicenseType.SelectedIndex = -1;

                txtManufacturer.Text = _selectedSoftware.Manufacturer;
                dpPurchaseDate.SelectedDate = _selectedSoftware.PurchaseDate;
                txtCost.Text = _selectedSoftware.Cost.ToString();
                txtResponsible.Text = _selectedSoftware.ResponsiblePerson;
                txtUserCount.Text = _selectedSoftware.UserCount.ToString();
                dpExpirationDate.SelectedDate = _selectedSoftware.ExpirationDate;
                txtNotes.Text = _selectedSoftware.Notes;

                UpdateStatus($"Выбрано: {_selectedSoftware.Name}");
            }
        }
    }
}