using Unity.Collections;
using Unity.Jobs;

public struct SimpleJob : IJob
{
    public float x;
    public NativeArray<float> array;

    public void Execute()
    {
        if (array.Length > 0)
            array[0] = x;
    }
}
