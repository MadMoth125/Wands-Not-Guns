using UnityEngine;
using Obvious.Soap;

namespace Weapons.Events
{
	[CreateAssetMenu(fileName = "scriptable_event_" + nameof(ProjectileHitData), menuName = "Soap/ScriptableEvents/"+ nameof(ProjectileHitData))]
	public class ScriptableEventCollision : ScriptableEvent<ProjectileHitData>
	{
    
	}
}