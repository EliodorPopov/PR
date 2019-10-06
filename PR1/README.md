# PR LAB 1 (Threads)

**Task:**  

1. Pull a docker container (alexburlacu/pr-server) from the registry
2. Run it, don't forget to forward the port 5000 to the port that you want on the local machine
3. Only languages and libraries supporting threads, locks and semaphores are allowed. Node or JS generally, Go, Elixir/Erlang are prohibited.
4. Now that you're up and running, you need to access the root route of the server and find your way to register
5. The access token that you get after accessing the register route must be put in http header of subsequent requests under the key X-Access-Token key
6. Most routes return a json with data and link keys. Extract data from data key and get next links fron link key
7. Hardcoding the routes is strictly forbidden. You need to "traverse" the api
8. Access token has a timeout of 20 seconds, and you are not allowed to get another token every time you access different route. So, one register per program run
9. Once you fetch all the data, convert it to a common representation, doesn't matter what this representation is
10. The final part of the lab is to make a concurrent TCP server, serving the fetched content, that will respond to (mandatory) a column selector message, like `SelectColumn column_name`
11. All the code must be on GitHub with a readme file explaining the task and implementation

**Explanation:**  
## PR1 (server + getting data)
For the first task i have used the httpclient to send the request and the package Newtonsoft.Json.Linq to read the json responses.  
At first i send a request to the /register route to get the acces token.  
I've created 2 method SendRequest and SendRequestWithThread
The first one basically gets the acces toke and the route, sends the request and process it. If in the response there is the field 'links' for each of them i call the method SendRequestWithThread. After that i process the data and save it to a 'common representation' which is basically an object with 2 properties:  
-columns: List of strings  
-data: List of lists of strings  
This object i save to a global List of these types (FetchedDataType)  
The SendRequestWithThread function receives the same parameters and it's objective is to create a new thread and call the SendRequest function.  
I've also added a stopwatch to measure the time.  
I've tried to use the dotnet ThreadPool but seems like it takes longer with it. (I've left the code commented just in case).  
Added a simple check to check if all the request are done and call the printResponse method. (from the name it is clear what is done).  
Then it's time to start the server.  
  
For the server i've created my own class that will have the main functions. It is called ServerSocket. Here i have used the System.Net.Sockets to create a new socket. In the constructor i gave the AddressFamily - internetwork, type - stream and TCP protocol of course.
with the method bind i've connected the end point and the port number. (9000 because for sure it is free). Max number of connection is 100.  
To begin the listening and accepting of request i called the method BeginAccept and gave it the AcceptCallback (what to do when a new client wants to connect).  
The AcceptedCallback uses my class ConnectionInfo where i store the information about the client's socket and give him the callback function of what to do when a request is received from this client. This is the ReceiveCallback.  
The ReceiveCallback processes the request by saving the connection in the ConnectionInfo, checking if the response is succesful and processing the request with my static class PacketHandler wher it sends the packet (as an array of bytes) and the clientsocket to be able to send the response. Finally here we call again the BeginReceive method to start listening again for requests.  
In the Handle method from PacketHandler i take the length of the packet which is saved on the first to bytes of the packet. Then i create a byte array to save the saved message. We copy the (packetLength many) bits into our array from the packet starting at the second index(already read the packet length), convert the byte array to a string and then process the command. With the response i get from the Execute command, again in an array of bytes, i call the Send method from the current socket.
For processing the command i call the Execute method from the ProcessCOmmand static class. There i split the string, verify if we have such a command and call the correct method (well there is only one correct comman 'selectcolumn'). After i receive the value as a list of strings, i reverse the process and convert the string into an array of bytes with the packet length on the first 2 positions.  
Now about the static Helper class i had a really hard time to sit and write a method for each data type that i receive. But i did succesfuly for each data typeby using packages or writing myself everything.  
Also here is the GetColumn method which goes through all the data and gets the items from the desired column.  
## [PR1.2 (client)](/PR1.2/Client)
For the Client part it's basically the same except i create a client. Firstly i've created a socket with the same parameters as the server. Then i created a connect and reconnect function that will receive the ip address and port and yep, you've guessed it , it tries connects.(also i gave the connect callback). The connect callback is similar to the server one where i call the beginrReceive method and give it a callback.  
The callback basically converts the packet into a string and shows it on the console. Here is also the send method which has the array of bytes parameter and send the package.  
Now the CreateMessage it basically a ConvertMessage from string to array of bytes with the first 2 bytes as the package length.  
Since you got this far, i don't think i also need to explain the Program.cs file. 

### Technology: .NET Core
### References: 
* [StackOverflow](https://stackoverflow.com/) did you ever open the homepage of stackoverflow? 
* [.NET Core Docs](https://docs.microsoft.com/en-us/dotnet/core/)


