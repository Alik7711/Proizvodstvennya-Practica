using System;

namespace SoftwareLibrary.Models
{
    public class Software
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string LicenseType { get; set; }
        public string Manufacturer { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal Cost { get; set; }
        public string ResponsiblePerson { get; set; }
        public int UserCount { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Notes { get; set; }

        public Software()
        {
            Id = 0;
            Name = "";
            Version = "";
            LicenseType = "";
            Manufacturer = "";
            PurchaseDate = DateTime.Now;
            Cost = 0;
            ResponsiblePerson = "";
            UserCount = 1;
            ExpirationDate = null;
            Notes = "";
        }
    }
}