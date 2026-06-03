using Krasnoludki.Core.Models;

namespace Krasnoludki.Core.Algorithms;


public class SegmentTree {

    private List<Dwarf> Decametrists;
    private int[] _tree;

    public SegmentTree(List<Dwarf> decametrists)
    {
        this.Decametrists = decametrists;

        int n = decametrists.Count;
        _tree = new int[4 * n];
        Build(0, 0, n - 1);
    }

    public Dwarf GetLoudestDecametrist()
    {
        return GetLoudestDecametrist(0, Decametrists.Count - 1);
    }

    public Dwarf GetLoudestDecametrist(int l, int r)
    {
        int idx = Query(0, 0, Decametrists.Count - 1, l, r);
        return Decametrists[idx];
    }

    private void Build(int node, int start, int end)
    {
        if (start == end)
        {
            _tree[node] = start;
            return;
        }

        int mid = (start + end) / 2;
        Build(2 * node + 1, start, mid);
        Build(2 * node + 2, mid + 1, end);
        _tree[node] = Louder(_tree[2 * node + 1], _tree[2 * node + 2]);
    }

    private int Query(int node, int start, int end, int l, int r)
    {
        if (r < start || end < l)
            return -1;

        if (l <= start && end <= r)
            return _tree[node];

        int mid = (start + end) / 2;
        int leftIdx  = Query(2 * node + 1, start, mid, l, r);
        int rightIdx = Query(2 * node + 2, mid + 1, end, l, r);

        if (leftIdx == -1)  return rightIdx;
        if (rightIdx == -1) return leftIdx;
        return Louder(leftIdx, rightIdx);
    }

    private int Louder(int idxA, int idxB)
    {
        return Decametrists[idxA].getLoudness() >= Decametrists[idxB].getLoudness() ? idxA : idxB;
    }
}