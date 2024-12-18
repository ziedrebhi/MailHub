# MailHub
MailHub is an email management application built with .NET 9. It enables users to create, configure, and send personalized email templates. The application supports user authentication, email status monitoring, and background processing of email queues.

![MailHub](https://github.com/user-attachments/assets/9ef3d18d-943d-4249-b54c-1149a0b6e481)

# MailHub Features (Until now)

## 1. Email Configurations Management
- Configuring email sender and saving data in the database.
- Manage SMTP or other email service configurations for sending emails.

## 2. Email Templates Management
- Creating, updating, and deleting email templates.
- Templates can contain dynamic parameters for customization during sending.

## 3. Authentication (Login & Signup)
- User authentication for accessing the application.
- JWT authentication for securing endpoints and managing user sessions.

## 4. Email Sending Service (Background Service)
- A background service that processes emails.
- Sends emails in the background to avoid blocking the main thread.

## 5. Email Queueing
- Queueing emails for sending either via an API or using the client NuGet package.
- Allows email requests to be stored and processed asynchronously.

## 6. Queue Monitoring
- Added functionality to monitor the status of queued emails.
- Includes endpoints to check the current status, including:
  - **Pending**
  - **Sent**
  - **Failed**
- Provides detailed information such as error messages and timestamps of sent emails.

## 7. API Client (NuGet Package)
- Developed a generic API client that interacts with MailHub API for email queueing.
- Supports different authorization methods like JWT, Basic Auth, or API Key.
- Encapsulates communication with MailHub endpoints for client usage.

## 8. Email Queue Management (via API & NuGet)
- Clients can add emails to the queue using API endpoints or through the NuGet package.
- Includes all necessary details such as template, recipient, and dynamic parameters.

## 9. Error Handling & Logging for Email Queue
- Provides error messages when emails fail to send, which helps in troubleshooting and monitoring.


## Tech Stack

- Backend: .NET 9
- Authentication: JWT (JSON Web Tokens)
- Database: SQL Server (using Entity Framework Core)
- Architecture: Clean Architecture, CQRS, Mediatr
- Validation: FluentValidation
- Email Sending: Custom SMTP integration
  
![image](https://github.com/user-attachments/assets/75e6b28f-398c-4546-9c18-8c2bd872da93)

## Usage
 1.  Authentication:
     -  Use the sign-up endpoint to create a new user.
     - Log in to get a JWT token that will be used to access protected resources.
2. Email Template Management:
    -  Create email templates with dynamic parameters.
    -  Templates can be configured via the API or frontend.
3. Email Sending:
    -  Add emails to the queue, and they will be processed and sent in the background.
    -  The background service will check the queue every 10 seconds.

4. Monitoring
    - Track the status of emails sent and view logs.
  
## Usage of MailHub.ApiClient package 
 

    // Create an instance of HttpClient
    var httpClient = new HttpClient();

    // Create an instance of JwtAuthorizationProvider with a sample JWT token
    var jwtAuthorizationProvider = new JwtAuthorizationProvider("your-jwt-token");

    // Create an instance of ApiClient, passing HttpClient and JwtAuthorizationProvider
    var apiClient = new ApiClient(httpClient, jwtAuthorizationProvider);

    // Create an instance of MailHubApi, passing the ApiClient and base endpoint URL
    var mailHubApi = new MailHubApi(apiClient, "https://your-mailhub-api.com");

    // Create an EmailQueue object to be added to the queue
    var emailQueue = new EmailQueue
    {
        TemplateId = 1,  // Optional TemplateId (or use TemplateName)
        TemplateName = "WelcomeEmail",  // Optional TemplateName
        Recipient = "recipient@example.com",  // Email recipient
        Parameters = new Dictionary<string, string> 
        {
            { "FirstName", "John" },
            { "LastName", "Doe" }
        }  // Parameters to be passed to the email template
    };

    // Call AddEmailToQueueAsync to add the email to the queue
    var resultMessage = await mailHubApi.AddEmailToQueueAsync(emailQueue, CancellationToken.None);

