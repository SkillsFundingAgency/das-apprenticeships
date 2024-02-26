## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Apprenticeships

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

_Update these badges with the correct information for this project. These give the status of the project at a glance and also sign-post developers to the appropriate resources they will need to get up and running_

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/das-apprenticeships?branchName=master)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=das-apprenticeships&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=_projectId_&metric=alert_status)](https://sonarcloud.io/dashboard?id=_projectId_)
[![Jira Project](https://img.shields.io/badge/Jira-Project-blue)](https://skillsfundingagency.atlassian.net/jira/software/c/projects/FLP/boards/753)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/3480354918/Flexible+Payments+Models)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

The Apprenticeships application is responsible for managing the lifecycle of an apprenticeship once it has been initially approved by the employer and provider.

This functionality was originally intertwined with the initial approval functionality in commitments but has been split to simplify the solution and also to allow the two data models to diverge.

## How It Works

The Inner API/Application repository is made up of two main components, an Azure Functions App and a Web API.

### Inner API

The Inner API, and MVC WebAPI, is responsible for providing functionality to query and amend Apprenticeship data.  The API is currently only used by the Apprenticeships UI but the intention is that any consumers of the existing Approvals API which need post approval data will eventually use this inner API.

### Functions

The Functions App is responsible for consuming messages from other applications and synchronising the data held against an Apprenticeship based on those events.  For example, when a new Apprenticeship is approved in Commitments, the functions app will handle that message and identify if the approval is for a pre-existing Apprenticeship, if it is then the new approval details will be associated to that Apprenticeship and if not a new Apprenticeship will be created.

## 🚀 Installation

### Pre-Requisites

* A clone of this repository
* A code editor that supports Azure functions and .Net6
* Azure Storage Emulator (Azureite)
* A SQL Server instance

### Config

Most of the application configuration is taken from the [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config) and the default values can be used in most cases.  The config json will need to be added to the local Azure Storage instance with a a PartitionKey of LOCAL and a RowKey of SFA.DAS.Apprenticeships_1.0. To run the application locally the following values need to be updated:

| Name                        | Value                                    |
| --------------------------- | ---------------------------------------- |
| NServiceBusConnectionString | UseLearningEndpoint=true                 |
| DbConnectionString          | Your local DB instance connection string |

## 🔗 External Dependencies

Whilst it might seem very odd, the Apprenticeships Functions App is dependant upon the Apprenticeships Outer API.  If you want to run certain functions you will need to make sure the Outer API is running locally or the configuration is pointing to a deployed instance.

## Running Locally

* Deploy the database project to the database instance specified in config
* Make sure Azure Storage Emulator (Azureite) is running
* Run the application