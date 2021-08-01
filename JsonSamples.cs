namespace JsonDataTreeVisualizer
{
    //I use this website to validate and minify JSON: https://jsonformatter.org/
    //and this website to stringify JSON: https://onlinetexttools.com/json-stringify-text
    public static class JsonSamples
    {
        public const string SimpleJson = "{\"Application Name\":\"Json Data Tree Visualizer\",\"Supported types\":4}";
        public const string ComplexJson = "{\"Application Name\":\"JsonDataTreeVisualizer\",\"Hosts\":{\"Identity\":\"http://ipaddress:2900/api/\",\"Member\":\"http://ipaddress:2700/api/\",\"Verifier\":{\"KeyManager\":\"http://ipaddress:2000/api/\",\"JwtKeySettings\":{\"KeyLifetimeDays\":1},\"IsAuthEnabled\":false}},\"IsDevelopment\":true,\"ExpectedRequestsPerSec\":150,\"LinkExpiryMins\":5.5,\"Other\":null}";
        public const string ComplexMultiLevelJson = "{\"Application Name\":\"JsonDataTreeVisualizer\",\"Hosts\":{\"Identity\":\"http://ipaddress:2900/api/\",\"Member\":\"http://ipaddress:2700/api/\",\"Verifier\":{\"KeyManager\":\"http://ipaddress:2000/api/\",\"JwtKeySettings\":{\"KeyLifetimeDays\":1},\"IsAuthEnabled\":false}},\"RabbitMQ\":{\"Host\":\"http://ipaddress:1567/\",\"Username\":\"guest\",\"Password\":\"guest\"},\"LinkExpiryMins\":5.5}";

        public const string InvalidJson = "";

        public const string Inner = "{\"MyInfo\":{\"name\":\"Mohamed\"}}";

        public const string Simple2 = "{\"Info\":{\"Name\":\"Mohamed\",\"WorkingFromHome\":false,\"Height\":189,\"Occupation\":{\"Job\":\"Software developer\",\"Company\":\"GET Group\"}}}";
        public const string Sample3 = "{\"name\":\"Mohamed\"}";
    }
}
