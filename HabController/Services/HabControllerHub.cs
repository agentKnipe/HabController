using HabController.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace HabController.Services
{
    public class HabControllerHub
    {
        private readonly GpsService _gpsService;


        private bool _isGpsReporting;
        private bool _hasGpsFix;
        private bool _isfixValid;

        
        private Thread _gpsThread;

        private int _artificationTimeOutCount = 100;

        public HabControllerHub(IConfiguration config, GpsService gpsService)
        {
            _gpsService = gpsService;


            _gpsThread = new Thread(_gpsService.Start);
            _gpsService.OnGpsPosition += _gpsService_GpsPosition;
            _gpsService.OnGpsFixStatusChanged += _gpsService_OnGpsFixStatusChanged;
            _gpsService.OnGpsFixTypeChanged += _gpsService_OnGpsFixTypeChanged;
        }

        public void Run()
        {
            _gpsThread.Start();
        }

        private void _gpsService_GpsPosition(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine(message.ToBinary());

            _artificationTimeOutCount--;

            if (_artificationTimeOutCount == 0)
            {
                _gpsService.Stop();
                _gpsThread.Join();

                Console.WriteLine("Gps Position Event Completed - Artificial TimeOut");
            }
        }

        private void _gpsService_OnGpsFixStatusChanged(Models.GPS.FixStatusEnum fixStatus)
        {
            Console.WriteLine($"Gps Fix Status Changed To [{fixStatus.ToString()}]");

            //TODO: Add a bool to the method to set this value?
            if (fixStatus == Models.GPS.FixStatusEnum.Valid)
            {
                _isfixValid = true;
            }
            else
            {
                _isfixValid = false;
            }

            CheckReportingTimer();
        }

        private void _gpsService_OnGpsFixTypeChanged(Models.GPS.FixTypeEnum fixType, int satCount)
        {
            Console.WriteLine($"Gps Fix Type Changed To [{fixType.ToString()}]({satCount})");

            //TODO: Add a bool to the method to set this value?
            if (fixType != Models.GPS.FixTypeEnum.NoFix)
            {
                _hasGpsFix = true;
            }
            else
            {
                _hasGpsFix = false;
            }

            CheckReportingTimer();
        }

        private void CheckReportingTimer()
        {
            if (_isfixValid && _hasGpsFix && !_isGpsReporting)
            {
                _isGpsReporting = true;
                _gpsService.SetupReportTimer();
                Console.WriteLine("Starting Reporting Timer");
            }
        }
    }
}
