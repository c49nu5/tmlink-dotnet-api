namespace Cygnus.BLE.Interfaces
{
    public interface IBLECharacteristic
    {
        EventHandler<BLECharacteristicValueChangedEventArgs> CharacteristicValueChanged { get; set; }
        string? Uuid { get; }

        Task<byte[]?> ReadValue();
        Task StartNotifications();
        Task WriteValueWithResponse(byte[] bytes);
    }
}