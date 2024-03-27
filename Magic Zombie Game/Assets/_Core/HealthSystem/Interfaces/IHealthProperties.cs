
using UnityEngine;

public interface IHealthProperties
{
	public GameObject GetOwningGameObject();
	
	public void SetOwningGameObject(GameObject owner);
	
	public float GetHealth();
	
	public void SetHealth(float health);
	
	public float GetMaxHealth();
	
	public void SetMaxHealth(float healthMax, bool fullHealth);
	
	public float GetHealthNormalized();
	
	public void Heal(float healAmount);
	
	public void HealComplete();
	
	public void Damage(float damageAmount);
	
	public bool IsDead();
	
	public void Die();
}