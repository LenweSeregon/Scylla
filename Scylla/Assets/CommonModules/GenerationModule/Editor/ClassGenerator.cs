using System.Linq;
using Scylla.UtilsModule;

namespace Scylla.GenerationModule.Editor
{
    using System.Text;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class ClassGenerator
    {
        private string[] _usings;
        private string[] _classAttributes;
        private string _path;
        private string _fileName;
        private string _className;
        private string _namespaceName;
        private string _extendName;
        private string[] _interfaceNames;
        private string[] _whereClauses;

        private StringBuilder _builder;
        private int _indentationLevel;

        public ClassGenerator(string[] classAttributes, string[] usings, string path, string fileName, string className, string namespaceName,
            string extendName, string[] interfaceNames, string[] whereClauses)
        {
            _usings = usings;
            _classAttributes = classAttributes;
            _path = path;
            _fileName = fileName;
            _className = className;
            _namespaceName = namespaceName;
            _extendName = extendName;
            _interfaceNames = interfaceNames;
            _whereClauses = whereClauses;
        }

        public void GenerateClass()
        {
            string path = Path.Combine(_path, _fileName + ".cs");
            string fileContent = GetContent();
            if (File.Exists(_path))
            {
                File.Delete(_path);
            }

            using (StreamWriter outfile = new StreamWriter(path))
            {
                outfile.WriteLine(fileContent);
            }
            
            AssetDatabase.Refresh();
        }

        private string GetContent()
        {
            _builder = new StringBuilder();
            _indentationLevel = 0;

            WriteLineToBuffer("// File auto-generated from Scylla Generation Module");
            WriteLineToBuffer("// You can modify this class manually");
            WriteLineToBuffer("");
            
            // Namespace start Bracket
            if (string.IsNullOrEmpty(_namespaceName) == false)
            {
                
                WriteLineToBuffer("namespace " + _namespaceName);
                WriteLineToBuffer("{");
                _indentationLevel++;
            }
            
            // Write usings
            if (_usings != null && _usings.Length > 0)
            {
                foreach (string usingValue in _usings)
                {
                    WriteLineToBuffer(usingValue);
                }

                WriteLineToBuffer("");
            }
            
            // Write class attributes
            if (_classAttributes != null && _classAttributes.Length > 0)
            {
                foreach (string attribute in _classAttributes)
                {
                    WriteLineToBuffer(attribute);
                }
            }

            // Write class Line
            WriteLineToBuffer(GetClassLine());
            WriteLineToBuffer("{");
            WriteLineToBuffer("}");
            
            // Namespace end Bracket
            if (string.IsNullOrEmpty(_namespaceName) == false)
            {
                _indentationLevel--;
                WriteLineToBuffer("}");
            }
            
            return _builder.ToString();
        }

        private void WriteLineToBuffer(string line)
        {
            StringBuilder builderLineIndented = new StringBuilder();
            for (int i = 0; i < _indentationLevel; i++)
            {
                builderLineIndented.Append("\t");
            }
            builderLineIndented.Append(line);
            
            _builder.AppendLine(builderLineIndented.ToString());
        }

        private string GetClassLine()
        {
            string classNameUpperCase = _className.FirstCharacterToUpper();
            bool hasExtends = false;
            
            StringBuilder builder = new StringBuilder();
            
            // Base class declaration
            builder.Append("public class " + classNameUpperCase);
            
            // Extends
            if (string.IsNullOrEmpty(_extendName) == false)
            {
                hasExtends = true;
                builder.Append(" : " + _extendName);
            }
            
            // Implements
            if (_interfaceNames != null && _interfaceNames.Length > 0)
            {
                if (hasExtends)
                    builder.Append(", ");
                else
                    builder.Append(" : ");

                for (int i = 0; i < _interfaceNames.Length; i++)
                {
                    builder.Append(_interfaceNames[i]);
                        
                    if (i < _interfaceNames.Length - 1)
                        builder.Append(", ");
                }
            }
            
            // Where clauses
            if (_whereClauses != null && _whereClauses.Length > 0)
            {
                foreach (string clause in _whereClauses)
                {
                    builder.Append(clause);
                }
            }
            
            return builder.ToString();
        }
    }
}

