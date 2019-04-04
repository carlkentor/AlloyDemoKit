# AlloyDemoKit
A version of the Alloy reference site containing additional features for demoing purposes.  This is not intended to be a starter solution but provides the ability to showcase a number of features and add-ons that may be needed for a demo.

See the [wiki](https://github.com/episerver/AlloyDemoKit/wiki) for more information

The site can be re-designed or updated quite easily.  There are a number of useful block and page types and Find is used by default for search.

Note:  Add-ons and packages installed will need to be configured.  For example, Social Reach, Goolge Analytics, etc. 
If you are making commits to AlloyDemoKit you must ensure that you have removed any configuration that is specific to your demos



# Vulcan setup 

## To get vulcan up and running we'd need to set the following: 

1. Install [Java 11^](https://www.oracle.com/technetwork/java/javase/downloads/jdk12-downloads-5295953.html)
2. Ensure the Environment variable "JAVA_HOME" exists. example(C:\Program Files\Java\jdk-11.0.2)
3. Download [ElasticSearch 6^] (https://www.elastic.co/downloads/elasticsearch)
4. Open up CMD (Admin) => navigate to the elastic folder and run elasticsearch.bat 
5. Open Powershell and run "Invoke-RestMethod http://localhost:9200" OR open CMD and run "curl http://localhost:9200"
6. Ensure that the following appsettings are present and point to the elasticsearch port (9200) 
     <add key="VulcanUrl" value="http://localhost:9200/" />
     <add key="VulcanIndex" value="vulcan" />