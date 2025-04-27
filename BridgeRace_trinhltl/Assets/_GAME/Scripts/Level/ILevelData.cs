namespace _GAME.Scripts.Level
{
    using _GAME.Scripts.Floor;

    public interface ILevelData
    {
        Floor[] GetFloors();
    }
}