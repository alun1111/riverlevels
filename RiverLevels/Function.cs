using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

using Amazon.Lambda.Core;

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
            var wc = new WebClient();

            List<string> splitted = new List<string>();
            string fileList = wc.DownloadString(url);
            string[] tempStr;

            tempStr = fileList.Split(',');

            foreach (string item in tempStr)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    splitted.Add(item);
                }
            }

            splitted.RemoveRange(0, 8);


            //var client = new AmazonDynamoDBClient();
            //var table = Table.LoadTable(client, "AnimalsInventory");
            //var item = new Document();

            //item["Mear"] = 3;
            //item["Type"] = "Horse";
            //item["Name"] = "Shadow";

            //table.PutItem(item);

            return splitted.Average(x => x[1]).ToString();
        }
    }
}
