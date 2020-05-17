using System;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Scylla.CommonModules.IOModule
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class StandaloneFileUtility : FileUtility
    {
        public bool FileExists(string filePath)
        {
            try
            {
                return File.Exists(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError("StandaloneFileUtility - FileExists : " + filePath + " == " + e.Message);
                return false;
            }
        }

        public bool FolderExists(string folderPath)
        {
            try
            {
                return Directory.Exists(folderPath);
            }
            catch (Exception e)
            {
                Debug.LogError("StandaloneFileUtility - FolderExists : " + folderPath + " == " + e.Message);
                return false;
            }
        }

        public void DeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError("StandaloneFileUtility - DeleteFile : " + filePath + " == " + e.Message);
            }
        }

        public void DeleteFolder(string folderPath, bool recurse)
        {
            try
            {
                Directory.Delete(folderPath, recurse);
            }
            catch (Exception e)
            {
                Debug.LogError("StandaloneFileUtility - DeleteFolder : " + folderPath + " == " + e.Message);
            }
        }

        public void CreateFolder(string folderPath)
        {
            try
            {
                Directory.CreateDirectory(folderPath);
            }
            catch (Exception e)
            {
                Debug.LogError("StandaloneFileUility - CreateFolder : " + folderPath + " == " + e.Message);
            }
        }

        public List<string> GetAllFoldersInFolder(string folderPath)
        {
            return Directory.GetDirectories(folderPath).ToList();
        }
        
        public List<string> GetAllFilesInFolder(string folderPath, string searchPattern = "*.*")
        {
            List<string> files = new List<string>();

            DirectoryInfo directoryInfoSave = new DirectoryInfo(folderPath);
            if (directoryInfoSave.Exists)
            {
                FileInfo[] allFilesSave = directoryInfoSave.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
                foreach (var file in allFilesSave)
                {
                    files.Add(file.FullName);
                }
            }

            return files;
        }
    }
}
