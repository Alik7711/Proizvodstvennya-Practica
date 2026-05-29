using System.Collections.Generic;
using SoftwareLibrary.Models;

namespace SoftwareLibrary.Services
{
    public interface IDataStorage
    {
        List<Software> GetAll();
        Software GetById(int id);
        void Add(Software software);
        void Update(Software software);
        void Delete(int id);
        List<Software> Search(string searchText);
    }
}