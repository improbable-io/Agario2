using System;
using Newtonsoft.Json;

namespace Improbable.Unity.Core
{
    internal class ProjectDescriptor
    {
        [JsonProperty("name")]
        public string Name;

        public static ProjectDescriptor Load(string path) {
            var descriptorText = System.IO.File.ReadAllText(path);
            try
            {
	            return JsonConvert.DeserializeObject<ProjectDescriptor>(descriptorText);
	        }
	        catch (JsonReaderException e)
	        {
	        	throw new FormatException(String.Format("Failed deserializing project descriptor from {0}", path), e);
	        }
        }
    }
}