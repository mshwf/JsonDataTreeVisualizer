using System.Collections.Generic;

namespace JsonDataTreeVisualizer
{
    //I use this website to validate and minify JSON: https://jsonformatter.org/
    //and this website to stringify JSON: https://onlinetexttools.com/json-stringify-text
    public static class JsonSamplesData
    {

        public static readonly List<JsonSample> Samples = new()
        {
            new JsonSample { Id = "1", Name = "Simple object", Value = Sample1 },
            new JsonSample { Id = "2", Name = "Nested object", Value = Sample2 },
            new JsonSample { Id = "3", Name = "Key value object", Value = Sample3 },
            new JsonSample { Id = "4", Name = "JSON data types", Value = JsonTypes },
            new JsonSample { Id = "5", Name = "Multi level object", Value = ComplexMultiLevelJson },
            new JsonSample { Id = "6", Name = "Invalid JSON", Value = InvalidJson },
            new JsonSample { Id = "7", Name = "Simple array", Value = SimpleArray },
            new JsonSample { Id = "8", Name = "Nested arrays", Value = NestedArrays },
            new JsonSample { Id = "9", Name = "Object with array", Value = ObjectWithArraySample },
            new JsonSample { Id = "10", Name = "Array of objects", Value = ArraySample },
            new JsonSample { Id = "11", Name = "Array inside object", Value = ObjectContainsArrayOfValues },
            new JsonSample { Id = "12", Name = "Swagger doc sample", Value = SwaggerDocSample },
        };

