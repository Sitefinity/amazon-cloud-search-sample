# amazon-cloud-search-sample

The Amazon cloud search sample is module-free and is suitable for people who want to build a project from ground-up.   
Using the Amazon search sample, you can build an Amazon cloud search service.

### Deprecated capabilities

Amazon Cloud Search is no longer supported. If you have Amazon CloudSearch setup, the system status dashboard widget will report an error. We suggest configuring one of the [three built-in search services](https://www.progress.com/documentation/sitefinity-cms/for-developers-compare-search-services) instead.

> **Latest supported version**: Sitefinity CMS 13.2.7500.0

### Requirements

* Sitefinity CMS license
* .NET Framework 4
* Visual Studio 2017
* Microsoft SQL Server 2008R2 or later versions
* [AWS Account](http://docs.aws.amazon.com/AWSSimpleQueueService/latest/SQSGettingStartedGuide/AWSAccounts.html) 

### Prerequisites

Clear the NuGet cache files. To do this:

1. In Windows Explorer, open the **%localappdata%\NuGet\Cache** folder.
2. Select all files and delete them.

### Nuget package restoration
The solution in this repository relies on NuGet packages with automatic package restore while the build procedure takes place.   
For a full list of the referenced packages and their versions see the [packages.config](https://github.com/Sitefinity-SDK/amazon-cloud-search-sample/blob/master/SitefinityWebApp/packages.config) file.    
For a history and additional information related to package versions on different releases of this repository, see the [Releases page](https://github.com/Sitefinity-SDK/amazon-cloud-search-sample/releases).    

## More information

This sample code is governed by the incuded [EULA](https://github.com/Sitefinity/amazon-cloud-search-sample/blob/master/EULA.md). If you require support with it, you may create pull requests and open issues in this repository and the Sitefinity team will address them.
