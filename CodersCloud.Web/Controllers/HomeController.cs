using CodersCloud.Data;
using CodersCloud.Web.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CodersCloud.Web.Controllers
{
    public class HomeController : Controller
    {
        private CloudQueue requestsQueue;
        private CloudBlobContainer requestBlob;
        private CloudStorageAccount storageAccount;

        public HomeController()
        {
            InitializeStorage();
        }

        private void InitializeStorage()
        {
            storageAccount = CloudStorageAccount.Parse(
                    ConfigurationManager.ConnectionStrings[ResourceNames.AzureWebJobsStorageConnectionStringKey].ToString());

            var queueClient = storageAccount.CreateCloudQueueClient();
            requestsQueue = queueClient.GetQueueReference(ResourceNames.coderscloudqueuerequestsKey);

            var blobClient = storageAccount.CreateCloudBlobClient();
            requestBlob = blobClient.GetContainerReference(ResourceNames.coderscloudblobrequestsKey);
        }

        // GET: Home
        public ActionResult Index()
        {
            return RedirectToAction("Test");
        }

        // Get: Home/Test
        public ActionResult Test()
        {
            return View();
        }

        // Post: Home/Test
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Test(BuildRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var jobId = Guid.NewGuid();

                var blobName = "test_" + DateTime.Now.Ticks + ".txt";
                var blockBlob = requestBlob.GetBlockBlobReference(blobName);

                await blockBlob.UploadTextAsync(model.Source);
                Uri url = new Uri(Url.Action("StatusNotification", null, null, Request.Url.Scheme));
                BuildRequest request = new BuildRequest() { BlobUri = blockBlob.Uri, JobId = jobId, Url = url };
                var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(request));
                await requestsQueue.AddMessageAsync(queueMessage);
                Trace.TraceInformation("Created test queue message");
                return Json(new { jobId }, JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);
        }

        public ActionResult StatusNotification(Guid jobId, string status = null, string url = null)
        {
            var connections = BuildClientsHub.GetUserConnections(jobId);

            if (connections != null)
            {
                foreach (var connection in connections)
                {
                    // Notify the client to refresh the list of connections
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<BuildClientsHub>();
                    if (!string.IsNullOrEmpty(status))
                    {
                        hubContext.Clients.Clients(new[] { connection }).updateStatus(status);
                    }
                    if (!string.IsNullOrEmpty(url))
                    {
                        hubContext.Clients.Clients(new[] { connection }).updateResult(url);
                    }
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

    }
}