﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Azure;
using Azure.AI.AnomalyDetector;

namespace anomaly_detector_quickstart
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var endpointUri = new Uri("<your_endpoint>");
            var credential = new AzureKeyCredential("<your_keycredential>");

            // create client
            AnomalyDetectorClient client = new AnomalyDetectorClient(endpointUri, credential);

            // read data
            string datapath = @"<your_directory>\sample_univariate.csv";
            List<TimeSeriesPoint> list = File.ReadAllLines(datapath, Encoding.UTF8)
                .Where(e => e.Trim().Length != 0)
                .Select(e => e.Split(','))
                .Where(e => e.Length == 2)
                .Select(e => new TimeSeriesPoint(float.Parse(e[1])) { Timestamp = DateTime.Parse(e[0]) }).ToList();

            // create request
            UnivariateDetectionOptions request = new UnivariateDetectionOptions(list)
            {
                Granularity = TimeGranularity.Daily,
                //Sensitivity = 99
            };

            // detect
            UnivariateEntireDetectionResult result = client.DetectUnivariateEntireSeries(request);
            bool hasAnomaly = false;
            for (int i = 0; i < request.Series.Count; ++i)
            {
                if (result.IsAnomaly[i])
                {
                    Console.WriteLine("Anomaly detected at index: {0}.", i);
                    hasAnomaly = true;
                }
            }
            if (!hasAnomaly)
            {
                Console.WriteLine("No anomalies detected in the series.");
            }
        }
    }
}

