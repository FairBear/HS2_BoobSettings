using AIChara;
using HarmonyLib;
using UnityEngine;

namespace HS2_BoobSettings
{
	public partial class HS2_BoobSettings
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(BustSoft), "ReCalc")]
		public static bool PatchPrefix_BustSoft_ReCalc(ChaControl ___chaCtrl, ChaInfo ___info, int[] changePtn)
		{
			if (___chaCtrl == null ||
				___info == null ||
				changePtn.Length == 0)
				return false;

			BoobController controller = ___chaCtrl.GetComponent<BoobController>();

			if (controller == null ||
				!controller.overridePhysics)
				return true;

			Util.UpdateSoftness(
				___chaCtrl.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.BreastL),
				changePtn,
				controller.damping,
				controller.elasticity,
				controller.stiffness
			);
			Util.UpdateSoftness(
				___chaCtrl.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.BreastR),
				changePtn,
				controller.damping,
				controller.elasticity,
				controller.stiffness
			);
			// Inert rarely gets updated. This makes sure it does.
			___chaCtrl.ChangeBustInert(false);

			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(ChaControl), "ChangeBustInert")]
		public static bool PatchPrefix_ChaControl_ChangeBustInert(ChaControl __instance, bool h)
		{
			BoobController controller = __instance.GetComponent<BoobController>();

			if (controller == null ||
				!controller.overridePhysics)
				return true;

			Util.UpdateInert(
				__instance.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.BreastL),
				controller.inert
			);
			Util.UpdateInert(
				__instance.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.BreastR),
				controller.inert
			);

			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(BustGravity), "ReCalc")]
		public static bool PatchPrefix_BustGravity_ReCalc(ChaControl ___chaCtrl, ChaInfo ___info, int[] changePtn)
		{
			if (___chaCtrl == null ||
				___info == null ||
				changePtn.Length == 0)
				return false;

			BoobController controller = ___chaCtrl.GetComponent<BoobController>();

			if (controller == null ||
				!controller.overrideGravity)
				return true;

			Vector3 gravity = new Vector3(
				controller.gravityX,
				controller.gravityY,
				controller.gravityZ
			);

			Util.UpdateGravity(
				___chaCtrl.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.BreastL),
				changePtn,
				gravity
			);
			Util.UpdateGravity(
				___chaCtrl.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.BreastR),
				changePtn,
				gravity
			);

			return false;
		}
	}
}
