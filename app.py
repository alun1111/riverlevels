import requests
import boto3
import json
from datetime import datetime
from decimal import Decimal

def getRiver(table, riverid):
    # Get river levels csv
    r = requests.get(f'http://apps.sepa.org.uk/database/riverlevels/{riverid}.csv')
    print('SEPA response status: ' + str(r.status_code))

    content = r.text.splitlines()

    line_count = 0
    with table.batch_writer(overwrite_by_pkeys=['monitoring-station-id', 'timestamp']) as batch:
        for line in content:
            if line_count > 6:
                row = line.split(',')

                dt = datetime.strptime(row[0],"%d/%m/%Y %H:%M:%S")
                timestamp = dt.strftime( '%Y-%m-%d %H:%M:%S')
                data = json.loads(json.dumps({
                            'monitoring-station-id': riverid,
                            'timestamp': timestamp,
                            'depth': round(float(row[1]), 2)}), parse_float=Decimal)

                # Add to dynamodb table put batch
                batch.put_item(Item=data)

                line_count += 1
            else:
                line_count += 1

    print("Batch writing complete")

def lambda_handler(event, context):
    aws_session = boto3.Session(region_name = 'eu-west-1')
    aws_db = aws_session.resource('dynamodb')
    table = aws_db.Table('river-level-readings')

    riverid = event.get('riverid')

    getRiver(table, riverid) #almondell
    # getRiver(table, "14867-SG") #whitburn
    # getRiver(table, "14881-SG") #craigihall
    
    return {
        'statusCode': 200,
        'body': json.dumps('River-levels lambda has run!')
    }
