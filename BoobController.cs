using ExtensibleSaveFormat;
using KKAPI;
using KKAPI.Chara;
using KKAPI.Maker;
using System.Collections.Generic;
using UnityEngine;

namespace HS2_BoobSettings
{
	public class BoobController : CharaCustomFunctionController
	{
		public const string BUTT = "butt_";

		public const string OVERRIDE_PHYSICS = "overridePhysics";
		public const string DAMPING = "damping";
		public const string ELASTICITY = "elasticity";
		public const string STIFFNESS = "stiffness";
		public const string INERT = "inert";

		public const string OVERRIDE_GRAVITY = "overrideGravity";
		public const string GRAVITY_X = "gravityX";
		public const string GRAVITY_Y = "gravityY";
		public const string GRAVITY_Z = "gravityZ";

		public static Dictionary<string, string> makerAPILabels = new Dictionary<string, string>()
		{
			{ OVERRIDE_PHYSICS, "Override Breast Physics" },
			{ DAMPING, "Breast Damping" },
			{ ELASTICITY, "Breast Elasticity" },
			{ STIFFNESS, "Breast Stiffness" },
			{ INERT, "Breast Inert" },

			{ OVERRIDE_GRAVITY, "Override Breast Gravity" },
			{ GRAVITY_X, "Breast Gravity X" },
			{ GRAVITY_Y, "Breast Gravity Y" },
			{ GRAVITY_Z, "Breast Gravity Z" },

			{ BUTT + OVERRIDE_PHYSICS, "Override Butt Physics" },
			{ BUTT + DAMPING, "Butt Damping" },
			{ BUTT + ELASTICITY, "Butt Elasticity" },
			{ BUTT + STIFFNESS, "Butt Stiffness" },
			{ BUTT + INERT, "Butt Inert" },

			{ BUTT + OVERRIDE_GRAVITY, "Override Butt Gravity" },
			{ BUTT + GRAVITY_X, "Butt Gravity X" },
			{ BUTT + GRAVITY_Y, "Butt Gravity Y" },
			{ BUTT + GRAVITY_Z, "Butt Gravity Z" }
		};

		public static Dictionary<string, bool> boolDefaults = new Dictionary<string, bool>()
		{
			{ OVERRIDE_PHYSICS, false },
			{ OVERRIDE_GRAVITY, false },

			{ BUTT + OVERRIDE_PHYSICS, false },
			{ BUTT + OVERRIDE_GRAVITY, false }
		};

		public static Dictionary<string, float> floatDefaults = new Dictionary<string, float>()
		{
			{ DAMPING, 0.14f },
			{ ELASTICITY, 0.17f },
			{ STIFFNESS, 0.5f },
			{ INERT, 0.8f },
			{ GRAVITY_X, 0f },
			{ GRAVITY_Y, -0.0003f },
			{ GRAVITY_Z, 0f },

			{ BUTT + DAMPING, 0.01f },
			{ BUTT + ELASTICITY, 0.1f },
			{ BUTT + STIFFNESS, 0.3f },
			{ BUTT + INERT, 0.9f },
			{ BUTT + GRAVITY_X, 0f },
			{ BUTT + GRAVITY_Y, 0f },
			{ BUTT + GRAVITY_Z, 0f }
		};

		public static Dictionary<string, float> floatMults = new Dictionary<string, float>()
		{
			{ GRAVITY_X, 100f },
			{ GRAVITY_Y, 100f },
			{ GRAVITY_Z, 100f }
		};

		public static string[] prefixKeys = new string[]
		{
			string.Empty, // Boob
			BUTT
		};

		public static string[] boolKeys = new string[]
		{
			OVERRIDE_PHYSICS,
			OVERRIDE_GRAVITY
		};

		public static string[] floatKeys = new string[]
		{
			DAMPING,
			ELASTICITY,
			STIFFNESS,
			INERT,
			GRAVITY_X,
			GRAVITY_Y,
			GRAVITY_Z
		};

		public Dictionary<string, bool> boolData;
		public Dictionary<string, float> floatData;

		protected override void Awake()
		{
			boolData = new Dictionary<string, bool>();
			floatData = new Dictionary<string, float>();

			foreach (string prefix in prefixKeys)
			{
				foreach (string boolKey in boolKeys)
				{
					string key = prefix + boolKey;
					boolData[key] = boolDefaults[key];
				}

				foreach (string floatKey in floatKeys)
				{
					string key = prefix + floatKey;
					floatData[key] = floatDefaults[key];
				}
			}

			base.Awake();
		}

		protected override void OnCardBeingSaved(GameMode currentGameMode)
		{
			PluginData data = new PluginData();

			foreach (string prefix in prefixKeys)
			{
				foreach (string boolKey in boolKeys)
				{
					string key = prefix + boolKey;

					if (boolData.TryGetValue(key, out bool value))
						data.data[key] = value;
				}

				foreach (string floatKey in floatKeys)
				{
					string key = prefix + floatKey;

					if (floatData.TryGetValue(key, out float value))
						data.data[key] = value;
				}
			}

			SetExtendedData(data);
		}

		protected override void OnReload(GameMode currentGameMode, bool maintainState)
		{
			Dictionary<string, object> data = GetExtendedData()?.data;

			foreach (string prefix in prefixKeys)
			{
				foreach (string boolKey in boolKeys)
				{
					string key = prefix + boolKey;
					data.TryGetValue(key, out bool value, boolDefaults[key]);
					boolData[key] = value;
				}

				foreach (string floatKey in floatKeys)
				{
					string key = prefix + floatKey;
					data.TryGetValue(key, out float value, floatDefaults[key]);
					floatData[key] = value;
				}
			}

			if (!MakerAPI.InsideMaker)
				return;

			CharacterLoadFlags flags = MakerAPI.GetCharacterLoadFlags();

			if (flags != null && !flags.Body)
				return;

			if (MakerAPI.GetCharacterControl() != ChaControl)
				return;

			HS2_BoobSettings.MakerAPI_Update(this);
		}

		public void UpdateBone(DynamicBone_Ver02 bone)
		{
			if (bone == null)
				return;

			UpdateSoftness(bone);
			UpdateWeight(bone);
		}

		public void UpdateSoftness(DynamicBone_Ver02 bone)
		{
			if (!boolData[OVERRIDE_PHYSICS])
				return;

			DynamicBone_Ver02.Particle particle;

			for (int i = 0; (particle = bone.getParticle(i)) != null; i++)
			{
				particle.Damping = floatData[DAMPING];
				particle.Elasticity = floatData[ELASTICITY];
				particle.Stiffness = floatData[STIFFNESS];
				particle.Inert = floatData[INERT];
			}
		}

		public void UpdateWeight(DynamicBone_Ver02 bone)
		{
			if (!boolData[OVERRIDE_GRAVITY])
				return;

			Vector3 gravity = new Vector3(floatData[GRAVITY_X], floatData[GRAVITY_Y], floatData[GRAVITY_Z]);
			bone.Gravity = gravity;

			foreach (DynamicBone_Ver02.BonePtn v in bone.Patterns)
				v.Gravity = gravity;
		}
	}
}
