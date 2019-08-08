using System;
using System.Collections.Generic;
using System.IO;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.Utils
{
    public class TempFiles : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly List<string> _files = new List<string>();

        public TempFiles(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public string CreateTempFile(params string[] lines)
        {
            var path = CreateOrClearTempFile();
            File.WriteAllLines(path, lines);
            return path;
        }

        public string CreateOrOverwriteTempFile(string fileName, params string[] lines)
        {
            var path = CreateOrClearTempFile(fileName);
            File.WriteAllLines(path, lines);
            return path;
        }

        /// <summary>
        /// If no fileName is provided then one is randomly generated.<br/>
        /// If the fileName is not rooted, it will be created in the users temp dir<br/>
        /// If the file already exists, it will be overwritten with an empy file
        /// </summary>
        public string CreateOrClearTempFile(string fileName = null)
        {
            string filePath;
            if (fileName == null)
            {
                // GetTempFileName creates the file
                filePath = Path.GetTempFileName();
            }
            else
            {
                if (Path.IsPathRooted(fileName))
                {
                    filePath = fileName;
                }
                else
                {
                    var uniqueRoot = Path.Combine(Path.GetTempPath(), "CommandDotNet.Tests");
                    if (!Directory.Exists(uniqueRoot))
                    {
                        Directory.CreateDirectory(uniqueRoot);
                    }
                    filePath = Path.Combine(uniqueRoot, fileName);
                }

                // overwrite if the file exists so we start with clean slate
                // in case a previous test run failed
                File.Create(filePath).Dispose();
            }

            _testOutputHelper.WriteLine($"created temp file: {filePath}");
            _files.Add(filePath);
            return filePath;
        }

        public void Dispose()
        {
            _files.ForEach(DeleteOrForget);
        }

        private void DeleteOrForget(string fileName)
        {
            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception e)
                {
                    _testOutputHelper.WriteLine($"failed to delete temp file: {fileName}. {e.Message}");
                }
            }
        }
    }
}