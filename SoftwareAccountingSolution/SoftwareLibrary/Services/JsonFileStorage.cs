using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SoftwareLibrary.Models;

namespace SoftwareLibrary.Services
{
    public class JsonFileStorage : IDataStorage
    {
        private readonly string _filePath;
        private List<Software> _softwareList;
        private int _nextId;

        public JsonFileStorage(string filePath = "software.json")
        {
            _filePath = filePath;
            LoadData();
        }

        private void LoadData()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                _softwareList = JsonConvert.DeserializeObject<List<Software>>(json) ?? new List<Software>();

                if (_softwareList.Count > 0)
                    _nextId = _softwareList.Max(s => s.Id) + 1;
                else
                    _nextId = 1;
            }
            else
            {
                _softwareList = new List<Software>();
                _nextId = 1;
                SaveData();
            }
        }

        private void SaveData()
        {
            string json = JsonConvert.SerializeObject(_softwareList, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public List<Software> GetAll()
        {
            return _softwareList.ToList();
        }

        public Software GetById(int id)
        {
            return _softwareList.FirstOrDefault(s => s.Id == id);
        }

        public void Add(Software software)
        {
            software.Id = _nextId++;
            _softwareList.Add(software);
            SaveData();
        }

        public void Update(Software software)
        {
            var existing = GetById(software.Id);
            if (existing != null)
            {
                existing.Name = software.Name;
                existing.Version = software.Version;
                existing.LicenseType = software.LicenseType;
                existing.Manufacturer = software.Manufacturer;
                existing.PurchaseDate = software.PurchaseDate;
                existing.Cost = software.Cost;
                existing.ResponsiblePerson = software.ResponsiblePerson;
                existing.UserCount = software.UserCount;
                existing.ExpirationDate = software.ExpirationDate;
                existing.Notes = software.Notes;
                SaveData();
            }
        }

        public void Delete(int id)
        {
            var software = GetById(id);
            if (software != null)
            {
                _softwareList.Remove(software);
                SaveData();
            }
        }

        public List<Software> Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAll();

            searchText = searchText.ToLower();
            return _softwareList.Where(s =>
                s.Name.ToLower().Contains(searchText) ||
                s.Version.ToLower().Contains(searchText) ||
                s.LicenseType.ToLower().Contains(searchText) ||
                s.Manufacturer.ToLower().Contains(searchText) ||
                s.ResponsiblePerson.ToLower().Contains(searchText)
            ).ToList();
        }
    }
}