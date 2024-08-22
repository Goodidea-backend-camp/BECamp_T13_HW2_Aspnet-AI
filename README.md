# OpenAI Application README

## Overview

This repository contains the codebase for an OpenAI application developed using ASP.NET 8.0 MVC. The application utilizes various OpenAI models to provide innovative features including generating poems from keywords, creating images with customizable details, and implementing a spam message filter.

## Features

### 1. Poem Generation

- Input: A keyword from the user.
- Output: An MP3 file of a generated poem.
- Functionality: The chatbot generates a poem based on the provided keyword, converts it into an MP3 file, and provides a download link for the user.

### 2. Image Generation

- Input: A keyword from the user.
- Output: A generated image.
- Functionality: The chatbot generates an image based on the provided keyword. Users can continue adding details to refine the image. The entire chat history, including image modifications, is saved for future reference.

### 3. Spam Message Filter

- Input: A text message from the user.
- Output: A determination of whether the message is spam.
- Functionality: The chatbot analyzes the input text and identifies whether it is a spam message, filtering out unwanted content.

## Getting Started

To get started with the development or deployment of this OpenAI application, follow the steps below:

1. Clone the Repository: Clone this repository to your local machine using the following command:

```
git clone https://github.com/Goodidea-backend-camp/BECamp_T13_HW2_Aspnet-AI.git
```

2. Install Dependencies: Navigate to the project directory and install the required dependencies using the package manager:

```
cd src/
dotnet restore
```

3. Configure User Secrets: Obtain API keys for the OpenAI services used in the application (e.g., GPT, DALL-E). Configure the API keys and any other user secrets using the following command:

```
dotnet user-secrets set "OpenAI:APIKey" "YourOpenAIAPIKey"
dotnet user-secrets set "MySQL:BECampT13HW2": "YourDatabaseConnectionString"
dotnet user-secrets set "Email:Username" "YourEmailSendingAccount"
dotnet user-secrets set "Email:Password" "YourEmailSendingPassword"
dotnet user-secrets set "Email:Host" "YourEmailSendingHost"
```

4. Build the project and run the Application: Once the dependencies are installed and the user secrets are configured, build the project and then run the application using the following command:

```
dotnet build
dotnet run
```

5. Access the Application: Access the application through the provided URL or port number. You may need to configure additional settings based on your deployment environment.
