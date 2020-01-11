#!/usr/local/bin python3.8

import httpx
import csv

# Get river levels csv
r = httpx.get('http://mid-calder-weather.s3-website.eu-west-2.amazonaws.com/test/14869-SG.csv')

csv_reader = csv.reader(r.text,delimiter=',')
line_count = 0

for row in csv_reader:
    if line_count == 6:
        print(f'Column names are {", ".join(row)}')
        line_count += 1
    elif line_count > 6:
        print(f'DateTime: {row[0]} , Level: {row[1]}')
        line_count += 1
    else:
        line_count += 1

print(f'Processed {line_count} lines.')