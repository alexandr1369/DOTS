using Unity.Jobs;
using Unity.Collections;

public struct AnotherJob : IJob
{
    public NativeArray<float> array;

    public void Execute()
    {
        if (array.Length > 0)
            array[0] += 1;
    }
}
