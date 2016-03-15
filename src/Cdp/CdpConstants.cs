using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.ApiHub
{
    internal static class CdpConstants
    {
        public const int DefaultFileWatcherIntervalInSeconds = 30;

        public const string CdpConnectionStringTemplate = "Endpoint={0};Scheme={1};AccessToken={2}";

        public const string DatasetMetadata= "/$metadata.json/datasets";

        public const string DefaultDatasetName = "/default";
        public const string DatasetRoot = "datasets" + DefaultDatasetName;
        public const string FilesRoot = DatasetRoot + "/files";
        public const string FoldersRoot = DatasetRoot + "/folders";
        public const string TopMostFolderRoot = DatasetRoot + "/rootfolders";
        public const string TriggersRoot = DatasetRoot + "/triggers";

        public const string FileMetadataByPathTemplate = DatasetRoot + "/GetFileByPath?path={0}";
        public const string FileMetadataByIdTemplate = FilesRoot + "/{0}";
        public const string FileContentByPathTemplate = DatasetRoot + "/GetFileContentByPath?path={0}";
        public const string FileContentByIdTemplate = FileMetadataByIdTemplate + "/content";
        public const string CreateFileTemplate = FilesRoot + "?folderpath={0}&name={1}";
        public const string OnNewFileTemplate = TriggersRoot + "/onnewfile?folderId={0}";
        public const string OnUpdateFileTemplate = TriggersRoot + "/onupdatedfile?folderId={0}";

    }
}
