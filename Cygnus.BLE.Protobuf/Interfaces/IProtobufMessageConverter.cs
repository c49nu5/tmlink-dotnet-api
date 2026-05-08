namespace Cygnus.BLE.Protobuf.Interfaces
{
    public interface IProtobufMessageConverter
    {
        T FromProtobuf<T>(byte[] protobufData);
        T FromZippedProtoBuf<T>(byte[] data);
        byte[] ToProtobuf<T>(T message);
        byte[] ToZippedProtobuf<T>(T message);
    }
}