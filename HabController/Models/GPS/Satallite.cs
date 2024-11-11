using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabController.Models.GPS
{
    public class Satallite
    {
        public int ID { get; set; }
        public int Elevation { get; set; }
        public int Azimuth { get; set; }
        public int SignalToNoise { get; set; }

        public Satallite(string id, string elevation, string azimuth, string signalToNoise)
        {
            if (signalToNoise.Contains('*'))
            {
                signalToNoise = signalToNoise.Substring(0, signalToNoise.IndexOf("*"));
            }

            ID = int.Parse(id);
            Elevation = int.Parse(elevation);
            Azimuth = int.Parse(azimuth);
            if (!string.IsNullOrEmpty(signalToNoise))
            {
                SignalToNoise = int.Parse(signalToNoise);
            }
        }
    }
}
