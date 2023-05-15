﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConnectContracts.BindingModels
{
    public class CarBindingModel
    {
        public int? Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public DateTime Year { get; set; }
        public string VIN { get; set; }
        public string LicensePlate { get; set; }
        public string Colour { get; set; }
        public byte Photo { get; set; }
    }
}