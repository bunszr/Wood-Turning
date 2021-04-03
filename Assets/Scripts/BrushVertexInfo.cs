[System.Serializable]
public struct BrushVertexInfo
{
    public int bottomIndex;
    public float newVertexY;

    public BrushVertexInfo(int pairIndex, float newEdgePosY)
    {
        this.bottomIndex = pairIndex;
        this.newVertexY = newEdgePosY;
    }
}