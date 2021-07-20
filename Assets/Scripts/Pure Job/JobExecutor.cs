using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public class JobExecutor : MonoBehaviour
{
    private NativeArray<float> currentArray;

    private void Start()
    {
        currentArray = new NativeArray<float>(1, Allocator.TempJob);
        SimpleJob simpleJob = new SimpleJob()
        {
            x = 5,
            array = currentArray
        };
        AnotherJob anotherJob = new AnotherJob()
        {
            array = currentArray
        };

        JobHandle jobHandle = simpleJob.Schedule();
        JobHandle anotherJobHandle = new AnotherJob() { array = currentArray }.Schedule(anotherJob.Schedule(jobHandle));
        anotherJobHandle.Complete();
        print(currentArray[0]);
    }
}