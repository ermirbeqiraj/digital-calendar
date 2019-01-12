using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GetImages
{
    /*
     * Resources:
     * https://developers.google.com/drive/api/v3/about-files
     * https://developers.google.com/drive/api/v3/search-parameters
     * https://developers.google.com/drive/api/v3/manage-downloads
     * 
    */
    class Program
    {
        const string CLIENT_ID_FILE_NAME = "dd-client-id.json";
        const string DRIVE_FOLDER_ID = "1mF9vGUoGpaHLG_jnP_c2HKyL6o_8HqhZ";

        //static readonly string DOWNLOAD_FOLDER_PATH = @"C:\eb\ermir.net\github\digital-calendar\Source\WebApp\wwwroot\images";
        static readonly string DOWNLOAD_FOLDER_PATH = @"/var/netcore/web/wwwroot/images";
        static readonly string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "DC-Download-Photos";

        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream = new FileStream(CLIENT_ID_FILE_NAME, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,

            });

            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";
            listRequest.Q = $"'{DRIVE_FOLDER_ID}' in parents";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    DownloadFile(file.Id, file.Name, service);
                }
            }
        }

        static void DownloadFile(string fileId, string fileName, DriveService driveService)
        {
            if (File.Exists(Path.Combine(DOWNLOAD_FOLDER_PATH, fileName)))
                return;

            var request = driveService.Files.Get(fileId);
            using (var stream = new MemoryStream())
            {
                request.Download(stream);

                stream.Seek(0, SeekOrigin.Begin);
                using (var fs = new FileStream(Path.Combine(DOWNLOAD_FOLDER_PATH, fileName), FileMode.Create))
                {
                    stream.CopyTo(fs);
                    fs.Flush();
                }
            }
        }
    }
}
