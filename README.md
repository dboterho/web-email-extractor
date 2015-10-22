# Web Email Extractor

## What is it?
A .NET console application for extracting all unique email addresses from a collection of websites supplied via a CSV file. The application scans all the markup of the homepage, and the immediate sub-pages, and identifies email addresses through use of a regular expression.

## Motivation
As a freelance developer I'm constantly in need of new leads. I sometimes use Google to identify a list of relevant businesses who might find my skills as a developer useful, and then compile their website URL's into a list. Navigating each of these websites for a contact email address proves painfully time consuming. So I created this console app to do the work for me. All that is needed afterwards is carefully worded and professional email to make myself known to them as a developer. NOTE: I CAREFULLY pick the URL's of businesses that I consider to be appropriate targets for my cold-emails and try my best not to spam irrelevant recipients.

## Installation
The project was created using Visual Studio 2015 and as such, this IDE and version is required to open the solution and debug the application.

## How to use
Upon starting the console application the user will be asked to provide the path to a CSV comma delimited file which contains a list of website URL's. The format on the CSV file should be as follows:

// input.csv file
http://www.website1.com,
https://www.website2.com,
http://www.website3.com/,
http://www.website4.com,

The user will then be asked to provide the output directory. This is the folder where the file containing the extracted email addresses will be output to. The user will also be prompted to indicate if they would like verbose progress messages to be logged. The console will display relevant progress updates until completion. On completion the output file, now located in the output directory, will contain a list of website URL's similar to the input file but with all the unique email addresses stored in the second column:

// ExtractionResult_yyyyMMdd_HHmmss.csv file
http://www.website1.com,test1@email.com;test2@email.com;test3@email.com;
https://www.website2.com,
http://www.website3.com/,test4@email.com;test5@email.com;
http://www.website4.com,test6@email.com;

NOTE: My suggestion is that you run this application behind a VPN or Proxy to conceal your real IP address.

## Configuration
A set of variables can be configured through the app.config file:
1. CsvDelimiter: The CSV delimiter character used in the input and output files.
2. EmailRegex: The regular expression used to find email addresses in the webpage markup.
3. HrefRegex: The regular expression used to find href attributes in the webpage markup.
4. InvalidSiteLinkPatterns: This is a list of patterns which filters out internal site links which we do not want to be searched for email addresses. For example, the pattern '.jpg' indicates we do not want to look at the response of any '/xxx.jpg' paths.

## Tests
Tests are located in the WebEmailExtractor.Tests project. This project has two dependencies Moq and NUnit, which are both used to write and run the tests.

## Licensing
The source is licensed under the GNU General Public License which allows end users the freedoms to run, study, share (copy), and modify the software.

## Contributors
Daniel Boterhoven	http://danielboterhoven.it