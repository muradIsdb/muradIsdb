using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlfrescoImporter
{
       // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class User
    {
        public bool Delete { get; set; }
        public bool Write { get; set; }
        public bool CancelCheckOut { get; set; }
        public bool ChangePermissions { get; set; }
        public bool CreateChildren { get; set; }
        public bool Unlock { get; set; }
    }

    public class Data
    {
        public string ticket { get; set; }
    }

    public class AlfTicket
    {
        public Data data { get; set; }
    }



    public class Permissions
    {
        public bool inherited { get; set; }
        public List<string> roles { get; set; }
        public User user { get; set; }
    }

    public class CmCreator
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string displayName { get; set; }
        public string userName { get; set; }
    }

    public class CmModifier
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string displayName { get; set; }
        public string userName { get; set; }
    }

    public class CmCreated
    {
        public DateTime iso8601 { get; set; }
        public string value { get; set; }
    }

    public class CmModified
    {
        public DateTime iso8601 { get; set; }
        public string value { get; set; }
    }

    public class Properties
    {
        [Newtonsoft.Json.JsonProperty("cm:title")]
        public string CmTitle { get; set; }

        [JsonProperty("cm:creator")]
        public CmCreator CmCreator { get; set; }

        [JsonProperty("cm:modifier")]
        public CmModifier CmModifier { get; set; }

        [JsonProperty("cm:created")]
        public CmCreated CmCreated { get; set; }

        [JsonProperty("cm:name")]
        public string CmName { get; set; }

        [JsonProperty("sys:store-protocol")]
        public object SysStoreProtocol { get; set; }

        [JsonProperty("sys:node-dbid")]
        public object SysNodeDbid { get; set; }

        [JsonProperty("sys:store-identifier")]
        public object SysStoreIdentifier { get; set; }

        [JsonProperty("sys:locale")]
        public object SysLocale { get; set; }

        [JsonProperty("cm:modified")]
        public CmModified CmModified { get; set; }

        [JsonProperty("cm:description")]
        public string CmDescription { get; set; }

        [JsonProperty("sys:node-uuid")]
        public object SysNodeUuid { get; set; }

        [JsonProperty("cm:autoVersion")]
        public string CmAutoVersion { get; set; }

        [JsonProperty("cm:versionLabel")]
        public string CmVersionLabel { get; set; }

        [JsonProperty("cm:autoVersionOnUpdateProps")]
        public string CmAutoVersionOnUpdateProps { get; set; }

        [JsonProperty("cm:content")]
        public object CmContent { get; set; }

        [JsonProperty("cm:lastThumbnailModification")]
        public List<string> CmLastThumbnailModification { get; set; }

        [JsonProperty("cm:initialVersion")]
        public string CmInitialVersion { get; set; }

        [JsonProperty("cm:author")]
        public string CmAuthor { get; set; }
    }

    public class Parent
    {
        public bool isLink { get; set; }
        public string nodeRef { get; set; }
        public Permissions permissions { get; set; }
        public bool isLocked { get; set; }
        public List<string> aspects { get; set; }
        public bool isContainer { get; set; }
        public string type { get; set; }
        public Properties properties { get; set; }
    }

    public class Aos
    {
        public string baseUrl { get; set; }
    }

    public class Custom
    {
        public Aos aos { get; set; }
        public object vtiServer { get; set; }
    }

    public class ItemCounts
    {
        public int folders { get; set; }
        public int documents { get; set; }
    }

    public class Metadata
    {
        public string repositoryId { get; set; }
        public string container { get; set; }
        public Parent parent { get; set; }
        public Custom custom { get; set; }
        public bool onlineEditing { get; set; }
        public ItemCounts itemCounts { get; set; }
        public string workingCopyLabel { get; set; }
    }

    public class Node
    {
        public bool isLink { get; set; }
        public List<string> aspects { get; set; }
        public bool isContainer { get; set; }
        public string type { get; set; }
        public string encoding { get; set; }
        public string contentURL { get; set; }
        public int size { get; set; }
        public string nodeRef { get; set; }
        public Permissions permissions { get; set; }
        public bool isLocked { get; set; }
        public string mimetype { get; set; }
        public Properties properties { get; set; }
        public string mimetypeDisplayName { get; set; }
    }

    public class Likes
    {
        public bool isLiked { get; set; }
        public int totalLikes { get; set; }
    }

    public class Site
    {
        public string name { get; set; }
        public string title { get; set; }
        public string preset { get; set; }
    }

    public class Container
    {
        public string name { get; set; }
        public string type { get; set; }
        public string nodeRef { get; set; }
    }

    public class Location
    {
        public string repositoryId { get; set; }
        public Site site { get; set; }
        public Container container { get; set; }
        public string path { get; set; }
        public string repoPath { get; set; }
        public string file { get; set; }
        public Parent parent { get; set; }
    }

    public class Item
    {
        public Node node { get; set; }
        public string version { get; set; }
        public string webdavUrl { get; set; }
        public bool isFavourite { get; set; }
        public Likes likes { get; set; }
        public Location location { get; set; }
    }

    public class Root
    {
        public int totalRecords { get; set; }
        public int startIndex { get; set; }
        public Metadata metadata { get; set; }
        public List<Item> items { get; set; }
    }


}
