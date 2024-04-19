using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class NearFarValue
{
	public NearFarValue(float near, float far)
	{
		this.near = near;
		this.far = far;
	}

	[HorizontalGroup("Value", 0.5f)]
	[HideLabel]
	[SuffixLabel("Near", Overlay = true)]
	public float near;

	[HorizontalGroup("Value", 0.5f)]
	[HideLabel]
	[SuffixLabel("Far", Overlay = true)]
	public float far;

	#region Operators

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != this.GetType()) return false;
		return Equals((NearFarValue)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(near, far);
	}

	protected bool Equals(NearFarValue other)
	{
		return near.Equals(other.near) && far.Equals(other.far);
	}

	// implicit operators
	public static implicit operator Vector2(NearFarValue value) => new Vector2(value.near, value.far);
	public static implicit operator NearFarValue(Vector2 value) => new NearFarValue(value.x, value.y);
	public static bool operator ==(NearFarValue a, NearFarValue b) => (a != null && b != null) && a.near == b.near && a.far == b.far;
	public static bool operator !=(NearFarValue a, NearFarValue b) => !(a == b);
	public static bool operator ==(NearFarValue a, Vector2 b) => (a != null) && a.near == b.x && a.far == b.y;
	public static bool operator !=(NearFarValue a, Vector2 b) => !(a == b);
	public static bool operator ==(Vector2 a, NearFarValue b) => (b != null) && a.x == b.near && a.y == b.far;
	public static bool operator !=(Vector2 a, NearFarValue b) => !(a == b);

	#endregion
}