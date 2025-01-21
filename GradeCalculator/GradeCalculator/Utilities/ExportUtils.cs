using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Text;

namespace GradeCalculator.Utilities
{
    // template
    public abstract class ExportHelper
    {
        public FileContentResult GetDownload(object value)
        {
            var file = Serialize(value);
            var bytes = ToBytes(file);
            return GetResult(bytes);
        }

        public abstract FileContentResult GetResult(byte[] bytes);

        public abstract byte[] ToBytes(string serialized);

        public abstract string Serialize(object value);
    }

    // singleton
    public class JsonExportHelper : ExportHelper
    {
        private const string FILE_IS_NOT_JSON_ERROR = "File is not json and can't be serialized.";
        private const string FILE_NAME = "ocjene.json";

        private static readonly Lazy<JsonExportHelper> instance = new Lazy<JsonExportHelper>(() => new JsonExportHelper());

        public static JsonExportHelper Instance 
        { 
            get 
            { 
                return instance.Value; 
            } 
        }

        public override FileContentResult GetResult(byte[] bytes)
        {
            return new FileContentResult(bytes, "application/json")
            {
                FileDownloadName = FILE_NAME
            };
        }

        public override byte[] ToBytes(string serialized)
        {
            return Encoding.UTF8.GetBytes(serialized);
        }

        public override string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value) ?? throw new Exception(FILE_IS_NOT_JSON_ERROR);
        }
    }

    // factory
    public static class ExportHelperFactory
    {
        public static ExportHelper GetJsonExportHelper() => JsonExportHelper.Instance;
    }
}
