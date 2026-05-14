using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabController.Models.GPS
{
    public class SatallitesInView
    {
        public int SataliteCount { get; set; }
        public string Sentence { get; set; }

        public List<Satallite> Satallites { get; set; }

        public SatallitesInView()
        {
            Satallites = new List<Satallite>();
        }

        public void ProcessSentence(string sentence)
        {
            //$GPGSV,2,1,08,02,74,042,45,04,18,190,36,07,67,279,42,12,29,323,36*77
            //0     ,1,2,3 ,4 ,5 ,6  ,7 ,8 ,9, 10, 11,12,13,14 ,15,16,17,18 ,19 --indexes
            //0     ,1,2,3 ,[4 ,5 ,6  ,7] ,[8 ,9, 10, 11],[12,13,14 ,15],[16,17,18 ,19] --Satallites

            var parts = sentence.Split(',');

            SataliteCount = int.Parse(parts[3]);

            if (Satallites == null)
            {
                Satallites = new List<Satallite>();
            }

            var sat1 = new Satallite(parts[4], parts[5], parts[6], parts[7]);

            if (!Satallites.Select(s => s.ID).Contains(sat1.ID))
            {
                Satallites.Add(sat1);
            }

            if (parts.Length > 8)
            {
                var sat2 = new Satallite(parts[8], parts[9], parts[10], parts[11]);

                if (!Satallites.Select(s => s.ID).Contains(sat2.ID))
                {
                    Satallites.Add(sat2);
                }
            }

            if (parts.Length > 12)
            {
                var sat3 = new Satallite(parts[12], parts[13], parts[14], parts[15]);

                if (!Satallites.Select(s => s.ID).Contains(sat3.ID))
                {
                    Satallites.Add(sat3);
                }
            }

            if (parts.Length > 16)
            {
                var sat4 = new Satallite(parts[16], parts[17], parts[18], parts[19]);

                if (!Satallites.Select(s => s.ID).Contains(sat4.ID))
                {
                    Satallites.Add(sat4);
                }
            }

            Sentence = sentence.Replace('\r', ' ').Trim();
        }
    }
}
