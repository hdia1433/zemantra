using Godot;

public partial class SizeValues : Node
{
    public static SizeValues Inst {get; private set;}

    public int GridSize {get; private set;}
    public float GridSizeM {get; private set;}
    public float GridSizeMHalf {get; private set;}

    public SizeValues()
    {
        Inst = this;

        GridSize = 80;
        GridSizeM = GridSize / 100f;
        GridSizeMHalf = GridSizeM / 2f;
    }
}
