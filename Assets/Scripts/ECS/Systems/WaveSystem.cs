using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;

[AlwaysSynchronizeSystem]
public class WaveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float elapsedTime = (float)Time.ElapsedTime;
        Entities.ForEach((ref Translation trans, in MoveSpeedData msData, in WaveData waveData) =>
        {
            float yPos = waveData.amplitude * math.sin(elapsedTime * msData.Value
                + trans.Value.x * waveData.xOffset + trans.Value.z * waveData.zOffset) + waveData.yOffset;
            trans.Value = new float3(trans.Value.x, yPos, trans.Value.z);
        }).ScheduleParallel();
    }
}
