namespace MyApp.Server.Services;

public class InventoryValidationException : Exception
{
    public InventoryValidationException(string message) : base(message)
    {
    }
}
