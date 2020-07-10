namespace Scylla.Editor
{
    using System;
    using System.IO;
    using UnityEditor;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EnumerationGenerator
    {
        private string _mPath;
        private string _mFileName;
        private string _mEnumerationName;
        private string _mEnumerationNamespace;
        private string[] _mEnumerationValues;
        
        public EnumerationGenerator(string path, string fileName, string enumName, string enumNamespace, string[] enumValues)
        {
            _mPath = path;
            _mFileName = fileName;
            _mEnumerationName = enumName;
            _mEnumerationNamespace = enumNamespace;
            _mEnumerationValues = enumValues;
        }

        public void GenerateFile()
        {
            string path = Path.Combine(_mPath, _mFileName + ".cs");
            List<string> fileContent = GetFileContent();
            if (File.Exists(_mPath))
            {
                File.Delete(_mPath);
            }

            using (StreamWriter outfile = new StreamWriter(path))
            {
                foreach (string line in fileContent)
                {
                    outfile.WriteLine(line);
                }
            }
            AssetDatabase.Refresh();
        }

        private List<string> GetFileContent()
        {
            List<string> fileContent = new List<string>();
            
            fileContent.Add("// File auto-generated from Scylla SceneManagement");
            fileContent.Add("// You MUST not modify this enumeration manually but use the generation tool provided");
            fileContent.Add("");
            
            if (string.IsNullOrEmpty(_mEnumerationNamespace) == false)
            {
                fileContent.Add("namespace " + _mEnumerationNamespace);
                fileContent.Add("{");
            }

            fileContent.Add("\tpublic enum " + _mEnumerationName);
            fileContent.Add("\t{");
            for (var i = 0; i < _mEnumerationValues.Length; i++)
            {
                string value = "";
                if (i == _mEnumerationValues.Length - 1)
                {
                    value = "" + _mEnumerationValues[i];
                }
                else
                {
                    value = "" + _mEnumerationValues[i] + ",";
                }
                
                fileContent.Add("\t\t"+value);
            }

            fileContent.Add("\t}");
            if (string.IsNullOrEmpty(_mEnumerationNamespace) == false)
            {
                fileContent.Add("}");
            }

            return fileContent;
        }
    }
}

