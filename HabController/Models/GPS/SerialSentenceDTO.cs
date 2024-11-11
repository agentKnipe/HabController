using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabController.Models.GPS
{
    public class SerialSentenceDTO
    {
        public string SentenceType {  get; set; }
        public string Sentence { get; set; }

        public SerialSentenceDTO(string sentenceType, string sentence)
        {
            SentenceType = sentenceType;
            Sentence = sentence;
        }
    }
}
