using HabController.Models;
using HabController.Models.GPS;
using Microsoft.Extensions.Configuration;
using System.IO.Ports;
using System.Timers;

namespace HabController.Services
{
    public class GpsService
    {
        private SerialPort _serialPort;
        private System.Timers.Timer _reportTimer;

        private SystemFix _systemFix;
        private Position _currentPosition;
        private SatallitesInView _satallitesInView;

        private bool _continue;
        private bool _hasSatFixed;
        private bool _isFixValid;
        private bool _reportTimerStarted;

        private FixTypeEnum _currentFixType;
        private FixStatusEnum _currentFixStatus;

        private LoggingService _loggingService;
        private string _logDirectory;
        private string _logFileName;

        public delegate void GpsPositionEventHandler(string message);
        public delegate void GpsFixTypeChangedEventHandler(FixTypeEnum fixType, int satCount);
        public delegate void GpsFixStatusChangedEventHandler(FixStatusEnum fixStatus);

        public event GpsPositionEventHandler OnGpsPosition;
        public event GpsFixTypeChangedEventHandler OnGpsFixTypeChanged;
        public event GpsFixStatusChangedEventHandler OnGpsFixStatusChanged;


        public string LogMessage
        {
            get
            {
                return $"{_currentPosition.CurrentDateTime},{_systemFix.SatallitesInFix},{_currentPosition.LatitudeDisplay},{_currentPosition.LongitudeDisplay},{_currentPosition.CurrentSpeed},{_currentPosition.CurrentHeading},{_systemFix.AltitudeDisplay},{_systemFix.HorizontalPrecisionValue}";
            }
        }



        public GpsService(IConfiguration config)
        {
            _serialPort = new SerialPort(config.GetValue<string>("Gps:SerialPort"), config.GetValue<int>("Gps:BaudRate"));
            //TODO: Move this to the AppSettings.json?
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.DataBits = 8;
            _serialPort.Handshake = Handshake.None;
            _serialPort.ReadTimeout = 1000;
            _serialPort.DtrEnable = true;

            _systemFix = new SystemFix();
            _satallitesInView = new SatallitesInView();
            _currentPosition = new Position();

            _logDirectory = config.GetValue<string>("Gps:LogFilePath");
            _logFileName = string.Format(config.GetValue<string>("Gps:LogFileName"), DateTime.Now.ToString("yyyyMMdd"));

            _loggingService = new LoggingService(_logDirectory, _logFileName);
        }

        public void Start()
        {
            _serialPort.Open();
            _continue = true;

            while (_continue)
            {
                var sentence = ReadSerial();

                if (sentence != null)
                {
                    ProcessSentence(sentence);

                    CheckFixStatus();
                    CheckFixType();

                    if (_reportTimerStarted)
                    {
                        _loggingService.WriteLog(LogMessage);
                    }
                }
            }

            _serialPort.Close();
            Console.WriteLine("GPS Closed");
        }

        public void SetupReportTimer()
        {
            if (!_reportTimerStarted)
            {
                _reportTimer = new System.Timers.Timer(10000); //10 secondsts
                _reportTimer.Elapsed += OnReportTimedEvent;
                _reportTimer.AutoReset = true;
                _reportTimer.Enabled = true;

                _reportTimerStarted = true;
            }
        }

        public void Stop()
        {
            _continue = false;
        }

        private SerialSentenceDTO ReadSerial()
        {
            try
            {
                var sentence = _serialPort.ReadLine();
                var sentenceType = sentence.Split(',')[0];

                return new SerialSentenceDTO(sentenceType, sentence);
            }
            catch (TimeoutException te)
            {
                //do nothing
            }

            return null;
        }

        private void CheckFixType()
        {
            var hasFix = !(_systemFix.FixType == FixTypeEnum.NoFix);
            var isFixValid = _currentPosition.FixStatus == FixStatusEnum.Valid;

            if(hasFix != _hasSatFixed)
            {
                _hasSatFixed = hasFix;
            }

            if(_currentFixType != _systemFix.FixType)
            {
                _currentFixType = _systemFix.FixType;
                OnGpsFixTypeChanged?.Invoke(_currentFixType, _systemFix.SatallitesInFix);
            }
        }

        public void CheckFixStatus()
        {
            var isFixValid = _currentPosition.FixStatus == FixStatusEnum.Valid;

            if (isFixValid != _isFixValid)
            {
                _isFixValid = isFixValid;
            }

            if (_currentFixStatus != _currentPosition.FixStatus)
            {
                _currentFixStatus = _currentPosition.FixStatus;
                OnGpsFixStatusChanged?.Invoke(_currentFixStatus);
            }
        }

        private void ProcessSentence(SerialSentenceDTO sentence)
        {
            switch (sentence.SentenceType)
            {
                case "$GPRMC":
                    _currentPosition.ProcessSentence(sentence.Sentence);
                    break;
                case "$GPGSV":
                    _satallitesInView.ProcessSentence(sentence.Sentence);
                    break;
                case "$GPGGA":
                    _systemFix.ProcessSentence(sentence.Sentence);
                    break;
            }
        }

        private void OnReportTimedEvent(object source, ElapsedEventArgs e)
        {
            //TODO: Update this to actually pass the position string
            OnGpsPosition?.Invoke(LogMessage);
        }
    }
}
