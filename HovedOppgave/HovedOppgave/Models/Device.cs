using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Models
{
    public class Device
    {
        public int DeviceID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SerialNum { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public bool IsRackMountable { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public decimal InputVoltage { get; set; }

        //Foreignkeys
        public virtual int DeviceTypeID { get; set; }
        public virtual int RoomID { get; set; }

        public Device()
        { }

        public Device(string name, string description, string serialNum, int height, int weight,
            bool isRackMountable, string model, string brand, decimal inputVoltage)
        {
            this.Name = name;
            this.Description = description;
            this.SerialNum = serialNum;
            this.Height = height;
            this.Weight = weight;
            this.IsRackMountable = isRackMountable;
            this.Model = model;
            this.Brand = brand;
            this.InputVoltage = inputVoltage;
        }
    }
}