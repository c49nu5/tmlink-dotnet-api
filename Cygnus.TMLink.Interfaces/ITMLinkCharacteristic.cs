namespace Cygnus.TMLink.Interfaces
{
    public interface ITMLinkCharacteristic
    {
        event EventHandler<ValueChangedEventArgs>? CharacteristicValueChanged;

        string Uuid { get; }

        Task<byte[]> ReadValue();
        Task StartNotifications();
        Task WriteValueWithResponse(byte[] bytes);
    }
}