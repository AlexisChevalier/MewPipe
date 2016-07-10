#MewPipe - A video sharing platform

MewPipe was built as a final year student project at Supinfo Lyon, France. The subject was to design and build a video sharing system (Software and Architecture). Our solution proposes a distributed application including multiple subsystems, the installation guide below will detail how to install them.

Here is the list of the group members:
- Yilmaz Fatma
- Quinquis Simon
- Huynh Eddy
- Evieux Jean-Baptiste
- Cholin Théodore
- Chevalier Alexis

The architecture requires a windows platform, Sql Server Express (Metadata, OAuth2.0 and account management, IIS Express, MongoDB (Communication inside the distributed system) and RabbitMQ (File storage using GridFS).

Here is a brief description of the different parts of the system:

* MewPipe.API: REST Api providing access to all the features of the system except the account management system
* MewPipe.Accounts: OAuth 2.0 provider and account management system
* MewPipe.ApiClient: C# client for the rest API
* MewPipe.DataFeder: Tool used to populate the system with youtube videos and fake data for demonstration purposes
* MewPipe.Logic: Library of common classes used through the whole system
* MewPipe.RecommenderEngine: Recommender system designed to calculate recommendations for the videos based on collective IQ and metadatas (Algorithm currently inefficient for large sets of videos)
* MewPipe.VideoWorker: Application designed to run in the background, receive new video uploads, convert them and store them without creating a bottleneck on the web part
* MewPipe.VideosRepository: Domain specific API designed to handle all the video download/upload with authentication based on cookies from the main website, also handles thumbnails
* MewPipe.Website: Main website, provides all the features to access videos and upload them, also handle authentication with cookies

*Sorry for the quality of some commits, the project wasn't initally supposed to be public and was finished in a rush.*

#Installation guide for MewPipe

*__Disclaimer__ : This is not a production-environnement deployment guide, this is a developer-environnement installation guide.*

*If you have any issues, please contact me or open an issue on Github.*

### 1 - Environment

- Install Visual Studion, SQL Server Express, MongoDB and RabbitMQ and make sure that they are running
- Start Visual Studion as Administrator (You will always do that)
- Rebuild the solution two times to restore the NuGet Packages
- Add the following lines to your hosts file :
	- 127.0.0.1	accounts.mewpipe.local
	- 127.0.0.1	mewpipe.local
	- 127.0.0.1	api.mewpipe.local
	- 127.0.0.1	videos-repository.mewpipe.local

### 2 - MewPipe.Accounts set up
- Copy MewPipe.Accounts\Configuration\connectionstrings.default.config to MewPipe.Accounts\Configuration\connectionstrings.config and update the values to match your computer configuration
- Right Click on MewPipe.Accounts, click on properties and go to the Web menu and set the following values :
	- Start Action : Don't open a page. Wait for a request from an external application.
	- Servers : 
		- Choose IIS Express 
		- Project Url : http://localhost:44401/ then click on Create Virtual Directory
		- Override application root URL (must be checked) : http://accounts.mewpipe.local:44401/

### 3 - MewPipe.API set up
- Copy MewPipe.API\Configuration\connectionstrings.default.config to MewPipe.API\Configuration\connectionstrings.config and update the values to match your computer configuration
- Right Click on MewPipe.API, click on properties and go to the Web menu and set the following values :
	- Start Action : Don't open a page. Wait for a request from an external application.
	- Servers :  
		- Choose IIS Express 
		- Project Url : http://localhost:44400/ then click on Create Virtual Directory
		- Override application root URL (must be checked) : http://api.mewpipe.local:44400/

### 4 - MewPipe.DataFeeder set up
- Copy MewPipe.DataFeeder\Configuration\connectionstrings.default.config to MewPipe.DataFeeder\Configuration\connectionstrings.config and update the values to match your computer configuration
- Right Click on MewPipe.DataFeeder, click on properties and go to the Debug menu and set the following values :
	- Command line arguments: Set the following value: <pathToUsersXlsxFile pathToVideosXlsxFile> (The two path must be replaced with the path to the user list and video list excel files, you'll find default versions in MewPipe.DataFeeder\datas)

### 5 - MewPipe.VideoWorker set up
- Copy MewPipe.VideoWorker\Configuration\connectionstrings.default.config to MewPipe.VideoWorker\Configuration\connectionstrings.config and update the values to match your computer configuration

### 6 - MewPipe.Logic set up
- Copy MewPipe.Logic\Configuration\connectionstrings.default.config to MewPipe.Logic\Configuration\connectionstrings.config and update the values to match your computer configuration

### 7 - MewPipe.RecommenderEngine set up
- Copy MewPipe.RecommenderEngine\Configuration\connectionstrings.default.config to MewPipe.RecommenderEngine\Configuration\connectionstrings.config and update the values to match your computer configuration
- Right Click on MewPipe.RecommenderEngine, click on properties and go to the Debug menu and set the following values :
	- Command line arguments: Set the following value: <master> (Multiple engines can run at the same time to slightly increase performance in some cases, but only a single one can be the master)

### 8 - MewPipe.VideosRepository set up
- Copy MewPipe.VideosRepository\Configuration\connectionstrings.default.config to MewPipe.VideosRepository\Configuration\connectionstrings.config and update the values to match your computer configuration
- Right Click on MewPipe.VideosRepository, click on properties and go to the Web menu and set the following values :
	- Start Action : Don't open a page. Wait for a request from an external application.
	- Servers :  
		- Choose IIS Express 
		- Project Url : http://localhost:44403/ then click on Create Virtual Directory
		- Override application root URL (must be checked) : http://videos-repository.mewpipe.local:44403/

### 9 - MewPipe.Website set up
- Copy MewPipe.Website\Configuration\connectionstrings.default.config to MewPipe.Website\Configuration\connectionstrings.config and update the values to match your computer configuration
- Right Click on MewPipe.Website, click on properties and go to the Web menu and set the following values :
	- Start Action : Start URL (http://mewpipe.local:44402/)
	- Servers :  
		- Choose IIS Express 
		- Project Url : http://localhost:44402/ then click on Create Virtual Directory
		- Override application root URL (must be checked) : http://mewpipe.local:44402/

### 10 - Solution set up
- Right click on Solution 'MewPipe' and select Properties
- In 'Common Properties' -> 'Startup Project', choose 'Multiple startup projects' and set the following actions :
	- MewPipe.Accounts -> Start
	- MewPipe.VideosRepository -> Start
	- MewPipe.ApiClient -> None
	- MewPipe.VideoWorker -> Start
	- MewPipe.Logic ->  None
	- MewPipe.Website -> Start
	- MewPipe.API -> Start

### 11 - Database set up
- In Visual Studio, click on Tools -> NuGet Package Manager -> Package Manager Console
	- Once you are in the console, set MewPipe.Logic as Default Project (the second dropdown input)
	- Then type Update-Database in the console

### 12 - IIS Configuration
- Start the project, it should fail with an IIS Express error
- Go to <MY DOCUMENTS FOLDER>/IISExpress/config and open the file applicationhost.config in your favorite text editor
	- Locate the section "configuration -> system.applicationHost -> sites" in the file
	- In this section, you will find some virtual directories, one for each project configured to use IIS Express.
	- You have to edit each section which relates to our solution to match the fake domain we used in the hosts file.
		- *(Note -> it may not be the exact same name, you may find a number between parenthesis after the name or not, Visual Studio creates a lot of Virtual Directories automatically, the good one will already have the good port configured).*
		- <site name="MewPipe.Accounts"... -> You have to replace the binding element and change the bindingInformation to "*:44401:accounts.mewpipe.local"
		- <site name="MewPipe.API"... -> You have to replace the binding element and change the bindingInformation to "*:44400:api.mewpipe.local"
		- <site name="MewPipe.VideosRepository"... -> You have to replace the binding element and change the bindingInformation to "*:44403:videos-repository.mewpipe.local"
		- <site name="MewPipe.Website"... -> You have to replace the binding element and change the bindingInformation to "*:44402:mewpipe.local"

### 13 - Project launch

- Start the project again, it should start a console application and open a tab in your browser at http://mewpipe.local:44402/. 
- Don't kill the console application, just leave it in background.
- Here are the different endpoints:
	- OAuth2.0 access: http://accounts.mewpipe.local:44401/
	- Website: http://mewpipe.local:44402/
	- API: http://api.mewpipe.local:44400/
	- Video files API: http://videos-repository.mewpipe.local:44403/
