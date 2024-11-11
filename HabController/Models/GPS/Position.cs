using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabController.Models.GPS
{
    public enum FixStatusEnum
    {
        Invalid, Valid
    }

    public class Position
    {
        public DateTime CurrentDateTime { get; set; }
        public FixStatusEnum FixStatus { get; set; }
        public string Latitude { get; set; }
        public string LatitudeDirection { get; set; }
        public string Longitude { get; set; }
        public string LongitudeDirection { get; set; }
        public double CurrentSpeed { get; set; }
        public double CurrentHeading { get; set; }

        public string Sentence { get; set; }
        public bool IsReady { get; set; }


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

        public void ProcessSentence(string sentence)
        {
            //$GPRMC,210230,A,3855.4487,N,09446.0071,W,0.0,076.2,130495,003.8,E*69
            //0     ,1     ,2,3        ,4,5         ,6,7  ,8    ,9     ,10   ,11 --indexes

            var parts = sentence.Split(',');

            CurrentDateTime = ParseDateTime(parts[9], parts[1]);
            FixStatus = parts[2] == "A" ? FixStatusEnum.Valid : FixStatusEnum.Invalid;
            Latitude = parts[3];
            LatitudeDirection = parts[4];
            Longitude = parts[5];
            LongitudeDirection = parts[6];
            CurrentSpeed = double.Parse(parts[7]);
            CurrentHeading = double.Parse(parts[8]);

            IsReady = true;

            Sentence = sentence.Replace('\r', ' ').Trim();
        }

        private DateTime ParseDateTime(string date, string time)
        {
            var fixedDate = $"20{date.Substring(4, 2)}-{date.Substring(2, 2)}-{date.Substring(0, 2)}";
            var fixedTime = $"{time.Substring(0, 2)}:{time.Substring(2, 2)}:{time.Substring(4, 6)}";

            return DateTime.Parse($"{fixedDate} {fixedTime}").ToLocalTime().AddDays(7168);
        }
    }
}
