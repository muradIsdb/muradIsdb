using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.SharePoint.Client;
using System.Security;
using System.Net;
using File = System.IO.File;
using Newtonsoft.Json;
using AlfrescoImporter;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.Diagnostics;
using NLog;

namespace AlfrescoImporterCode
{
    class Program
    {
        //static string SharePointUrl = "https://isdb.sharepoint.com/teams/IDBGLibrary-Public";
        static string SharePointUrlDoc = @"https://isdb.sharepoint.com/teams/IDBGLibrary-Public/";

        static string SharePointUrl = @"https://isdb.sharepoint.com/teams/IDBGLibrary-Public";
       // static string SharePointUrlDoc = @"https://isdb.sharepoint.com/IDBGLibrary-Public/_vti_bin/sites.asmx";
        static string alfticket;
        static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static int totalNumberDocument = 0;
        static int totalNumberFolder = 0;

        static void Main(string[] args)
        {            
            logger.Info("Application Started");
            Console.Write("Application Started....");

            //var DoMigration =  MigrateChildren("09a50944-fc11-4beb-b251-a304123efe60", ""); //main DocumentLibrary
            var DoMigration = MigrateChildren("7309f976-d5c9-44ff-90c2-b61d58e3b31f", ""); // Test DocumentLibrary
            DoMigration.Wait();
            logger.Info("Application Ended");
            logger.Info("Total Number of Folder is: " + totalNumberFolder.ToString());
            logger.Info("Total Number of Document is: " + totalNumberDocument.ToString());
            NLog.LogManager.Shutdown();
            Console.WriteLine("Done!");
            Console.WriteLine("Total Number of Folder is: " + totalNumberFolder.ToString());
            Console.WriteLine("Total Number of Document is: " + totalNumberDocument.ToString());
            Console.WriteLine("Done!");

        }
       
        // <summary>
        //this method get the document from Alfresco
        // </summary>
        static async Task<HttpResponseMessage> getAlfFile(string fullNodeRef)
        {
            if (alfticket != null)
                alfticket = getAlfNewTicket();
            string _url;
            _url= "https://ecm.isdb.org/alfresco/s/slingshot/node/content/"+fullNodeRef.Replace(":/","")+"?a=true&alf_ticket="+alfticket;
            HttpClient _client = new HttpClient();
            HttpResponseMessage response = await _client.GetAsync(_url);
            return response;
        }

        /// <summary>
        //// get access to sharepoint list
        /// </summary>
        private static List getAccessToSPList(ClientContext context)
        {
            SecureString theSecureString = new NetworkCredential("", @"sdfdsf").SecurePassword;
            context.Credentials = new SharePointOnlineCredentials("SVC_IDBALFSPP@idbhq.org", theSecureString);
            var oList = context.Web.Lists.GetByTitle("Documents");
            oList.EnableFolderCreation = true;
            context.Load(oList);
            context.ExecuteQuery();
            return oList;
        }

        // <summary>
        //this method create folder in Sharepoint - To be added by Fadi
        // </summary>
        private static void PostFolderToSP(string folderPath, string folderName)
        {
            try
            {
                using (var context = new ClientContext(SharePointUrl))
                {
                    var oList = getAccessToSPList(context);
                    ListItemCreationInformation FolderCreationInformation = new ListItemCreationInformation();

                    FolderCreationInformation.UnderlyingObjectType = FileSystemObjectType.Folder;
                    FolderCreationInformation.LeafName = folderName;
                    ListItem newItem;
                    

                    //start
                    Folder parentfolder = null;
                    if (folderPath == "")
                    {
                        parentfolder = oList.RootFolder;
                        //parentfolder.AddSubFolder(folderPath);
                        newItem = oList.AddItem(FolderCreationInformation);
                        newItem["Title"] = folderName;
                        newItem.Update();
                    }
                    else
                    {
                        parentfolder = oList.RootFolder.Folders.Add((folderPath+"/"+ folderName).Replace("////", "/").Replace("///", "/").Replace("//","/"));
                        parentfolder.Update();
                       
                    }

                   
                    context.ExecuteQuery();
                    totalNumberFolder += 1;
                    //end

                    // working
                    //ListItem newItem = oList.AddItem(FolderCreationInformation);
                    //newItem["Title"] = folderName;
                    //newItem.Update();
                    //context.ExecuteQuery();
                    // working
                }
            }

            catch (Exception ex)
            {
                String exDetail = String.Format("Exception message: {0}{1}Exception Source: {2}{1}Exception Code Line No: {3}{1}",
                    ex.Message, Environment.NewLine, ex.Source, ex.StackTrace);
                logger.Error(exDetail);
                Console.Write(exDetail);

            }
        }
        // <summary>
        //this method create folder in Sharepoint - To be added by Fadi
        // </summary>
        static async Task PostDocumentToSP(HttpResponseMessage response, string fileName, string fileTitle, string filePath, string fileAuther, string fileDesc)
        {
            try
            {
                using (var context = new ClientContext(SharePointUrl))
                {
                    List oList = getAccessToSPList(context);
                    var fileCreationInformation = new FileCreationInformation();
                             
                    var stream = await response.Content.ReadAsStreamAsync();
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);


                        fileCreationInformation.ContentStream = memoryStream;
                        fileCreationInformation.Overwrite = true;

                        Microsoft.SharePoint.Client.File uploadFile;

                        //Upload URL
                        // " * : < > ? / \ | %
                        string cleanFileName;
                        cleanFileName = fileName.Replace("%", " ").Replace("\"", " ").Replace("*", " ").Replace(":", " ").Replace("<", " ").Replace(">", " ").Replace("?", " ").Replace("/", " ").Replace("\\", " ").Replace("|", " "); 
                        fileCreationInformation.Url = cleanFileName;
                        //uploadFile = oList.RootFolder.Files.Add(fileCreationInformation);

                        Folder Clientfolder = null;
                        if (filePath == "")
                        {
                            Clientfolder = oList.RootFolder;
                        }
                        else if (filePath == "Shared Documents/")
                        {
                            filePath = "";
                            Clientfolder = oList.RootFolder;
                        }
                        else
                        {

                            Clientfolder = oList.RootFolder.Folders.Add(filePath.Replace("////", "/").Replace("///", "/").Replace("//", "/"));
                            Clientfolder.Update();
                        }

                        uploadFile = Clientfolder.Files.Add(fileCreationInformation);

                        ListItem item = uploadFile.ListItemAllFields;                      
                        item.ParseAndSetFieldValue("Title", fileTitle);
                        item.ParseAndSetFieldValue("Alfresco_x0020_Author", fileAuther);
                        item.ParseAndSetFieldValue("Description1", fileDesc);


                        item.SystemUpdate();
                        context.Load(item);// uploadFile);
                        context.Load(uploadFile);
                        context.ExecuteQuery();
                        totalNumberDocument+=  1;
                        var UniqueId = uploadFile.UniqueId;
                        memoryStream.Close();

                    }

                }

            }

