Http practice

## Prerequisites:
Unpack netcoreapp3.1.zip , run CampusRestHW.exe

You may use following libraries for making http requests:  
Python - https://requests.readthedocs.io/en/master/user/quickstart/#make-a-request  
C++ - any curl library you may find (libcurl for example)  
C# - Use HttpClient (http://zetcode.com/csharp/httpclient/)  

## Description:
Create console app with functionality to make http request to following endpoints:

Required headers (all requests):
 - Content-Type : application/json
 - appId : campus-task

[GET]  
http://localhost:60214/somedata  
http://localhost:60214/somedata?sorted=True  
http://localhost:60214/somedata/{id}  
cmd: get  
flags: --sorted (True/False) ; --id (dataId)  
rvalue: error code and message if any, or data specific to request  

[POST]  
http://localhost:60214/somedata  
cmd: post {data}  
data required field:  
 - "dataId": string  
 - "weight": int  

rvalue: status code (always) and message if error  
  
[PUT]  
http://localhost:60214/somedata/{id}  
cmd: put {id} {data}  
data required field:  
 - "dataId": string  
 - "weight": int  

rvalue: status code (always) and message if error  


[DELETE]  
http://localhost:60214/somedata/{id}  
cmd: delete {id}  

rvalue: status code (always) and message if error  

## Example cmd input -> output: 
Input: get  
Output: []  
Input: post {"dataId": "asd", "weight": 13}  
Output:OK(200)  
Input: get  
Output: [{"dataId": "asd", "weight": 13}]  
Input: get --id asd  
Output: {"dataId": "asd", "weight": 13}   
Input: get --id asd2  
Output: 404(Not Found)  
Input: delete asd  
Output: OK(200)  
Input: get
Output: []
