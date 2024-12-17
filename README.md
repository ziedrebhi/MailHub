# MailHub
MailHub is an email management application built with .NET 9. It enables users to create, configure, and send personalized email templates. The application supports user authentication, email status monitoring, and background processing of email queues.

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