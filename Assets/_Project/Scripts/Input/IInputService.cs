namespace RunRich.Input
{
    public interface IInputService
    {
        float HorizontalDelta { get; }
        bool PressedThisFrame { get; }
    }
}
