using Cygnus.BLE.Protobuf.Interfaces;
using System.IO.Compression;

namespace Cygnus.BLE.Protobuf.Services
{
    public class ProtobufMessageConverter : IProtobufMessageConverter
    {
        public T FromZippedProtoBuf<T>(byte[] data)
        {
            using MemoryStream unzippedStream = new();
            using (MemoryStream compressedData = new(data))
            using (GZipStream decompressor = new(compressedData, CompressionMode.Decompress))
            {
                decompressor.CopyTo(unzippedStream);
            }

            unzippedStream.Seek(0, SeekOrigin.Begin);
            byte[] protobufData = unzippedStream.ToArray();
            return FromProtobuf<T>(protobufData);
        }

        public T FromProtobuf<T>(byte[] protobufData)
        {
            return ProtoBuf.Serializer.Deserialize<T>(protobufData);
        }

        public byte[] ToZippedProtobuf<T>(T message)
        {
            byte[] protobufData = ToProtobuf(message);
            using MemoryStream recordZip = new MemoryStream();
            using (GZipStream compressor = new GZipStream(recordZip, CompressionMode.Compress))
            {
                compressor.Write(protobufData);
            }

            return recordZip.ToArray();
        }

        public byte[] ToProtobuf<T>(T message)
        {
            using MemoryStream data = new MemoryStream();
            ProtoBuf.Serializer.Serialize(data, message);
            data.Seek(0, SeekOrigin.Begin);
            return data.ToArray();
        }
    }
}
