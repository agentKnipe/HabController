using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabController.Models.GPS
{
    public enum FixTypeEnum
    {
        NoFix = 0,
        GpsOnly = 1,
        DGPS = 2,
        PPS = 3,
        RealTimeKinematic = 4,
        FloatRTK = 5,
        Estimated = 6,
        ManualMode = 7,
        SimulationMode = 8
    }

    public class SystemFix
    {
        public string CurrentTime { get; set; }
        public FixTypeEnum FixType { get; set; }
        public string FixAccuracy { get; set; }
        public double HorizontalPrecisionValue { get; set; }
        public string Latitude { get; set; }
        public string LatitudeDirection { get; set; }
        public string Longitude { get; set; }
        public string LongitudeDirection { get; set; }

        public int SatallitesInFix { get; set; }
        public double Altitude { get; set; }
        public string AltitudeUnits { get; set; }

        public string Sentence { get; set; }

        public string LatitudeDisplay
        {
            get
            {
                if (!string.IsNullOrEmpty(Latitude))
                {
                    var degrees = double.Parse(Latitude.Substring(0, 2));
                    var minutes = double.Parse(Latitude.Substring(2, 6)) / 60.0;

                    return $"{(degrees + minutes).ToString("##.#####")} {LatitudeDirection}";
                }

                return string.Empty;
            }
        }

        public string LongitudeDisplay
        {
            get
            {
                if (!string.IsNullOrEmpty(Longitude))
                {

                    var degrees = double.Parse(Longitude.Substring(0, 3));
                    var minutes = double.Parse(Longitude.Substring(3, 7)) / 60.0;

                    return $"{(degrees + minutes).ToString("##.#####")} {LongitudeDirection}";
                }

                return string.Empty;
            }
        }

        public string AltitudeDisplay
        {
            get
            {
                return $"{Altitude} {AltitudeUnits}";
            }
        }

        public void ProcessSentence(string sentence)
        {
            //$GPGGA,210230,3855.4487,N,09446.0071,W,1,07,1.1,370.5,M,-29.5,M,,*7A
            //0     ,1     ,2        ,3,4         ,5,6,7 ,8  ,9    ,10,11  ,12,13,14 --indexes

            var parts = sentence.Split(',');

            CurrentTime = ParseTime(parts[1]);
            Latitude = parts[2];
            LatitudeDirection = parts[3];
            Longitude = parts[4];
            LongitudeDirection = parts[5];

            SatallitesInFix = int.Parse(parts[7]);
            Altitude = double.Parse(parts[9]);
            AltitudeUnits = parts[10];

            FixAccuracy = GetPrecision(double.Parse(parts[8]));
            HorizontalPrecisionValue = double.Parse(parts[8]);

            FixType = (FixTypeEnum)(int.Parse(parts[6]));

            Sentence = sentence.Replace('\r', ' ').Trim();
        }

        private string ParseTime(string time)
        {
            var fixedTime = $"{time.Substring(0, 2)}:{time.Substring(2, 2)}:{time.Substring(4, 6)}";

            return fixedTime;
        }

        private string GetPrecision(double precisionValue)
        {
            var returnString = string.Empty;

            if (precisionValue < 1)
            {
                returnString = "Ideal";
            }
            else if (precisionValue >= 1 && precisionValue < 2)
            {
                returnString = "Excellent";
            }
            else if (precisionValue >= 2 && precisionValue < 5)
            {
                returnString = "Good";
            }
            else if (precisionValue >= 5 && precisionValue < 10)
            {
                returnString = "Moderate";
            }
            else if (precisionValue >= 10 && precisionValue < 20)
            {
                returnString = "Fair";
            }
            else
            {
                returnString = "Poor";
            }

            return returnString;
        }
    }
}
