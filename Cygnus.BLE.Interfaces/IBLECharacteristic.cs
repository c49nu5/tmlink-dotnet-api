namespace Cygnus.BLE.Interfaces
{
    public interface IBLECharacteristic
    {
        event EventHandler<BLECharacteristicValueChangedEventArgs>? CharacteristicValueChanged;

        string? Uuid { get; }

        Task<byte[]?> ReadValue();
        Task StartNotifications();
        Task WriteValueWithResponse(byte[] bytes);
    }
}