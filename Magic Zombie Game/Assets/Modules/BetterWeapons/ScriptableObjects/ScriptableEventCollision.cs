using UnityEngine;
using Obvious.Soap;

[CreateAssetMenu(fileName = "scriptable_event_" + nameof(ProjectileHitData), menuName = "Soap/ScriptableEvents/"+ nameof(ProjectileHitData))]
public class ScriptableEventCollision : ScriptableEvent<ProjectileHitData>
{
    
}

