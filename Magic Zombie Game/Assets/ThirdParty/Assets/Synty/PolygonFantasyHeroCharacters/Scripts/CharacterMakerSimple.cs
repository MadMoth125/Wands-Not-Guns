using System;
using System.Collections.Generic;
using System.Linq;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace PsychoticLab.Custom
{
	[ExecuteInEditMode]
	public class CharacterMakerSimple : MonoBehaviour
	{
		public GameObject character;

		[Sirenix.OdinInspector.FilePath]
		public string savePath = "Assets/ThirdParty/Assets/Synty/PolygonFantasyHeroCharacters/Prefabs/";
		
		[TabGroup("Character")]
		[Inlined]
		[SerializeField]
		private CharacterObjectGroups characterObjectGroups = new();
		
		[TabGroup("Attachments")]
		[Inlined]
		[SerializeField]
		private CharacterAttachmentGroups characterAttachmentGroups = new();

		[TabGroup("Materials")]
		[Inlined]
		[SerializeField]
		private CharacterMaterial characterMaterial = new();
		
		[Button]
		public void ApplyCharacterDetails()
		{
			var characterRenderers = GetAllCharacterRenderers(character);
			var characterMeshFilters = characterObjectGroups.GetAllCharacterMeshFilters();
			var characterRightMeshNames = new List<string>();
			var characterAttachmentMeshFilters = characterAttachmentGroups.GetAllCharacterMeshFilters();
			var characterRightAttachmentMeshNames = new List<string>();
			var enabledRenderers = new List<SkinnedMeshRenderer>();
			
			characterRenderers.ForEach(smr => smr.gameObject.SetActive(false));
			enabledRenderers.Clear();

			for (int i = 0; i < characterRenderers.Length; i++)
			{
				// Skip head elements if current head disallows them
				if (!characterObjectGroups.AllowsHeadElements)
				{
					if (characterRenderers[i].gameObject.name.Contains("FacialHair"))
					{
						continue;
					}
					
					if (characterRenderers[i].gameObject.name.Contains("Eyebrow"))
					{
						continue;
					}
				}
				
				// Handles body parts (hands, arms, legs)
				for (int j = 0; j < characterMeshFilters.Length; j++)
				{
					if (characterMeshFilters[j] == null) continue;
					string meshFilterName = characterMeshFilters[j].sharedMesh.name.Replace("_Static", "");

					// skip any left side meshes if symmetric parts are enabled
					if (characterObjectGroups.symmetric)
					{
						if (RightSideMesh(meshFilterName))
						{
							// store the right side mesh name for later use
							characterRightMeshNames.Add(meshFilterName);
						}
						else if (LeftSideMesh(meshFilterName))
						{
							continue;
						}
					}
					
					if (meshFilterName == characterRenderers[i].gameObject.name)
					{
						characterRenderers[i].gameObject.SetActive(true);
						enabledRenderers.Add(characterRenderers[i]);
					}
				}
				
				// Handles attachments (hat, cape, knee/elbow attachments)
				for (int j = 0; j < characterAttachmentMeshFilters.Length; j++)
				{
					if (characterAttachmentMeshFilters[j] == null) continue;
					string meshFilterName = characterAttachmentMeshFilters[j].sharedMesh.name.Replace("_Static", "");

					if (characterAttachmentGroups.symmetric)
					{
						if (RightSideMesh(meshFilterName))
						{
							characterRightAttachmentMeshNames.Add(meshFilterName);
						}
						else if (LeftSideMesh(meshFilterName))
						{
							continue;
						}
					}
					
					if (meshFilterName == characterRenderers[i].gameObject.name)
					{
						characterRenderers[i].gameObject.SetActive(true);
						enabledRenderers.Add(characterRenderers[i]);
					}
				}
				
				// Handles symmetric parts (left side meshes)
				if (characterObjectGroups.symmetric)
				{
					for (int j = 0; j < characterRightMeshNames.Count; j++)
					{
						// Skip any parts that aren't left side meshes
						if (!LeftSideMesh(characterRenderers[i].gameObject.name)) continue;
						if (characterRenderers[i].gameObject.name == GetLeftFromRight(characterRightMeshNames[j]))
						{
							characterRenderers[i].gameObject.SetActive(true);
							enabledRenderers.Add(characterRenderers[i]);
						}
					}

					for (int j = 0; j < characterRightAttachmentMeshNames.Count; j++)
					{
						if (!LeftSideMesh(characterRenderers[i].gameObject.name)) continue;
						if (characterRenderers[i].gameObject.name == GetLeftFromRight(characterRightAttachmentMeshNames[j]))
						{
							characterRenderers[i].gameObject.SetActive(true);
							enabledRenderers.Add(characterRenderers[i]);
						}
					}
				}
			}

			if (characterMaterial.globalMaterial != null)
			{
				characterMaterial.ApplyMaterialToSkinnedMeshes(characterRenderers);
			}
			
			return;

			bool RightSideMesh(string meshName)
			{
				return meshName.Contains("Right");
			}
			
			bool LeftSideMesh(string meshName)
			{
				return meshName.Contains("Left");
			}
			
			string GetLeftFromRight(string rightName)
			{
				return rightName.Replace("Right", "Left");
			}
		}

		[Button]
		public void ResetCharacterDetails()
		{
			var characterRenderers = GetAllCharacterRenderers(character);
			characterRenderers.ForEach(smr => smr.gameObject.SetActive(true));
		}
		
		[Button]
		public void SaveCharacterAsPrefab()
		{
			string prefabPath = savePath;
			string prefabName = character.name + UnityEngine.Random.Range(0, 1000) + ".prefab";
			string prefabFullPath = prefabPath + prefabName;
			
			PrefabUtility.SaveAsPrefabAsset(character, prefabFullPath);
		}

		public SkinnedMeshRenderer[] GetAllCharacterRenderers(GameObject character)
		{
			// makes sure to include inactive game objects in the list
			return character.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
		}
		
		#region Unity Methods

		private void Start()
		{
		
		}

		private void Update()
		{
		
		}

		private void OnEnable()
		{
		
		}

		private void OnDisable()
		{
		
		}

		#endregion
	}
	
	[Serializable]
	public class CharacterObjectGroups
	{
		public bool AllowsHeadElements => head != null && !AssetDatabase.GetAssetPath(head).Contains("/HeadNoElement");

		[Tooltip("if true, parts selected for the character's right side will be mirrored on the left side.\n" +
		         "This will also prevent the selection of parts for the character's left side.")]
		public bool symmetric = false;
		
		private const string PATH = "Assets/ThirdParty/Assets/Synty/PolygonFantasyHeroCharacters/Prefabs/Characters_ModularParts_Static/";
		private const string SEARCH_FILTER = "t:Prefab";
		private const int PREVIEW_HEIGHT = 100;
		private const int DROPDOWN_WIDTH = 1100;
		private const ObjectFieldAlignment PREVIEW_ALIGNMENT = ObjectFieldAlignment.Center;
		
		[TitleGroup("Head Elements", Alignment = TitleAlignments.Centered)]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "Head" + "|" + PATH + "HeadNoElement")] // including two paths with the '|' separator
		public GameObject head;
		
		[TitleGroup("Head Elements")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "Eyebrow")]
		[EnableIf(nameof(AllowsHeadElements))]
		public GameObject eyebrow;
	
		[TitleGroup("Head Elements")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "FacialHair")]
		[EnableIf(nameof(AllowsHeadElements))]
		public GameObject facialHair;
		
		[TitleGroup("Body Elements", Alignment = TitleAlignments.Centered)]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "Torso")]
		public GameObject torso;
		
		[TitleGroup("Right Arm Elements", Alignment = TitleAlignments.Centered)]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "ArmUpperRight")]
		public GameObject armUpperRight;
		
		[TitleGroup("Left Arm Elements", Alignment = TitleAlignments.Centered)]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "ArmUpperLeft")]
		[DisableIf(nameof(symmetric))]
		public GameObject armUpperLeft;
		
		[TitleGroup("Right Arm Elements")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "ArmLowerRight")]
		public GameObject armLowerRight;
		
		[TitleGroup("Left Arm Elements")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "ArmLowerLeft")]
		[DisableIf(nameof(symmetric))]
		public GameObject armLowerLeft;
		
		[TitleGroup("Right Arm Elements")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "HandRight")]
		public GameObject handRight;
		
		[TitleGroup("Left Arm Elements")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "HandLeft")]
		[DisableIf(nameof(symmetric))]
		public GameObject handLeft;
		
		[TitleGroup("Body Elements")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "Hips")]
		public GameObject hips;
		
		[TitleGroup("Leg Elements", Alignment = TitleAlignments.Centered)]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "LegRight")]
		public GameObject legRight;
		
		[TitleGroup("Leg Elements")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "LegLeft")]
		[DisableIf(nameof(symmetric))]
		public GameObject legLeft;
		
		public GameObject[] GetAllCharacterParts()
		{
			return new[]
				{
					head,
					eyebrow,
					facialHair,
					torso,
					armUpperRight,
					armUpperLeft,
					armLowerRight,
					armLowerLeft,
					handRight,
					handLeft,
					hips,
					legRight,
					legLeft
				}
				.Where(go => go != null)
				.ToArray();
		}

		public MeshFilter[] GetAllCharacterMeshFilters()
		{
			return GetAllCharacterParts()
				.Select(go => go.GetComponent<MeshFilter>())
				.Where(mf => mf != null)
				.ToArray();
		}
	}

	[Serializable]
	public class CharacterAttachmentGroups
	{
		[Tooltip("if true, parts selected for the character's right side will be mirrored on the left side.\n" +
		         "This will also prevent the selection of parts for the character's left side.")]
		public bool symmetric = false;
		
		private const string PATH = "Assets/ThirdParty/Assets/Synty/PolygonFantasyHeroCharacters/Prefabs/Characters_ModularParts_Static/";
		private const string SEARCH_FILTER = "t:Prefab";
		private const int PREVIEW_HEIGHT = 100;
		private const int DROPDOWN_WIDTH = 900;
		private const ObjectFieldAlignment PREVIEW_ALIGNMENT = ObjectFieldAlignment.Center;

		[TitleGroup("Head Attachments", Alignment = TitleAlignments.Centered)]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "HeadCoveringBase")]
		public GameObject hat;

		[TitleGroup("Head Attachments")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "HeadCoveringNoNoFacialHair")] // typo in the path 'NoNo'
		public GameObject faceMask;

		[TitleGroup("Head Attachments")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "HeadCoveringNoHair")]
		public GameObject headCovering;

		[TitleGroup("Head Attachments")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "Hair")]
		public GameObject hair;

		[TitleGroup("Head Attachments")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "HelmetAttachment")]
		public GameObject helmetAttachment;

		[TitleGroup("Body Attachments", Alignment = TitleAlignments.Centered)]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "BackAttachment")]
		public GameObject backAttachment;

		[TitleGroup("Right Arm Attachments", Alignment = TitleAlignments.Centered)]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "ShoulderAttachmentRight")]
		public GameObject shoulderAttachmentRight;

		[TitleGroup("Left Arm Attachments", Alignment = TitleAlignments.Centered)]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "ShoulderAttachmentLeft")]
		[DisableIf(nameof(symmetric))]
		public GameObject shoulderAttachmentLeft;

		[TitleGroup("Right Arm Attachments")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "ElbowAttachmentRight")]
		public GameObject elbowAttachmentRight;

		[TitleGroup("Left Arm Attachments")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "ElbowAttachmentLeft")]
		[DisableIf(nameof(symmetric))]
		public GameObject elbowAttachmentLeft;

		[FormerlySerializedAs("hipsAttachment")]
		[TitleGroup("Body Attachments")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "HipAttachment")]
		public List<GameObject> hipsAttachments;

		[TitleGroup("Leg Attachments", Alignment = TitleAlignments.Centered)]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "KneeAttachmentRight")]
		public GameObject kneeAttachmentRight;

		[TitleGroup("Leg Attachments")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "KneeAttachmentLeft")]
		[DisableIf(nameof(symmetric))]
		public GameObject kneeAttachmentLeft;

		[TitleGroup("Head Attachments")]
		[PreviewField(Alignment = PREVIEW_ALIGNMENT, Height = PREVIEW_HEIGHT)]
		[AssetSelector(Filter = SEARCH_FILTER,
			FlattenTreeView = true,
			DropdownWidth = DROPDOWN_WIDTH,
			Paths = PATH + "Ears")]
		public GameObject ears;

		public GameObject[] GetAllCharacterParts()
		{
			var parts = new List<GameObject>
			{
				hat,
				faceMask,
				headCovering,
				hair,
				helmetAttachment,
				backAttachment,
				shoulderAttachmentLeft,
				shoulderAttachmentRight,
				elbowAttachmentLeft,
				elbowAttachmentRight,
				kneeAttachmentLeft,
				kneeAttachmentRight,
				ears
			};
			parts.AddRange(hipsAttachments);
			
			return parts
				.Where(go => go != null)
				.ToArray();
		}

		public MeshFilter[] GetAllCharacterMeshFilters()
		{
			return GetAllCharacterParts()
				.Select(go => go.GetComponent<MeshFilter>())
				.Where(mf => mf != null)
				.ToArray();
		}
	}

	[Serializable]
	public class CharacterMaterial
	{
		[InlineEditor(InlineEditorModes.GUIAndHeader)]
		public Material globalMaterial;
		
		public void ApplyMaterialToSkinnedMeshes(IList<SkinnedMeshRenderer> skinnedMeshRenderers)
		{
			foreach (var smr in skinnedMeshRenderers)
			{
				smr.material = globalMaterial;
			}
		}
	}
}