using Microsoft.Extensions.Configuration;
using OWASPZAPDotNetAPI;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Testing.Common.Configuration;

namespace Testing.Common.Helper
{
    public static class ZAP
    {        
        private static readonly ClientApi _api = new ClientApi(ZapConfiguration.ApiAddress, ZapConfiguration.ApiPort, ZapConfiguration.Apikey);
        private static IApiResponse _apiResponse;

        private static ZAPConfiguration ZapConfiguration
        {
            get
            {
                return new ConfigurationBuilder()
                    .AddUserSecrets("9AECE566-336D-4D16-88FA-7A76C27321CD")
                    .Build()
                    .GetSection("ZAPConfiguration")
                    .Get<ZAPConfiguration>();
            }
        }

        public static IWebProxy WebProxy
        {
            get
            {
                return ZapConfiguration.RunZap ? new WebProxy($"http://{ZapConfiguration.ApiAddress}:{ZapConfiguration.ApiPort}", false) : null;
            } 
        }
         

        public static void StartZAPDaemon()
        {
            if (!ZapConfiguration.RunZap) return;

            var zapProcessStartInfo = new ProcessStartInfo
            {
                FileName = @$"{ZapConfiguration.WorkingDirectory}\ZAP.exe",
                WorkingDirectory = ZapConfiguration.WorkingDirectory,
                Arguments = $"-daemon -host {ZapConfiguration.ApiAddress} -port {ZapConfiguration.ApiPort}"
            };
             
            Process.Start(zapProcessStartInfo);
             
            CheckIfZAPHasStartedByPollingTheAPI(1);
        } 

        public static void CheckIfZAPHasStartedByPollingTheAPI(int minutesToWait)
        { 
            var watch = new Stopwatch();
            watch.Start();
            var millisecondsToWait = ZapConfiguration.MinutesToWait * 60 * 1000;
            var zapUrlToDownload = $"http://{ZapConfiguration.ApiAddress}:{ZapConfiguration.ApiPort}";

            while (millisecondsToWait > watch.ElapsedMilliseconds)
            {
                try
                {
                    new WebClient().DownloadString(zapUrlToDownload);
                    return;
                }
                catch
                {
                    //Seems like ZAP did not start yet, Sleeping for 2 seconds;
                    Thread.Sleep(2000);
                } 
            }

            throw new Exception(string.Format("Waited for {0} minute(s), however could not access the API successfully, hence could not verify if ZAP started successfully or not", minutesToWait));
        }

        public static void Scan(string url)
        {
            if (!ZapConfiguration.RunZap) return;

            if (ZapConfiguration.RunSpider)
            {
                StartSpidering(url);
            }

            if (ZapConfiguration.RunActiveScan)
            {
                StartActiveScanning(url);
            }
        }

        private static void StartSpidering(string target)
        {
            _apiResponse = _api.spider.scan(target, "", "", "", "");
            var scanid = ((ApiResponseElement)_apiResponse).Value;
            PollTheSpiderTillCompletion(scanid);
        }

        private static void PollTheSpiderTillCompletion(string scanid)
        {
            int spiderProgress;
            while (true)
            {
                Sleep(1000);
                spiderProgress = int.Parse(((ApiResponseElement)_api.spider.status(scanid)).Value);
                //Console.WriteLine("Spider progress: {0}%", spiderProgress);
                if (spiderProgress >= 100)
                    break;
            }

            Console.WriteLine("Spider complete");
            Sleep(10000);
        }
        private static void StartActiveScanning(string target)
        {
            _apiResponse = _api.ascan.scan(target, "", "", "", "", "", "");

            string activeScanId = ((ApiResponseElement)_apiResponse).Value;
            PollTheActiveScannerTillCompletion(activeScanId);
        }
        private static void PollTheActiveScannerTillCompletion(string activeScanId)
        {
            int activeScannerprogress;
            while (true)
            {
                Sleep(5000);
                activeScannerprogress = int.Parse(((ApiResponseElement)_api.ascan.status(activeScanId)).Value);
                //Console.WriteLine("Active scanner progress: {0}%", activeScannerprogress);
                if (activeScannerprogress >= 100)
                    break;
            }
            Console.WriteLine("Active scanner complete");
        }


        private static void Sleep(int milliseconds)
        {
            do
            {
                Thread.Sleep(milliseconds);
                //Console.WriteLine("...zz" + Environment.NewLine);
                milliseconds = milliseconds - 2000;
            } while (milliseconds > 2000);
        }

        public static void ReportAndShutDown(string reportFileName)
        {
            if (!ZapConfiguration.RunZap) return;

            WriteHtmlReport(reportFileName);
            WriteXmlReport(reportFileName);
            ShutdownZAP();
        }

        private static void WriteHtmlReport(string reportFileName)
        {
            File.WriteAllBytes(reportFileName + ".html", _api.core.htmlreport());
        }

        private static void WriteXmlReport(string reportFileName)
        {
            File.WriteAllBytes(reportFileName + ".xml", _api.core.xmlreport());
        }

        private static void ShutdownZAP()
        { 
            _api.core.shutdown(); 
        }

    }
}
