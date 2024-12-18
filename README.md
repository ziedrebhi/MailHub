# MailHub
MailHub is an email management application built with .NET 9. It enables users to create, configure, and send personalized email templates. The application supports user authentication, email status monitoring, and background processing of email queues.

![MailHub](https://github.com/user-attachments/assets/9ef3d18d-943d-4249-b54c-1149a0b6e481)

## Features
-  User Authentication: Secure sign-up and login functionality using JWT.
- Email Template Management: Create, update, delete, and manage email templates with dynamic parameters.
- Email Queueing: Emails are queued for processing and sent in the background.
- SMTP Configuration: Users can configure email sending settings (SMTP) and designate a default sender.
- Email Monitoring: Track the status of sent emails (e.g., sent, failed) and view logs.

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

