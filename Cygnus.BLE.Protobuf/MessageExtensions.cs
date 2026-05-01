using System.IO.Compression;

namespace Cygnus.BLE.Protobuf
{
    public static class MessageExtensions
    {
        public static T FromZippedProtoBuf<T>(this byte[] data)
        {
            using MemoryStream unzippedStream = new();
            using (MemoryStream compressedData = new(data))
            using (GZipStream decompressor = new(compressedData, CompressionMode.Decompress))
            {
                decompressor.CopyTo(unzippedStream);
            }

            unzippedStream.Seek(0, SeekOrigin.Begin);
            byte[] protobufData = unzippedStream.ToArray();
            return protobufData.FromProtobuf<T>();
        }

        public static T FromProtobuf<T>(this byte[] protobufData)
        {
            return ProtoBuf.Serializer.Deserialize<T>(protobufData);
        }

        public static byte[] ToZippedProtobuf<T>(this T gaugeRecord)
        {
            byte[] protobufData = gaugeRecord.ToProtobuf();
            using MemoryStream recordZip = new MemoryStream();
            using (GZipStream compressor = new GZipStream(recordZip, CompressionMode.Compress))
            {
                compressor.Write(protobufData);
            }

            return recordZip.ToArray();
        }

        public static byte[] ToProtobuf<T>(this T gaugeRecord)
        {
            using MemoryStream data = new MemoryStream();
            ProtoBuf.Serializer.Serialize(data, gaugeRecord);
            data.Seek(0, SeekOrigin.Begin);
            return data.ToArray();
        }
    }
}
