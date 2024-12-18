using MailHub.ApiClient.Model;

namespace MailHub.ApiClient.Wrappers
{
    /// <summary>
    /// Wrapper class to interact with the MailHub API
    /// </summary>
    public class MailHubApi
    {
        private readonly ApiClient _apiClient;  // The ApiClient instance used to send HTTP requests
        private readonly string _baseEndpoint;  // The base URL for the MaiLHub API

        /// <summary>
        /// Initializes a new instance of the <see cref="MailHubApi"/> class.
        /// </summary>
        /// <param name="apiClient">The ApiClient instance used for making API requests.</param>
        /// <param name="baseEndpoint">The base URL endpoint for the MailHub API.</param>
        public MailHubApi(ApiClient apiClient, string baseEndpoint)
        {
            _apiClient = apiClient;  // Assign the provided ApiClient
            _baseEndpoint = baseEndpoint;  // Assign the provided base endpoint
        }

        /// <summary>
        /// Adds an email to the queue via the MailHub API.
        /// </summary>
        /// <param name="emailQueue">The email queue object containing the necessary data (TemplateId, TemplateName, Recipient, Parameters).</param>
        /// <param name="cancellationToken">The cancellation token to cancel the request.</param>
        /// <returns>A message indicating the result of the operation.</returns>
        public async Task<string> AddEmailToQueueAsync(EmailQueue emailQueue, CancellationToken cancellationToken)
        {
            // Define the API endpoint for adding email to the queue
            string endpoint = $"{_baseEndpoint}/api/email/Add";

            // Send the request to the API and get the result
            var result = await _apiClient.SendAsync<object>(
                HttpMethod.Post,
                endpoint,
                emailQueue,
                cancellationToken: cancellationToken
            );

            // Return a success or failure message
            return result != null ? "Email added to queue successfully." : "Failed to add email to queue.";
        }

        /*  Usage
         *   // Create an instance of HttpClient
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

         *  
         *  
         */
    }
}