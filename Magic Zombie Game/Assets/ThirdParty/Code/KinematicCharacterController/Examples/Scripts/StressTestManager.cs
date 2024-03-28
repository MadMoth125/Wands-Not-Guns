﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KinematicCharacterController.Examples
{
	public class StressTestManager : MonoBehaviour
	{
		public Camera Camera;
		public LayerMask UIMask;

		public InputField CountField;
		public Image RenderOn;
		public Image SimOn;
		public Image InterpOn;
		public ExampleCharacterController CharacterPrefab;
		public ExampleAIController AIController;
		public int SpawnCount = 100;
		public float SpawnDistance = 2f;

		private void Start()
		{
			KinematicCharacterSystem.EnsureCreation();
			CountField.text = SpawnCount.ToString();
			UpdateOnImages();

			KinematicCharacterSystem.Settings.AutoSimulation = false;
			KinematicCharacterSystem.Settings.Interpolate = false;
		}

		private void Update()
		{

			KinematicCharacterSystem.Simulate(Time.deltaTime, KinematicCharacterSystem.CharacterMotors, KinematicCharacterSystem.PhysicsMovers);
		}

		private void UpdateOnImages()
		{
			RenderOn.enabled = Camera.cullingMask == -1;
			/* Obsolete implementation, replaced by the following line
			 * SimOn.enabled = Physics.autoSimulation;
			 */
			SimOn.enabled = Physics.simulationMode != SimulationMode.Script;
			InterpOn.enabled = KinematicCharacterSystem.Settings.Interpolate;
		}

		public void SetSpawnCount(string count)
		{
			if (int.TryParse(count, out int result))
			{
				SpawnCount = result;
			}
		}

		public void ToggleRendering()
		{
			if(Camera.cullingMask == -1)
			{
				Camera.cullingMask = UIMask;
			}
			else
			{
				Camera.cullingMask = -1;
			}
			UpdateOnImages();
		}

		public void TogglePhysicsSim()
		{
			/* Obsolete implementation, replaced by the following line
			 * Physics.autoSimulation = !Physics.autoSimulation;
			 */
			Physics.simulationMode = Physics.simulationMode == SimulationMode.Script ? SimulationMode.FixedUpdate : SimulationMode.Script;
			UpdateOnImages();
		}

		public void ToggleInterpolation()
		{
			KinematicCharacterSystem.Settings.Interpolate = !KinematicCharacterSystem.Settings.Interpolate;
			UpdateOnImages();
		}

		public void Spawn()
		{
			for (int i = 0; i < AIController.Characters.Count; i++)
			{
				Destroy(AIController.Characters[i].gameObject);
			}
			AIController.Characters.Clear();

			int charsPerRow = Mathf.CeilToInt(Mathf.Sqrt(SpawnCount));
			Vector3 firstPos = ((charsPerRow * SpawnDistance) * 0.5f) * -Vector3.one;
			firstPos.y = 0f;

			for (int i = 0; i < SpawnCount; i++)
			{
				int row = i / charsPerRow;
				int col = i % charsPerRow;
				Vector3 pos = firstPos + (Vector3.right * row * SpawnDistance) + (Vector3.forward * col * SpawnDistance);

				ExampleCharacterController newChar = Instantiate(CharacterPrefab);
				newChar.Motor.SetPosition(pos);

				AIController.Characters.Add(newChar);
			}
		}
	}
}