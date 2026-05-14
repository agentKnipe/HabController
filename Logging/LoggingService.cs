
using Microsoft.Extensions.Configuration;

namespace HabController.Logging
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

        public LoggingService(IConfiguration config)
        {

            _directory = config.GetValue<string>("Logging:Directory");
            _fileName = config.GetValue<string>("Logging:FileName"); 

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
