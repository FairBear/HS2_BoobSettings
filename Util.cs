using AIProject;
using System;
using UnityEngine;

namespace HS2_BoobSettings
{
	public static class Util
	{
		public static void UpdateBustSoftness(DynamicBone_Ver02 bone,
											  int[] changePtn,
											  float damping,
											  float elasticity,
											  float stiffness)
		{
			if (bone == null)
				return;

			foreach (int ptn in changePtn)
				bone.setSoftParams(ptn, -1, damping, elasticity, stiffness, true);
		}

		public static void UpdateBustInert(DynamicBone_Ver02 bone, float inert)
		{
			if (bone == null)
				return;

			bone.setSoftParamsEx(0, -1, inert, true);
			bone.ResetPosition();
		}

		public static void UpdateBustGravity(DynamicBone_Ver02 bone,
											 int[] changePtn,
											 Vector3 gravity)
		{
			if (bone == null)
				return;

			foreach (int ptn in changePtn)
				bone.setGravity(ptn, gravity, true);
		}

		public static void UpdateButtPhysics(DynamicBone_Ver02 bone,
											 float damping,
											 float elasticity,
											 float stiffness,
											 float inert)
		{
			if (bone == null)
				return;

			bone.setSoftParams(0, -1, damping, elasticity, stiffness, true);
			bone.setSoftParamsEx(0, -1, inert, true);
			bone.ResetPosition();
		}

		public static void UpdateButtGravity(DynamicBone_Ver02 bone, Vector3 gravity)
		{
			if (bone == null)
				return;

			bone.setGravity(0, gravity, true);
		}

		public static void ResetButtPhysics(DynamicBone_Ver02 bone)
		{
			UpdateButtPhysics(
				bone,
				BoobController.floatDefaults[BoobController.BUTT + BoobController.DAMPING],
				BoobController.floatDefaults[BoobController.BUTT + BoobController.ELASTICITY],
				BoobController.floatDefaults[BoobController.BUTT + BoobController.STIFFNESS],
				BoobController.floatDefaults[BoobController.BUTT + BoobController.INERT]
			);
		}

		public static void ResetButtGravity(DynamicBone_Ver02 bone)
		{
			UpdateButtGravity(bone, new Vector3(
				BoobController.floatDefaults[BoobController.BUTT + BoobController.GRAVITY_X],
				BoobController.floatDefaults[BoobController.BUTT + BoobController.GRAVITY_Y],
				BoobController.floatDefaults[BoobController.BUTT + BoobController.GRAVITY_Z]
			));
		}
	}
}
