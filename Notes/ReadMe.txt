SVN Instructions:
Add all bin, obj folders to the IGNORE LIST. Do not check in these folders.
Add all folders under the packages folder to the IGNORE LIST also.
Only the repositories.config file is to be checked in, if changed. 


This solution works with Visual Studio 2012
Refer to SolutionSetup.docx for prerequisites required for the solution.
	
The solution consists of the following folders:
Domain:
This folder contains two projects:
	tmg.equinox.domain.entities 
		- contains the entities that map to the tables in the data store
	tmg.equinox.repository.interfaces
		- interfaces required to be implemented by the Repository / data access layer

ApplicationServices:
This folder contains the following projects:
	tmg.equinox.applicationservices.viewmodels
		- contains the models consumed/sent by the views
	tmg.equinox.applicationservices.interfaces
		- interfaces required to be implemented as Application Services

Implementation:
This folder contains the implementation of the Interfaces of the Repository
and ApplicationServices layer
The projects it contains are:
	tmg.equinox.applicationservices
		- implementation of the interfaces in the tmg.equinox.applicationservices.interfaces project
	tmg.equinox.repository
		- implementation of the interfaces in the tmg.equinox.repository.interfaces project
		- Entity Framework 6.0.2 has been set up for this project using NuGet
	tmg.equinox.infrastructure.interfaces
		- interfaces to access infrastructure services like logging will be declared here
		- a separate project will have to be implemented each infrastructure service like 
		  logging, that implements the appropriate interface/s declared here
	tmg.equinox.dependencyresolution
		- contains the dependency resolution mappings required by Unity
		- Unity.MVC5 has been set up for this project using NuGet
		 
Web:
This folder contains the following project:
	- tmg.equinox.web
		- this is the asp.net mvc5 project for the GUI of the application
		- Unity.MVC5 has been set up for this project using NuGet
			TODO :  need to remove Unity later from here if possible and 
			        move to the tmg.equinox.dependencyresolution project
