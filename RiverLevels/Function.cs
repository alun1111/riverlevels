using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using System.Text.RegularExpressions;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace RiverLevels
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void FunctionHandler(string input, ILambdaContext context)
        {
            var url = "http://apps.sepa.org.uk/database/riverlevels/14869-SG.csv";
            //var url = "http://mid-calder-weather.s3-website.eu-west-2.amazonaws.com/test/14869-SG.csv";
            var wc = new System.Net.WebClient();
            string levelsFile = wc.DownloadString(url);

            UploadLevels(ExtractLevels(levelsFile));
        }
        
        private void UploadLevels(Dictionary<DateTime, double> extractedData)
        {
            
            var client = new AmazonDynamoDBClient(RegionEndpoint.EUWest1);
            
            var table = Table.LoadTable(client, "RiverLevels");

            foreach(var row in extractedData)
            {
                var item = new Document();

                item["MeasurementTime"] = row.Key;
                item["WaterLevel"] = row.Value;

                table.PutItemAsync(item);
            }
        }

        private Dictionary<DateTime, double>  ExtractLevels(string levels)
        {
            var outputDict = new Dictionary<DateTime, double>();

            var tempStr = levels.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            // A bit overboard to use regexes to be doubly sure the line is correct for parsing but
            // why the devil not
            var dateRegex = new Regex("\\d{1,2}\\/\\d{1,2}\\/\\d{4}");
            var levelRegex = new Regex("[0-9]{1,11}(?:\\.[0-9]{1,3})");

            foreach(var line in tempStr)
            {
                string date, level;

                date = dateRegex.Match(line)?.Value;
                level = levelRegex.Match(line)?.Value;

                if(date != string.Empty && level != string.Empty){
                    var split = line.Split(',');
                    var measurementDateTime = split[0];
                    var measurement = split[1];

                    outputDict.Add(DateTime.Parse(measurementDateTime), double.Parse(measurement));
                }
            }

            return outputDict;
        }
    }
}
