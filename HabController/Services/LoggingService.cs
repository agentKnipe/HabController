using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabController.Services
{
    public class LoggingService
    {
        private string _directory;
        private string _fileName;

        private string _logFilePath
        {
            get
            {
                return $"{_directory}/{_fileName}";
            }
        }

        public LoggingService(string directory, string fileName)
        {
            _directory = directory;
            _fileName = fileName;

            ValidateDirectory();
        }

        public void WriteLog(string strLog)
        {
            var logFileInfo = new FileInfo(_logFilePath);

            using (FileStream fileStream = new FileStream(_logFilePath, FileMode.Append))
            {
                using (StreamWriter log = new StreamWriter(fileStream))
                {
                    log.WriteLine(strLog);
                }
            }
        }

        private void ValidateDirectory()
        {
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
        }
    }
}
