using Godot;
using System.Collections.Generic;

public partial class MoveSelectableMesh : MultiMeshInstance3D
{
    private Dictionary<Vector2I, int> indexLoc;

    public MoveSelectableMesh()
    {
        indexLoc = new();
    }

    ///<summary>
    ///Adds an index with the key of loc to the index location map to keep track of where each index in a multimesh is on the battle map.
    ///</summary>
    ///<param name="loc">The battlemap location that index of the multimesh is at.</param>
    ///<param name="index">The index of the multimesh that is at that battlemap location.</param>
    public void AddIndex(Vector2I loc, int index)
    {
        indexLoc.Add(loc, index);
    }

    ///<summary>
    ///Returns the index of a multimesh at a given battlemap location.
    ///</summary>
    ///<param name="loc">The battlemap location the index is at.</param>
    ///<returns>The multimesh index that is at that battlemap location</returns>
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
