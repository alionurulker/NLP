using Newtonsoft.Json;

namespace NLP.Model.ViewModel
{
    public class NLPViewModel
    {
        [JsonProperty("Output")]
        public string Output { get; set; }
    }
}
