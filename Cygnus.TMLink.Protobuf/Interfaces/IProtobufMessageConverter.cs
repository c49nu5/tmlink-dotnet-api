namespace Cygnus.TMLink.Protobuf.Interfaces
{
    public interface IProtobufMessageConverter
    {
        T FromProtobuf<T>(byte[] protobufData);
        T FromZippedProtobuf<T>(byte[] data);
        byte[] ToProtobuf<T>(T message);
        byte[] ToZippedProtobuf<T>(T message);
    }
}