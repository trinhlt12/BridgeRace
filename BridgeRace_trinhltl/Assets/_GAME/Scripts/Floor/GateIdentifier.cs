namespace _GAME.Scripts.Floor
{
    public class GateIdentifier
    {
        public Floor floor;
        public int gateIndex;

        public GateIdentifier(Floor floor, int gateIndex)
        {
            this.floor     = floor;
            this.gateIndex = gateIndex;
        }

        public override bool Equals(object obj)
        {
            if(!(obj is GateIdentifier)) return false;
            var other = (GateIdentifier)obj;
            return this.floor == other.floor && this.gateIndex == other.gateIndex;
        }

        public override int GetHashCode()
        {
            return (this.floor?.GetHashCode() ?? 0) ^ this.gateIndex.GetHashCode();
        }
    }
}