            catch (Exception ex)
            {

                String exDetail = String.Format("Exception message: {0}{1}Exception Source: {2}{1}Exception Code Line No: {3}{1}", ex.Message, Environment.NewLine, ex.Source, ex.StackTrace);
                logger.Error(exDetail);
                Console.Write(exDetail);
            }
        }

        // <example>Test return and deserialize results</example>
        static void GetFolder()
        {
            string filepath = @"C:\Users\300168\OneDrive - Islamic Development Bank (IDB)\Alfresco importer\testjson.json";
            if (File.Exists(filepath))
            {

                //call API - ticket
                //call API - get children as json
                // deserialize children

                Root RootFolders = JsonConvert.DeserializeObject<Root>(File.ReadAllText(filepath));

                Console.OutputEncoding = Encoding.UTF8;

                foreach (Item ii in RootFolders.items)
                {
                    Console.Write(ii.node.properties.CmName);
                    Debug.Write(ii.node.properties.CmName);
                }


                Console.ReadLine();
            }

            //call sharepoint API - upload file and properties

        }

        // <summary>
        //this method does the following
        // 1. return children of node from alfresco
        // 2. if child is a document, upload to sharepoint, if it's folder, call itself recursively 
        // </summary>
        static async Task<HttpResponseMessage> MigrateChildren(string noderef, string filePath)
        {
            //filePath = filePath.Replace("/","");
            string _url;
            _url = "https://ecm.isdb.org/alfresco/s/slingshot/doclib2/doclist/folder/node/workspace/SpacesStore/" + noderef + "?alf_ticket=";
            HttpClient _client = new HttpClient();
            _client.BaseAddress = new Uri(_url);

            // Add an Accept header for JSON format.
            _client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            if (alfticket is null)
                alfticket = getAlfNewTicket();
            if (alfticket is null)
            { 
                Console.Write("Alfresco isn't reachable");
                logger.Error("Alfresco isn't reachable");
               
            }
            HttpResponseMessage _response = _client.GetAsync(_url+ alfticket).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (_response.IsSuccessStatusCode)
            {
                Root nodeChildren;
                string res = _response.Content.ReadAsStringAsync().Result;
                nodeChildren = JsonConvert.DeserializeObject<Root>(res);
               
                    //has children
                    foreach (Item item in nodeChildren.items)
                    {
                        if (item.node.isContainer == true)
                        { // Folder
                              PostFolderToSP(filePath, item.node.properties.CmName);

                        // migrate children
                        if (filePath == "") {
                            filePath = "Shared Documents/";
                                }
                        else
                        {
                            filePath = filePath +"/" ;
                        }
                            var DoMigration = MigrateChildren(item.node.nodeRef.Replace("workspace://SpacesStore/",""), filePath+ item.node.properties.CmName);
                            DoMigration.Wait();
                        }
                        else
                        { //Document
                          // Save item in Shareopoint
                        var getfiletask = getAlfFile(item.node.nodeRef);
                        getfiletask.Wait();
                        var newtask = PostDocumentToSP(getfiletask.Result, item.node.properties.CmName, item.node.properties.CmTitle, filePath, item.node.properties.CmAuthor, item.node.properties.CmDescription);
                        newtask.Wait();
                    }
                       
                    
                }
            }
            else
            {
                Console.Write("{0} ({1})", (int)_response.StatusCode, _response.ReasonPhrase);
            }

            _client.Dispose();
            return _response;
        }
        
        // <summary>
        //this method returns a valid token
        // </summary>
        static string getAlfNewTicket()
        {
            string URL;
            URL = "https://ecm.isdb.org/alfresco/s/api/login";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var cred = new StringContent(@"{""username"": ""300168"",""password"": ""asdasd""}");
                HttpResponseMessage response = client.PostAsync(URL, cred).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body.
                    string res = response.Content.ReadAsStringAsync().Result;
                    alfticket = JsonConvert.DeserializeObject<AlfTicket>(res).data.ticket;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                }
                else
                {
                    Console.Write("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    logger.Error(response.ReasonPhrase);

                }
            }
            catch (Exception ex)
            {

                String exDetail = String.Format("Exception message: {0}{1}Exception Source: {2}{1}Exception Code Line No: {3}{1}", ex.Message, Environment.NewLine, ex.Source, ex.StackTrace);
                logger.Error(exDetail);
                Console.Write(exDetail);
            }

            client.Dispose();
            return alfticket;
        }


    }
    }

