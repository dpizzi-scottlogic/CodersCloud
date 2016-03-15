using CodersCloud.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.CSharp;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CodersCloud.WebJob
{
    public class Functions
    {
        public static async Task ProcessQueueMessage(
            [QueueTrigger("coderscloudqueuerequests")] BuildRequest request,
            [Blob("coderscloudblobrequests/{BlobName}")] CloudBlockBlob input,
            [Blob("coderscloudblobresponses/{BlobNameWithoutExtension}.exe")] CloudBlockBlob outputBlob)
        {
            var jobId = request.JobId;
            var url = request.Url;            
            var success = false;
            await CommunicateStatus(url, jobId, "Processing...");

            var inputStream = input.OpenRead();
            using (StreamReader reader = new StreamReader(inputStream))
            {
                string source = reader.ReadToEnd();

                var filename = request.BlobNameWithoutExtension + ".exe";

                if (Compile(filename, source))
                {
                    using (var compiledFileStream = new FileStream(filename, FileMode.Open))
                    {

                        outputBlob.UploadFromStream(compiledFileStream);
                    }
                    success = true;
                }
                else
                {
                    await CommunicateStatus(url, jobId, "Error(s)");
                }
            }

            if (success)
            {
                var response = new BuildResponse
                {
                    JobId = jobId,
                    BlobUri = outputBlob.Uri
                };
                DeleteBuildingFiles(request.BlobNameWithoutExtension);
                input.Delete();
                await CommunicateStatus(url, jobId, "Done", outputBlob.Uri.ToString());
            }
            else throw new Exception();
        }

        private static void DeleteBuildingFiles(string fileNameWithoutExtension)
        {
            File.Delete(string.Format("{0}.exe", fileNameWithoutExtension));
            File.Delete(string.Format("{0}.pdb", fileNameWithoutExtension));
        }

        public static bool Compile(string filename, string source)
        {
            var csc = new CSharpCodeProvider();
            var parameters = new CompilerParameters(new[] { "System.dll", "mscorlib.dll", "System.Core.dll" }, filename, true);
            parameters.GenerateExecutable = true;
            CompilerResults results = csc.CompileAssemblyFromSource(parameters, source);

            return results.Errors.Cast<CompilerError>().Count() == 0;
        }

        private static async Task CommunicateStatus(Uri url, Guid jobId, string status, string bloburl = null)
        {
            var httpClient = new HttpClient();

            var queryString = string.Format("?jobId={0}&status={1}", jobId, status);
            var request = url + queryString;
            if (bloburl != null)
            {
                request += string.Format("&url={0}", bloburl);
            }

            await httpClient.GetAsync(request);
        }
    }
}
