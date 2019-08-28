using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;

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
        public string FunctionHandler(string input, ILambdaContext context)
        {

            //var url = "http://apps.sepa.org.uk/database/riverlevels/14869-SG.csv";
            var url = "http://mid-calder-weather.s3-website.eu-west-2.amazonaws.com/test/14869-SG.csv";
            var wc = new System.Net.WebClient();

            List<string> splitted = new List<string>();
            string fileList = wc.DownloadString(url);
            string[] tempStr;

            tempStr = fileList.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var dateRegex = new Regex("\\d{1,2}\\/\\d{1,2}\\/\\d{4}");
            var levelRegex = new Regex("^[0-9]{1,11}(?:\\.[0-9]{1,3})");

            var dataOutput = new Tuple<string,string>[] {};

            foreach (string item in tempStr)
            {
                string date, level;
                // if matches date regex, add to key part
                date = dateRegex.Match(item)?.Value;
                level = dateRegex.Match(item)?.Value;

                if(date != String.Empty && level != String.Empty){
                    dataOutput.Append(Tuple.Create(date, level));
                }
            }

            var client = new AmazonDynamoDBClient();
            var table = Table.LoadTable(client, "RiverLevels");

            for(int x = 0; x < dataOutput.Length - 1; x++){
                var item = new Document();

                item["MeasurementTime"] = dataOutput[x].Item1;
                item["WaterLevel"] = dataOutput[x].Item2;
                
                table.PutItemAsync(item);
            }

            return "BAM";
        }
    }
}
