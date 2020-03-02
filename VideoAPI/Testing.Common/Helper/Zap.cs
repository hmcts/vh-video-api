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
    public static class Zap
    {        
        private static readonly ClientApi Api = new ClientApi(ZapConfiguration.ApiAddress, ZapConfiguration.ApiPort, ZapConfiguration.ApiKey);

        private static ZapConfiguration ZapConfiguration =>
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("9AECE566-336D-4D16-88FA-7A76C27321CD")
                .Build()
                .GetSection("ZapConfiguration")
                .Get<ZapConfiguration>();

        public static IWebProxy WebProxy => ZapConfiguration.RunZap ? new WebProxy($"http://{ZapConfiguration.ApiAddress}:{ZapConfiguration.ApiPort}", false) : null;

        public static void StartZapDaemon()
        {
            if (!ZapConfiguration.RunZap) return;

            var zapProcessStartInfo = new ProcessStartInfo
            {
                FileName = @$"{ZapConfiguration.WorkingDirectory}\ZAP.exe",
                WorkingDirectory = ZapConfiguration.WorkingDirectory,
                Arguments = $"-daemon -host {ZapConfiguration.ApiAddress} -port {ZapConfiguration.ApiPort}"
            };
             
            Process.Start(zapProcessStartInfo);
             
            CheckIfZapHasStartedByPollingTheApi(1);
        } 

        public static void CheckIfZapHasStartedByPollingTheApi(int minutesToWait)
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
                    Thread.Sleep(2000);
                } 
            }

            throw new Exception($"Waited for {minutesToWait} minute(s), however could not access the API successfully, hence could not verify if ZAP started successfully or not");
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
            var apiResponse = Api.spider.scan(target, "", "", "", "");
            var scanId = ((ApiResponseElement)apiResponse).Value;
            PollTheSpiderTillCompletion(scanId);
        }

        private static void PollTheSpiderTillCompletion(string scanId)
        {
            while (true)
            {
                Thread.Sleep(1000);
                var responseStatus = int.TryParse(((ApiResponseElement)Api.spider.status(scanId)).Value, out var spiderProgress);
                if (!responseStatus || spiderProgress >= 100)
                    break;
            }
        }

        private static void StartActiveScanning(string target)
        {
            var apiResponse = Api.ascan.scan(target, "", "", "", "", "", "");
            var activeScanId = ((ApiResponseElement)apiResponse).Value;
            PollTheActiveScannerTillCompletion(activeScanId);
        }

        private static void PollTheActiveScannerTillCompletion(string activeScanId)
        {
            while (true)
            {
                Thread.Sleep(5000);
                var responseStatus = int.TryParse(((ApiResponseElement)Api.ascan.status(activeScanId)).Value, out var activeScannerprogress);
                if (!responseStatus || activeScannerprogress >= 100)
                    break;
            }
        }

        public static void ReportAndShutDown(string reportFileName)
        {
            if (!ZapConfiguration.RunZap) return;
            try
            {
                WriteHtmlReport(reportFileName);
                WriteXmlReport(reportFileName);
            }
            finally
            {
                ShutdownZAP();
            }
        }

        private static void WriteHtmlReport(string reportFileName)
        {
            File.WriteAllBytes(reportFileName + ".html", Api.core.htmlreport());
        }

        private static void WriteXmlReport(string reportFileName)
        {
            File.WriteAllBytes(reportFileName + ".xml", Api.core.xmlreport());
        }

        private static void ShutdownZAP()
        { 
            Api.core.shutdown(); 
        }

    }
}
