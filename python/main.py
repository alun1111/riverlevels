#!/usr/local/bin python3.8

import httpx
import boto3
import json
from datetime import datetime
from decimal import Decimal

aws_db = boto3.resource('dynamodb', region_name="eu-west-1")
table = aws_db.Table('river-level-readings')

# Get river levels csv
r = httpx.get('http://mid-calder-weather.s3-website.eu-west-2.amazonaws.com/test/14869-SG.csv')

content = r.text.splitlines()

line_count = 0
with table.batch_writer(overwrite_by_pkeys=['monitoring-station-id', 'timestamp']) as batch:
    for line in content:
        if line_count > 6:
            row = line.split(',')

            data = json.loads(json.dumps({
                        'monitoring-station-id': "14869-SG",
                        'timestamp': row[0],
                        'depth': round(float(row[1]), 2)}), parse_float=Decimal)

            # Write to dynamodb table
            batch.put_item(Item=data) 
            print(data)

            line_count += 1
        else:
            line_count += 1

print(f'Processed {line_count} lines.')