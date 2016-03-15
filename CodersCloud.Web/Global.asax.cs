using CodersCloud.Data;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CodersCloud.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            InitializeStorage();
        }

        private void InitializeStorage()
        {
            var storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings[ResourceNames.AzureWebJobsStorageConnectionStringKey].ToString());
            
            // Blobs
            Trace.TraceInformation("Creating blob container");

            var blobClient = storageAccount.CreateCloudBlobClient();

            var coderscloudblobrequestsContainer = blobClient.GetContainerReference(ResourceNames.coderscloudblobrequestsKey);
            coderscloudblobrequestsContainer.CreateIfNotExists();

            var coderscloudblobresponsesContainer = blobClient.GetContainerReference(ResourceNames.coderscloudblobresponsesKey);
            if (coderscloudblobresponsesContainer.CreateIfNotExists())
            {
                coderscloudblobresponsesContainer.SetPermissions(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });
            }


            // Queues
            Trace.TraceInformation("Creating queues");

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            var queue = queueClient.GetQueueReference(ResourceNames.coderscloudqueuerequestsKey);
            queue.CreateIfNotExists();

            queue = queueClient.GetQueueReference(ResourceNames.coderscloudqueueresponsesKey);
            queue.CreateIfNotExists();

            Trace.TraceInformation("Storage initialized");

        }
    }
}
