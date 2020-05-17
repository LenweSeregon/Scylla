namespace Scylla.CommonModules.IOModule
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public interface FileUtility
    {
        bool FileExists(string filePath);
        bool FolderExists(string folderPath);
        void DeleteFile(string filePath);
        void DeleteFolder(string folderPath, bool recurse);
        void CreateFolder(string folderPath);
        
        List<string> GetAllFoldersInFolder(string folderPath);
        List<string> GetAllFilesInFolder(string folderPath, string searchPattern = "*.*");
    }
}