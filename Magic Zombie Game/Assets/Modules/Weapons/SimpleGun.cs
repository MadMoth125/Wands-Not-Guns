using Core.HealthSystem;
using Core.Utils;
using MyCustomControls;
using RotaryHeart.Lib.PhysicsExtension;
using UnityEngine;
using UnityEngine.InputSystem;
using PhysicsEx = RotaryHeart.Lib.PhysicsExtension.Physics;

public class SimpleGun : MonoBehaviour
{
	[SerializeField]
	private GameControlsAsset gameControls;
	
	[SerializeField]
	private MouseUtilityAsset mouseUtility;
	
	public LayerMask hitDetectionLayers;
	
	private Vector2 _cursorPosition;
	private Vector3 _cursorWorldPosition;
	
	#region Unity Methods

	private void Start()
	{
		
	}

	private void Update()
	{
		_cursorWorldPosition = mouseUtility.GetMouseWorldPosition(_cursorPosition);
	}

	private void OnEnable()
	{
		gameControls.OnMoveCursorCallback += OnMoveCursor;
		gameControls.OnAttackCallback += OnAttack;
	}

	private void OnDisable()
	{
		gameControls.OnMoveCursorCallback -= OnMoveCursor;
		gameControls.OnAttackCallback -= OnAttack;
	}

	#endregion

	private void OnMoveCursor(InputAction.CallbackContext ctx)
	{
		_cursorPosition = MouseUtils.ClampScreenPosition(ctx.ReadValue<Vector2>());
	}

	private void OnAttack(InputAction.CallbackContext ctx)
	{
		if (ctx.ReadValueAsButton())
		{
			Vector3 direction = (_cursorWorldPosition - transform.position).normalized;
			Vector3 leveledDirection = new Vector3(direction.x, 0f, direction.z);
			if (PhysicsEx.Raycast(
				    transform.position,
				    leveledDirection,
				    out RaycastHit hit,
				    1000f,
				    hitDetectionLayers,
				    PreviewCondition.Editor,
				    1f,
				    Color.green,
				    Color.red))
			{
				hit.collider.gameObject.GetComponent<IDamageable>()?.Kill();
			}
		}
	}
}