using GradeCalculator.Models;
using GradeCalculator.Repository;
using Newtonsoft.Json;

namespace GradeCalculator.Utilities
{
    public class FileImporter
    {
        internal IRepository<Godina>? Repo { get; private set; }
        internal IFormFile? File { get; private set; }
        internal FileType? ImportFileType { get; private set; }

        internal FileImporter() { }

        internal void SetRepository(IRepository<Godina> repo)
        {
            Repo = repo;
        }

        internal void SetImportFileType(FileType type)
        {
            ImportFileType = type;
        }

        internal void SetImportFile(IFormFile file)
        {
            File = file;
        }

        public void ImportFile()
        {
            using (var stream = new StreamReader(File.OpenReadStream()))
            {
                var fileString = stream.ReadToEnd();

                List<Godina> years;

                switch (ImportFileType)
                {
                    case FileType.Json:
                        years = JsonConvert.DeserializeObject<List<Godina>>(fileString) ?? new List<Godina>();
                        if (years != null && years is List<Godina>)
                        {
                            years.ForEach(y => Repo?.Add(y) );
                        }
                        break;
                    case FileType.Xml:
                        break;
                    case FileType.Csv:
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // builder
    public class FileImporterBuilder
    {
        private FileImporter _fileImporter = new FileImporter();

        public FileImporterBuilder SetRepository(IRepository<Godina> repo)
        {
            _fileImporter.SetRepository(repo);
            return this;
        }

        public FileImporterBuilder SetImportFile(IFormFile file)
        {
            _fileImporter.SetImportFile(file);
            return this;
        }

        public FileImporterBuilder SetImportFileType(FileType type)
        {
            _fileImporter.SetImportFileType(type);
            return this;
        }

        public FileImporter Build()
        {
            return _fileImporter;
        }
    }

    public enum FileType
    {
        Json, Xml, Csv
    }
}
