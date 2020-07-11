using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace MAS.Hackathon.BackEnd
{
    public static class Configuration
    {
        public static IServiceCollection AddFirebaseCloudMessaging(this IServiceCollection services)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "firebase-key.json");
            FirebaseApp.Create(new AppOptions { Credential = GoogleCredential.FromFile(filePath) });
            services.AddTransient(opts => FirebaseMessaging.DefaultInstance);
            return services;
        }
    }
}
