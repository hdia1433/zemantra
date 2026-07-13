using Godot;
using System.Collections.Generic;

public partial class MoveSelectableMesh : MultiMeshInstance3D
{
    private Dictionary<Vector2I, int> indexLoc;

    public MoveSelectableMesh()
    {
        indexLoc = new();
    }

    public void AddIndex(Vector2I loc, int index)
    {
        indexLoc.Add(loc, index);
    }

    public int LocToIndex(Vector2I loc)
    {
        return indexLoc[loc];
    }

    public void clearIndecis()
    {
        indexLoc.Clear();
        Multimesh.InstanceCount = 0;
    }
}
