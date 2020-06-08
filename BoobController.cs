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
		public const string OVERRIDE_PHYSICS = "overridePhysics";
		public const string DAMPING = "damping";
		public const string ELASTICITY = "elasticity";
		public const string STIFFNESS = "stiffness";
		public const string INERT = "inert";

		public const string OVERRIDE_GRAVITY = "overrideGravity";
		public const string GRAVITY_X = "gravityX";
		public const string GRAVITY_Y = "gravityY";
		public const string GRAVITY_Z = "gravityZ";

		public bool overridePhysics;
		public float damping;
		public float elasticity;
		public float stiffness;
		public float inert;

		public bool overrideGravity;
		public float gravityX;
		public float gravityY;
		public float gravityZ;

		protected override void OnCardBeingSaved(GameMode currentGameMode)
		{
			PluginData data = new PluginData
			{
				data =
				{
					{ OVERRIDE_PHYSICS, overridePhysics },
					{ DAMPING, damping },
					{ ELASTICITY, elasticity },
					{ STIFFNESS, stiffness },
					{ INERT, inert },

					{ OVERRIDE_GRAVITY, overrideGravity },
					{ GRAVITY_X, gravityX },
					{ GRAVITY_Y, gravityY },
					{ GRAVITY_Z, gravityZ }
				}
			};

			SetExtendedData(data);
		}

		protected override void OnReload(GameMode currentGameMode, bool maintainState)
		{
			Dictionary<string, object> data = GetExtendedData()?.data;

			data.TryGetValue(OVERRIDE_PHYSICS, out overridePhysics, false);
			data.TryGetValue(DAMPING, out damping, 0.14f);
			data.TryGetValue(ELASTICITY, out elasticity, 0.17f);
			data.TryGetValue(STIFFNESS, out stiffness, 0.5f);
			data.TryGetValue(INERT, out inert, 0.8f);

			data.TryGetValue(OVERRIDE_GRAVITY, out overrideGravity, false);
			data.TryGetValue(GRAVITY_X, out gravityX, 0f);
			data.TryGetValue(GRAVITY_Y, out gravityY, -0.0003f);
			data.TryGetValue(GRAVITY_Z, out gravityZ, 0f);

			if (MakerAPI.InsideAndLoaded &&
				MakerAPI.GetCharacterLoadFlags().Body &&
				MakerAPI.GetMakerBase().chaCtrl == ChaControl)
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
			if (!overridePhysics)
				return;

			DynamicBone_Ver02.Particle particle;

			for (int i = 0; (particle = bone.getParticle(i)) != null; i++)
			{
				particle.Damping = damping;
				particle.Elasticity = elasticity;
				particle.Stiffness = stiffness;
				particle.Inert = inert;
			}
		}

		public void UpdateWeight(DynamicBone_Ver02 bone)
		{
			if (!overrideGravity)
				return;

			Vector3 gravity = new Vector3(gravityX, gravityY, gravityZ);
			bone.Gravity = gravity;

			foreach (DynamicBone_Ver02.BonePtn v in bone.Patterns)
				v.Gravity = gravity;
		}
	}
}
