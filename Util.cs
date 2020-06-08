using UnityEngine;

namespace HS2_BoobSettings
{
	public static class Util
	{
		public static void UpdateSoftness(DynamicBone_Ver02 bone,
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

		public static void UpdateInert(DynamicBone_Ver02 bone, float inert)
		{
			if (bone == null)
				return;

			bone.setSoftParamsEx(0, -1, inert, true);
			bone.ResetPosition();
		}

		public static void UpdateGravity(DynamicBone_Ver02 bone,
										int[] changePtn,
										Vector3 gravity)
		{
			if (bone == null)
				return;

			foreach (int ptn in changePtn)
				bone.setGravity(ptn, gravity, true);
		}
	}
}
