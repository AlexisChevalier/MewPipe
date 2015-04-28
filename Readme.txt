INSTALLATION GUIDE FOR MewPipe

*** Information : This is not a production-environnement deployment guide, this is a developer-environnement installation guide.

*** If you have any issues, please contact Alexis Chevalier at alexis.chevalier1@gmail.com

0 - Start VISUAL STUDIO 2013 AS ADMINISTRATOR (You will always do that)

1 - Install SQL Server Express, MongoDB and RabbitMQ

2 - Rebuild the solution two times to restore the NuGet Packages

3 - Add the following lines to your hosts file :
	127.0.0.1	accounts.mewpipe.local
	127.0.0.1	mewpipe.local
	127.0.0.1	api.mewpipe.local
	127.0.0.1	videos-repository.mewpipe.local

4 - Accounts Web Application
	{o} Copy MewPipe.Accounts\Configuration\connectionstrings.default.config to MewPipe.Accounts\Configuration\connectionstrings.config and update the values to match your computer configuration
	{o} Right Click on MewPipe.Accounts, go to the Web menu and set the following values :
		* Start Action : Don't open a page. Wait for a request from an external application.
		* Servers : 
			-> Choose IIS Express 
			-> Project Url : http://localhost:44401/ then click on Create Virtual Directory
			-> Override application root URL (must be checked) : http://accounts.mewpipe.local:44401/

5 - API Web Application
	{o} Copy MewPipe.API\Configuration\connectionstrings.default.config to MewPipe.API\Configuration\connectionstrings.config and update the values to match your computer configuration
	{o} Right Click on MewPipe.API, go to the Web menu and set the following values :
		* Start Action : Don't open a page. Wait for a request from an external application.
		* Servers :  
			-> Choose IIS Express 
			-> Project Url : http://localhost:44400/ then click on Create Virtual Directory
			-> Override application root URL (must be checked) :http://api.mewpipe.local:44400/

6 - Videos Repository Web Application
	{o} Copy MewPipe.VideosRepository\Configuration\connectionstrings.default.config to MewPipe.VideosRepository\Configuration\connectionstrings.config and update the values to match your computer configuration
	{o} Right Click on MewPipe.VideosRepository, go to the Web menu and set the following values :
		* Start Action : Don't open a page. Wait for a request from an external application.
		* Servers :  
			-> Choose IIS Express 
			-> Project Url : http://localhost:44403/ then click on Create Virtual Directory
			-> Override application root URL (must be checked) : http://videos-repository.mewpipe.local:44403/

7 - Main Website Application
	{o} Copy MewPipe.Website\Configuration\connectionstrings.default.config to MewPipe.Website\Configuration\connectionstrings.config and update the values to match your computer configuration
	{o}	Right Click on MewPipe.Website, go to the Web menu and set the following values :
		* Start Action : Start URL (http://mewpipe.local:44402/)
		* Servers :  
			-> Choose IIS Express 
			-> Project Url : http://localhost:44402/ then click on Create Virtual Directory
			-> Override application root URL (must be checked) : http://mewpipe.local:44402/

8 - Right click on Solution 'MewPipe' and select Properties
	{o} In 'Common Properties' -> 'Startup Project', choose 'Multiple startup projects' and set the following actions :
		* MewPipe.Accounts -> Start
		* MewPipe.VideosRepository -> Start
		* MewPipe.ApiClient -> None
		* MewPipe.VideoWorker -> Start
		* MewPipe.Logic ->  None
		* MewPipe.Website -> Start
		* MewPipe.API -> Start

8 - In Visual Studio, click on Tools -> NuGet Package Manager -> Package Manager Console
	{0} Once you are in the console, set MewPipe.Logic as Default Project (the second dropdown input)
	{0} Then type Update-Database in the console (if this doesn't work please check your connection strings configuration or contact Alexis Chevalier)

9 - Start the project, it should fail with an IIS Express error

10 - Go to <MY DOCUMENTS FOLDER>/IISExpress/config and open the file applicationhost.config in your favorite text editor
	{0} Locate the section "configuration -> system.applicationHost -> sites" in the file
	{0} In this section, you will find some virtual directories, one for each project configured to use IIS Express.
	{0} You have to edit each section which relates to our solution to match the fake domain we used in the hosts file.
		(Note -> it may not be the exact same name, you may find a number between parenthesis after the name or not, Visual Studio creates a lot of Virtual Directories automatically, the good one will already have the good port configured).
		* <site name="MewPipe.Accounts"... -> You have to replace the binding element and change the bindingInformation to "*:44401:accounts.mewpipe.local"
		* <site name="MewPipe.API"... -> You have to replace the binding element and change the bindingInformation to "*:44400:api.mewpipe.local"
		* <site name="MewPipe.VideosRepository"... -> You have to replace the binding element and change the bindingInformation to "*:44403:videos-repository.mewpipe.local"
		* <site name="MewPipe.Website"... -> You have to replace the binding element and change the bindingInformation to "*:44402:mewpipe.local"

11 - Start the project again, it should start a console application and open a tab in your browser at http://mewpipe.local:44402/. Don't kill the console application, just leave it in background.