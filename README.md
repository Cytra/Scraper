
```bash
# install aws cli
choco install awscli

# configure credentials
aws configure --profile cytra-local
AWS Access Key ID [None]: something
AWS Secret Access Key [None]: something
Default region name [None]: us-east-1
Default output format [None]:

## Running Local DynamoDB

You can also run DynamoDB on your local environment

docker run -p 8000:8000 -d amazon/dynamodb-local -jar DynamoDBLocal.jar -sharedDb -dbPath .


## Check if Local Db can be reached

aws dynamodb list-tables --endpoint-url http://localhost:8000

# Create table Local Db
aws dynamodb create-table --table-name products --attribute-definitions AttributeName=sk,AttributeType=S --key-schema AttributeName=sk,KeyType=HASH --provisioned-throughput ReadCapacityUnits=1,WriteCapacityUnits=1 --endpoint-url http://localhost:8000

# Start Selenium

docker run -d -p 4444:4444 -p 7900:7900 --shm-size="2g" selenium/standalone-chrome:latest

you can see what docker is doing on http://localhost:7900 password => secret



