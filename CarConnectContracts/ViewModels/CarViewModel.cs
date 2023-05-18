using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConnectContracts.ViewModels
{
    public class CarViewModel
    {
        public int Id { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public DateTime Year { get; set; }
        public string? VIN { get; set; }
        public string? LicensePlate { get; set; }
        public string? Colour { get; set; }
        public string? FileName { get; set; }
        public string? Path { get; set; }
        public string? UserName { get; set; }
        public string? UserPhone { get; set; }
        public string? UserEmail { get; set; }
        public int UserId { get; set; }
    }
}
