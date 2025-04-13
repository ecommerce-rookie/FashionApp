using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Infrastructure.Storage.Cloudinary.Internals
{
    public static class CloudinaryOptions
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ImageFormat
        {
            jpg,
            png,
            gif,
            bmp,
            tiff,
            webp
        }

        public enum ImageSize
        {
            Avatar,
            Thumbnail,
            Another
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum TypeResize
        {
            Crop,
            Pad,
            BoxPad,
            Max,
            Min,
            Stretch,
            Manual
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ImageFolder
        {
            [Description("avatar")]
            Avatar,
            [Description("product")]
            Product,
            [Description("test")]
            Test,
        }
    }
}
