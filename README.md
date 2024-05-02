# OpenAI Application README

## Overview

This repository contains the codebase for an OpenAI application developed using ASP.NET 8.0 MVC. The application utilizes various OpenAI models to provide innovative features including generating poems from keywords, creating images with customizable details, and implementing a spam message filter.

## Features

### 1. Poem Generation

- Keyword-based Poem Generation: Users can input a keyword, and the application will generate a poem based on that keyword. The generated poem is then converted into an MP3 file, and users can download it using the provided link.

### 2. Image Generation

- Keyword-based Image Generation: Users can input a keyword and generate an image based on that keyword. The application allows users to continuously add details to the generated image. Additionally, the application supports saving the chat history, enabling users to review interactions and generated images.

### 3. Spam Message Filter

- Spam Filter: Users can input a text message, and the application will determine whether the message is spam or not. The spam filter utilizes machine learning models to analyze the text and identify common spam patterns, helping users to filter out unwanted messages effectively.

## Getting Started

To get started with the development or deployment of this OpenAI application, follow the steps below:

1. Clone the Repository: Clone this repository to your local machine using the following command:

```
git clone https://github.com/chungkai1029/BECamp_T13_HW2_Aspnet-AI.git
```

2. Install Dependencies: Navigate to the project directory and install the required dependencies using the package manager:

```
dotnet add package Azure.AI.OpenAI --prerelease
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.UserSecrets
```

3. Configure OpenAI API Keys: Obtain API keys for the OpenAI services used in the application (e.g., GPT, DALL-E). Configure the API keys in the application settings.

```
dotnet user-secrets set "OpenAI:APIKey" "YourOpenAIAPIKey"
```

4. Run the Application: Once the dependencies are installed and the API keys are configured, run the application using the following command:

```
dotnet run --no-launch-profile
```

5. Access the Application: Access the application through the provided URL or port number. You may need to configure additional settings based on your deployment environment.
