using System.IO;
using Microsoft.Maui.Storage;

namespace BlocDeNotas.Services
{
    public static class FirebaseCredentialManager
    {
        public static void SaveFirebaseCredentials()
        {
            string jsonCredentials = @"{
  ""type"": ""service_account"",
  ""project_id"": ""base-de-datos-usuarios-c9142"",
  ""private_key_id"": ""0526d2231175e2dcc9f2f72f47113e0d5977a8bc"",
  ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDAJB3JyW5H/KBu\nWE9md9/u5LdWkoElDglunU07xQMAtIwmi9j21accLEVMhuRsJ8RTxP5TXsYwwP0e\n8Q0SUa06tWkh7VlwSur3e4O0e/jAd2Svku0CI37OxM3xplkZachqBrAIpBMeTcl2\nn/DWIpmnUCm9+FlJV027UZU64aoDWtdZA2A7oI+ZDxfw/BKBvia7P6xdRgEj5gMz\n7DFtyr5P8kk1D8b/V7GR13zxqn6D7s8aenmUtmV1kVs14eTD8f7OSe8G9HWGtEpe\nDwPeYj6O7TJuQCtmY9HlMKIK4iXd9IZ0CFDBzJMn3UTOFA+WxPlR0YEePdqpXw4n\nwpigu1T3AgMBAAECggEAMjT9x62n8I2Vyl+vF8J5NG51bdzIfEtHsazO8rBPUY7c\nFHJ0lkNL8HOtBAs3gv99MLqIDnb0aOb0/XHVjZTkJDQlLaVcupgI//Q5fR7r6DcK\ni0pkPivSRettORjTrW1T3kizFP7ys/d1jlbK0tmltu+eiLJtelmiyTDPQtyW1Vsh\nc0KgpvBGr5NL9hhu07tCnxnCJ6NpkP5NUq2RkCqdO1gU1/55qo+9zNLobrOyknW2\nCC3ijswXlKLNBU9KDp4tPYkuV+MQpr24JXLANgYYy/N3TbhbsOO++VzVKFxO6H0v\nrXLCq7G4Eu39BeSHE7gAUoKuuQ7Xks6FCVM3bgYDEQKBgQD9+V5jFNFHtw077Fpr\nVCzn4+/h+SdL5T5splokGkGIG0rpRHNJxVtELeBUVN42S29yp1fpnFKwwlEmtizg\nXi19IPQQDSiBAu1SVjPsIA0/Y4VIXhIUe/SgZNVjDJ0lW2wrSh+PAJQisLyP/yhN\n1IMKH6Cw6XevH4jBvWJLH+w22QKBgQDBrHsO+iiL25RZxwGU0XsV/sk7TycyJtfO\n/ETOtZVMqNjSq+5mODdKaqMVCQau8nzP/dE24SoDK8sGltA/pDSnxW1AvZdjAUhe\nR1hqHYWkpzlI7RmadmEkt+ylczzdoxraVunvoEwSGSDL83KkK9crLak8+m9eBrBg\nHtm8gpioTwKBgF4Ugw9nBcNwHrnk6vW5P81C1Xi7CACIUiCDWv7mrwHRQmW40pAw\nSIewCtqSeaJZPVaWgO1r2AHcaZ1SfLs8h9NDYsWTaj6oK+uFPUSp9t5VamkNV9s4\ns7y6vYUshgXxoNdEZP5fM4DKQX+CAJccMtKMNsVyJsC7iLhkBEZZ8/G5AoGBAK4H\n2URqEckq9LF/m7IoUpw/KH/87lGoib/a+9FrZc9O4hbcnAqKdVSPYh/izniiOPmF\nMFxKFEBpN7SiDFtDqHUheLz2IHS8kFT3c0FXaeG8ykL3m0wrF7uw8hrx5D2c9OJB\nu1CyD+krX0claVyQcHbCPxMmTrCE/jRBRJKaH6cNAoGBAKOr7gO5xOWOz0F1wTJh\naP1f1hv+OhZcIN6qN0lYNRNwyJyc8t5qrZ1xkrIy4Vt41lsZJEXvjWYvPvUxEydT\n1dvsokr2j5gn2LCRhlKMqVGjbIu2AFNXiokJHpKdlk6v3YsljYzbxhEEDORowk1s\nRnP32oSTtI7gbZRGQCnTLHPv\n-----END PRIVATE KEY-----\n"",
  ""client_email"": ""firebase-adminsdk-fbsvc@base-de-datos-usuarios-c9142.iam.gserviceaccount.com"",
  ""client_id"": ""108094371710160103288"",
  ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
  ""token_uri"": ""https://oauth2.googleapis.com/token"",
  ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
  ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-fbsvc%40base-de-datos-usuarios-c9142.iam.gserviceaccount.com"",
  ""universe_domain"": ""googleapis.com""
}
";

            string path = Path.Combine(FileSystem.AppDataDirectory, "firebase_credentials.json");
            File.WriteAllText(path, jsonCredentials);
            Console.WriteLine($"Credenciales guardadas en: {path}");
        }

        public static string LoadFirebaseCredentials()
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, "firebase_credentials.json");

            if (!File.Exists(path))
            {
                Console.WriteLine("Error: No se encontró el archivo de credenciales.");
                return null;
            }

            return File.ReadAllText(path);
        }
    }
}