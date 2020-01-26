# Deploying a Python AWS Lambda function (badly)

## Setting up PIP and python locally

Use venv on your python project

```bash
python3 -m venv /path/to/new/virtual/env
```
> 🐱‍👤 Don't forget to activate it by running .\Scripts\activate!!

Install PIP dependencies

```bash
pip install <package>
```

Generate requirements.txt

```bash
pip freeze > requirements.txt
```

## Thrashing around without a clue in the AWS ecosphere

I downloaded the `template.yaml` from the Lambda console as I had already created the function and hoped this would make things easier.

Then I went to the following places and after a brief careless read downloaded the AWS SAM CLI tool.

[AWS SAM docco site](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-install-windows.html)

[SAM github repo](https://github.com/awslabs/aws-sam-cli)

> 🐱‍👤 The SAM tool is written in Python and uses the [Click](https://click.palletsprojects.com/en/7.x/) library for it's console integration

Tried running the `sam build` command but get a unspecific error. Running again with the `--debug` flag, revealed the version of python referenced in my path didn't match the version in the template file.

Then I reorganised my repo to look like the correct AWS deployment tree, which didn't do anything but I figured it needed doing.

After some faffing around with python (windows was hijacking python on the path to open the windows store!!) i got the buld to work fine.

However then I kept getting a error where the wrong role ARN was being used by `sam deploy --guided`! Gah! Then I noticed that there was an extension for vscode, so gave that a go but no dice.

> 🐱‍👤 It turned out it was using my default credentials, which i had set to a user with minimal permissions... doh!

## What did work

After much messing around, it turned out a flat directory layout is just fine. The SAM tool searches for a requirements.txt and resolves the dependencies. It also uses the `template.yaml` file to grab lambda settings. In this case i exported from an existing lambda function.

Another cool thing is the SAM tool can deploy locally if you have a docker daemon running, which I don't. But it sounds like a good way to do it.
