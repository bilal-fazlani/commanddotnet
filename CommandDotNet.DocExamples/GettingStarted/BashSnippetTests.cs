using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CommandDotNet.DocExamples.GettingStarted
{
    [TestFixture]
    public class BashSnippetTests
    {
        public static List<BashSnippet> Snippets { get; } = Assembly.GetExecutingAssembly()
            .ExportedTypes
            .SelectMany(t => t.GetSnippets())
            .ToList();

        public static List<TestCaseData> SnippetCaseData { get; } = Snippets
            .Select(s => new TestCaseData(s) { TestName = s.MemberName })
            .ToList();

        [Test]
        public void NoDupes()
        {
            var dupes = string.Join(Environment.NewLine,
                Snippets
                    .GroupBy(s => s.Name)
                    .Where(g => g.Count() > 1)
                    .Select(g =>
                        $"{g} {string.Join(",", g.Select(s => s.MemberName))}"));

            dupes.Should().BeNullOrEmpty("snippet names are duplicated");
        }

        [TestCaseSource(nameof(SnippetCaseData))]
        public void VerirySnippets(BashSnippet snippet) => snippet.VerifySnippet();


        [OneTimeTearDown]
        public void Teardown()
        {
            var projectDirectory = new DirectoryInfo(Environment.CurrentDirectory).Parent!.Parent!.Parent!;
            var snippetDirectory = new DirectoryInfo(Path.Combine(projectDirectory.FullName, "BashSnippets"));

            if (!snippetDirectory.Exists) snippetDirectory.Create();

            Dictionary<string, SnippetFile> files = snippetDirectory
                .EnumerateFiles()
                .Select(i => new SnippetFile(i))
                .ToDictionary(f => f.Name);

            bool MatchesFileContents(BashSnippet bashSnippet)
            {
                return files.Remove(bashSnippet.Name, out var file)
                       && file.IsSameSize(bashSnippet.FileText)
                       && file.GetText() == bashSnippet.FileText;
            }

            foreach (var snippet in Snippets)
            {
                if (MatchesFileContents(snippet))
                {
                    // nothing to change
                    continue;
                }

                new SnippetFile(snippetDirectory, snippet.Name)
                    .SaveText(snippet.FileText);
            }

            foreach (var file in files.Values)
            {
                file.Info.Delete();
            }
        }

        private class SnippetFile
        {
            public FileInfo Info { get; }
            public string Name { get; }

            public SnippetFile(DirectoryInfo dir, string snippetName)
            {
                Name = snippetName;
                Info = new FileInfo(Path.Combine(dir.FullName, $"{snippetName}.bash"));
            }

            public SnippetFile(FileInfo info)
            {
                Info = info;
                Name = Path.GetFileNameWithoutExtension(info.Name);
            }

            public bool IsSameSize(string text) => Info.Length == text.Length;

            public string GetText() => File.ReadAllText(Info.FullName);
            public void SaveText(string text) => File.WriteAllText(Info.FullName, text);
        }
    }
}