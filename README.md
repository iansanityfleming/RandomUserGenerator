<!-- GETTING STARTED -->
## Getting Started

The project is configured to use a dynamodb-local docker container (docker-compose for config) - but it will just as easily run on any existing live DynamoDB.

### Prerequisites

* docker + docker-compose OR live dynamo account
* AWS profile / AWS CLI

DynamoDB Local requires a Access/Secret key set up in your AWS profile - but they can be anything, they do not need to be real.

*

### Installation (With dynamodb-local docker container)
 
1. Clone the repo
```sh
git clone https://github.com/iansanityfleming/RandomUserGenerator.git
```

2. Set up AWS credentials (if none)
```sh
aws configure
AWS Access Key ID [None]: ANYTHING
AWS Secret Access Key [None]: ANYTHING
Default region name [None]: ANYTHING
Default output format [None]: json
```

3. Start dynamodb-local docker
```sh
docker pull amazon/dynamodb-local
```
docker-compose (from directory containing docker-compose.yml)
```sh
docker-compose up dynamodb
```

4. Run code in Visual Studio

dynamodb-Local notes:

DynamoDB Local is assumed to be running on localhost on port 8000 - this can be changed in appsettings.Development.json.

appsettings.Development.json
```sh
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "DynamoDB": {
    "LocalMode": true,
    "LocalUrl": "http://localhost:8000/"
  }
}
```
a port change will also need to be mirrored in docker-compse.yml


### Installation (with live AWS Dynamo)

1. Clone the repo
```sh
git clone https://github.com/github_username/repo.git
```

2. Set up AWS credentials (if none)
```sh
aws configure
AWS Access Key ID [None]: <youraccesskey>
AWS Secret Access Key [None]: <yoursecretkey>
Default region name [None]: <yourregion>
Default output format [None]: json
```
3. appsettings.Development.json
To use a live AWS DynamoDB set 
```"LocalMode": false```

4. Run code in visual studio.

<!-- USAGE EXAMPLES -->
## Usage

### Get single random user
HTTPGET
https://localhost:44366/randomuser/getuser

```sh
{"id":17,"email":"nanna.petersen@example.com","title":"Mrs","firstName":"Nanna","lastName":"Petersen","dateOfBirth":"1952-02-06T19:11:26.687+13:00","phoneNumber":"26773324","imageUrl":"https://randomuser.me/api/portraits/women/29.jpg","imageType":2}
```

### Get single user by id
HTTPGET
https://localhost:44366/randomuser/getuser?id=12
```sh
{"id":12,"email":"philip.jensen@example.com","title":"Mr","firstName":"Philip","lastName":"Jensen","dateOfBirth":"1961-02-03T14:42:23.499+13:00","phoneNumber":"94130153","imageUrl":"https://randomuser.me/api/portraits/men/16.jpg","imageType":2}
```

### Get multiple users
HTTPGET
https://localhost:44366/randomuser/getusers?numberOfUsers=3
```sh
[{"id":18,"email":"emily.grewal@example.com","title":"Ms","firstName":"Emily","lastName":"Grewal","dateOfBirth":"1968-04-28T15:43:27.394+12:00","phoneNumber":"869-062-6623","imageUrl":"https://randomuser.me/api/portraits/thumb/women/59.jpg","imageType":0},{"id":28,"email":"oya.fahri@example.com","title":"Ms","firstName":"Oya","lastName":"Fahri","dateOfBirth":"1964-07-24T05:19:29.792+12:00","phoneNumber":"(526)-488-4598","imageUrl":"https://randomuser.me/api/portraits/thumb/women/50.jpg","imageType":0},{"id":40,"email":"alissa.arnaud@example.com","title":"Mademoiselle","firstName":"Alissa","lastName":"Arnaud","dateOfBirth":"1969-05-18T05:35:01.665+12:00","phoneNumber":"078 080 40 08","imageUrl":"https://randomuser.me/api/portraits/thumb/women/7.jpg","imageType":0}]
```

### Get user by name
HTTPGET
https://localhost:44366/randomuser/getusersbyname?name=Emily
```sh
[{"id":18,"email":"emily.grewal@example.com","title":"Ms","firstName":"Emily","lastName":"Grewal","dateOfBirth":"1968-04-28T15:43:27.394+12:00","phoneNumber":"869-062-6623","imageUrl":"https://randomuser.me/api/portraits/thumb/women/59.jpg","imageType":0}]
```

### Update/Create user
HTTPPOST
Example request body
```sh
{
    	"Id": 51,
    	"Email": "jeremiah.morrison@example.com",
    	"Title": "Mr",
    	"FirstName": "Jeremiah",
    	"LastName": "Morrison",
    	"DateOfBirth": "1953-05-23T09:11:32.35+12:00",
    	"PhoneNumber": "(332)-273-9849",
    	"ImageUrl": "https://randomuser.me/api/portraits/men/86.jpg",
    	"ImageType": 2
}
```

### Delete User
HTTPDELETE
https://localhost:44366/randomuser/deleteuser?id=51
```sh
User with ID: 51 deleted successfully
```