        public const string Sample1 = "{\"Application Name\":\"Json Data Tree Visualizer\",\"Supported types\":6}";
        public const string Sample2 = "{\"Info\":{\"Name\":\"Mohamed\",\"WorkingFromHome\":false,\"Height\":189,\"Occupation\":{\"Job\":\"Software developer\",\"Company\":\"Company X\"}}}";
        public const string Sample3 = "{\"Application Name\":\"JsonDataTreeVisualizer\"}";
        public const string ComplexMultiLevelJson = "{\"Application Name\":\"JsonDataTreeVisualizer\",\"Hosts\":{\"Identity\":\"http://ipaddress:2900/api/\",\"Member\":\"http://ipaddress:2700/api/\",\"Verifier\":{\"KeyManager\":\"http://ipaddress:2000/api/\",\"JwtKeySettings\":{\"KeyLifetimeDays\":1},\"IsAuthEnabled\":false}},\"RabbitMQ\":{\"MqEndpoint\":\"http://ipaddress:1567/\",\"Username\":\"guest\",\"Password\":\"guest\"},\"LinkExpiryMins\":5.5}";
        public const string InvalidJson = "Not JSON";
        public const string Inner = "{\"app\":{\"name\":\"JsonDataTreeVisualizer\"}}";
        public const string ObjectWithArraySample = "{\"MyInfo\":{\"name\":\"Mohamed\"},\"addresses\":[{\"city\":\"city 1\",\"buildingNo\":30},{\"city\":\"city 2\",\"buildingNo\":4}]}";
        public const string SwaggerDocSample = "{\"openapi\":\"3.0.1\",\"info\":{\"title\":\"Library API\",\"version\":\"1\"},\"paths\":{\"/api/authors\":{\"get\":{\"tags\":[\"Authors\"],\"responses\":{\"200\":{\"description\":\"Success\",\"content\":{\"text/plain\":{\"schema\":{\"type\":\"array\",\"items\":{\"$ref\":\"#/components/schemas/Author\"}}},\"application/json\":{\"schema\":{\"type\":\"array\",\"items\":{\"$ref\":\"#/components/schemas/Author\"}}}}}}}},\"/api/authors/{authorId}\":{\"get\":{\"tags\":[\"Authors\"],\"parameters\":[{\"name\":\"authorId\",\"in\":\"path\",\"required\":true,\"schema\":{\"type\":\"string\",\"format\":\"uuid\"}}],\"responses\":{\"200\":{\"description\":\"Success\",\"content\":{\"text/plain\":{\"schema\":{\"$ref\":\"#/components/schemas/Author\"}},\"application/json\":{\"schema\":{\"$ref\":\"#/components/schemas/Author\"}}}}}},\"put\":{\"tags\":[\"Authors\"],\"parameters\":[{\"name\":\"authorId\",\"in\":\"path\",\"required\":true,\"schema\":{\"type\":\"string\",\"format\":\"uuid\"}}],\"requestBody\":{\"content\":{\"application/json-patch+json\":{\"schema\":{\"$ref\":\"#/components/schemas/AuthorForUpdate\"}},\"application/json\":{\"schema\":{\"$ref\":\"#/components/schemas/AuthorForUpdate\"}},\"text/json\":{\"schema\":{\"$ref\":\"#/components/schemas/AuthorForUpdate\"}},\"application/*+json\":{\"schema\":{\"$ref\":\"#/components/schemas/AuthorForUpdate\"}}}},\"responses\":{\"200\":{\"description\":\"Success\",\"content\":{\"text/plain\":{\"schema\":{\"$ref\":\"#/components/schemas/Author\"}},\"application/json\":{\"schema\":{\"$ref\":\"#/components/schemas/Author\"}}}}}},\"patch\":{\"tags\":[\"Authors\"],\"parameters\":[{\"name\":\"authorId\",\"in\":\"path\",\"required\":true,\"schema\":{\"type\":\"string\",\"format\":\"uuid\"}}],\"requestBody\":{\"content\":{\"application/json-patch+json\":{\"schema\":{\"type\":\"array\",\"items\":{\"$ref\":\"#/components/schemas/Operation\"}}},\"application/json\":{\"schema\":{\"type\":\"array\",\"items\":{\"$ref\":\"#/components/schemas/Operation\"}}},\"text/json\":{\"schema\":{\"type\":\"array\",\"items\":{\"$ref\":\"#/components/schemas/Operation\"}}},\"application/*+json\":{\"schema\":{\"type\":\"array\",\"items\":{\"$ref\":\"#/components/schemas/Operation\"}}}}},\"responses\":{\"200\":{\"description\":\"Success\",\"content\":{\"text/plain\":{\"schema\":{\"$ref\":\"#/components/schemas/Author\"}},\"application/json\":{\"schema\":{\"$ref\":\"#/components/schemas/Author\"}}}}}}},\"/api/authors/{authorId}/books\":{\"get\":{\"tags\":[\"Books\"],\"parameters\":[{\"name\":\"authorId\",\"in\":\"path\",\"required\":true,\"schema\":{\"type\":\"string\",\"format\":\"uuid\"}}],\"responses\":{\"200\":{\"description\":\"Success\",\"content\":{\"text/plain\":{\"schema\":{\"type\":\"array\",\"items\":{\"$ref\":\"#/components/schemas/Book\"}}},\"application/json\":{\"schema\":{\"type\":\"array\",\"items\":{\"$ref\":\"#/components/schemas/Book\"}}}}}}},\"post\":{\"tags\":[\"Books\"],\"parameters\":[{\"name\":\"authorId\",\"in\":\"path\",\"required\":true,\"schema\":{\"type\":\"string\",\"format\":\"uuid\"}}],\"requestBody\":{\"content\":{\"application/json-patch+json\":{\"schema\":{\"$ref\":\"#/components/schemas/BookForCreation\"}},\"application/json\":{\"schema\":{\"$ref\":\"#/components/schemas/BookForCreation\"}},\"text/json\":{\"schema\":{\"$ref\":\"#/components/schemas/BookForCreation\"}},\"application/*+json\":{\"schema\":{\"$ref\":\"#/components/schemas/BookForCreation\"}}}},\"responses\":{\"200\":{\"description\":\"Success\",\"content\":{\"text/plain\":{\"schema\":{\"$ref\":\"#/components/schemas/Book\"}},\"application/json\":{\"schema\":{\"$ref\":\"#/components/schemas/Book\"}}}}}}},\"/api/authors/{authorId}/books/{bookId}\":{\"get\":{\"tags\":[\"Books\"],\"parameters\":[{\"name\":\"authorId\",\"in\":\"path\",\"required\":true,\"schema\":{\"type\":\"string\",\"format\":\"uuid\"}},{\"name\":\"bookId\",\"in\":\"path\",\"required\":true,\"schema\":{\"type\":\"string\",\"format\":\"uuid\"}}],\"responses\":{\"200\":{\"description\":\"Success\",\"content\":{\"text/plain\":{\"schema\":{\"$ref\":\"#/components/schemas/Book\"}},\"application/json\":{\"schema\":{\"$ref\":\"#/components/schemas/Book\"}}}}}}}},\"components\":{\"schemas\":{\"Author\":{\"type\":\"object\",\"properties\":{\"Id\":{\"type\":\"string\",\"format\":\"uuid\"},\"FirstName\":{\"type\":\"string\",\"nullable\":true},\"LastName\":{\"type\":\"string\",\"nullable\":true}},\"additionalProperties\":false},\"AuthorForUpdate\":{\"type\":\"object\",\"properties\":{\"FirstName\":{\"type\":\"string\",\"nullable\":true},\"LastName\":{\"type\":\"string\",\"nullable\":true}},\"additionalProperties\":false},\"OperationType\":{\"enum\":[0,1,2,3,4,5,6],\"type\":\"integer\",\"format\":\"int32\"},\"Operation\":{\"type\":\"object\",\"properties\":{\"value\":{\"type\":\"object\",\"additionalProperties\":false,\"nullable\":true},\"OperationType\":{\"allOf\":[{\"$ref\":\"#/components/schemas/OperationType\"}],\"readOnly\":true},\"path\":{\"type\":\"string\",\"nullable\":true},\"op\":{\"type\":\"string\",\"nullable\":true},\"from\":{\"type\":\"string\",\"nullable\":true}},\"additionalProperties\":false},\"Book\":{\"type\":\"object\",\"properties\":{\"Id\":{\"type\":\"string\",\"format\":\"uuid\"},\"AuthorFirstName\":{\"type\":\"string\",\"nullable\":true},\"AuthorLastName\":{\"type\":\"string\",\"nullable\":true},\"Title\":{\"type\":\"string\",\"nullable\":true},\"Description\":{\"type\":\"string\",\"nullable\":true}},\"additionalProperties\":false},\"BookForCreation\":{\"type\":\"object\",\"properties\":{\"Title\":{\"type\":\"string\",\"nullable\":true},\"Description\":{\"type\":\"string\",\"nullable\":true}},\"additionalProperties\":false}}}}";
        public const string ArraySample = "[{\"city\":\"city 1\",\"buildingNo\":30},{\"city\":\"city 2\",\"buildingNo\":4}]";
        public const string SimpleArray = "[\"a\",\"b\", 5, true, null]";
        public const string ObjectContainsArrayOfValues = "{\"name\":\"mohamed\",\"addresses\":[\"cairo\",\"sharkia\"]}";
        public const string JsonTypes = "{\"string\":\"_str\",\"array\":[\"first\",\"second\",{\"number\":41},true], \"null\":null, \"object\":{\"key\":\"value\"}}";
        public const string NestedArrays = "[1,[true,[null,[\"deep\"]]]]";
    }

    public class JsonSample
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}