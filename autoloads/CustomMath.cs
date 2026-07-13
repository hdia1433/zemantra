using Godot;
using System.Numerics;

public partial class CustomMath : Node
{
    public static CustomMath Inst {get; private set;}

    public CustomMath()
    {
        Inst = this;
    }

    public T Square<T>(T value) where T: INumber<T>
    {
        return value * value;
    }
}
