namespace MrCheater.Domain.Models;

public class ControllerStateSnapshot
{
    public bool IsConnected { get; set; }
    public int UserIndex { get; set; }
    public string ControllerType { get; set; } = "Unknown";

    public bool DPadUp { get; set; }
    public bool DPadDown { get; set; }
    public bool DPadLeft { get; set; }
    public bool DPadRight { get; set; }

    public bool Start { get; set; }
    public bool Back { get; set; }
    public bool LeftThumb { get; set; }
    public bool RightThumb { get; set; }

    public bool LeftShoulder { get; set; }
    public bool RightShoulder { get; set; }

    public bool A { get; set; }
    public bool B { get; set; }
    public bool X { get; set; }
    public bool Y { get; set; }

    public byte LeftTrigger { get; set; }
    public byte RightTrigger { get; set; }

    public short ThumbLX { get; set; }
    public short ThumbLY { get; set; }
    public short ThumbRX { get; set; }
    public short ThumbRY { get; set; }

    public List<string> GetActiveButtonNames()
    {
        var list = new List<string>();
        if (DPadUp) list.Add("DPadUp");
        if (DPadDown) list.Add("DPadDown");
        if (DPadLeft) list.Add("DPadLeft");
        if (DPadRight) list.Add("DPadRight");

        if (A) list.Add("A");
        if (B) list.Add("B");
        if (X) list.Add("X");
        if (Y) list.Add("Y");

        if (LeftShoulder) list.Add("LeftShoulder");
        if (RightShoulder) list.Add("RightShoulder");
        if (LeftTrigger > 30) list.Add("LeftTrigger");
        if (RightTrigger > 30) list.Add("RightTrigger");

        if (Start) list.Add("Start");
        if (Back) list.Add("Back");
        if (LeftThumb) list.Add("LeftThumb");
        if (RightThumb) list.Add("RightThumb");

        return list;
    }
}
