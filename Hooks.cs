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

			if (controller == null)
				return true;

			bool flag = true;

			if (controller.boolData[BoobController.OVERRIDE_PHYSICS])
			{
				flag = false;

				float damping = controller.floatData[BoobController.DAMPING];
				float elasticity = controller.floatData[BoobController.ELASTICITY];
				float stiffness = controller.floatData[BoobController.STIFFNESS];

				Util.UpdateBustSoftness(
					___chaCtrl.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.BreastL),
					changePtn,
					damping,
					elasticity,
					stiffness
				);
				Util.UpdateBustSoftness(
					___chaCtrl.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.BreastR),
					changePtn,
					damping,
					elasticity,
					stiffness
				);
				// Inert rarely gets updated. This makes sure it does.
				___chaCtrl.ChangeBustInert(false);
			}

			if (controller.boolData[BoobController.BUTT + BoobController.OVERRIDE_PHYSICS])
			{
				float damping = controller.floatData[BoobController.BUTT + BoobController.DAMPING];
				float elasticity = controller.floatData[BoobController.BUTT + BoobController.ELASTICITY];
				float stiffness = controller.floatData[BoobController.BUTT + BoobController.STIFFNESS];
				float inert = controller.floatData[BoobController.BUTT + BoobController.INERT];

				Util.UpdateButtPhysics(
					___chaCtrl.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.HipL),
					damping,
					elasticity,
					stiffness,
					inert
				);
				Util.UpdateButtPhysics(
					___chaCtrl.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.HipR),
					damping,
					elasticity,
					stiffness,
					inert
				);
			}

			return flag;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(ChaControl), "ChangeBustInert")]
		public static bool PatchPrefix_ChaControl_ChangeBustInert(ChaControl __instance, bool h)
		{
			BoobController controller = __instance.GetComponent<BoobController>();

			if (controller == null ||
				!controller.boolData[BoobController.OVERRIDE_PHYSICS])
				return true;

			float inert = controller.floatData[BoobController.INERT];

			Util.UpdateBustInert(
				__instance.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.BreastL),
				inert
			);
			Util.UpdateBustInert(
				__instance.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.BreastR),
				inert
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

			if (controller == null)
				return true;

			bool flag = true;

			if (controller.boolData[BoobController.OVERRIDE_GRAVITY])
			{
				flag = false;
				Vector3 gravity = new Vector3(
					controller.floatData[BoobController.GRAVITY_X],
					controller.floatData[BoobController.GRAVITY_Y],
					controller.floatData[BoobController.GRAVITY_Z]
				);

				Util.UpdateBustGravity(
					___chaCtrl.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.BreastL),
					changePtn,
					gravity
				);
				Util.UpdateBustGravity(
					___chaCtrl.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.BreastR),
					changePtn,
					gravity
				);
			}


			// Butt

			if (controller.boolData[BoobController.BUTT + BoobController.OVERRIDE_GRAVITY])
			{
				Vector3 gravity = new Vector3(
					controller.floatData[BoobController.BUTT + BoobController.GRAVITY_X],
					controller.floatData[BoobController.BUTT + BoobController.GRAVITY_Y],
					controller.floatData[BoobController.BUTT + BoobController.GRAVITY_Z]
				);

				Util.UpdateButtGravity(
					___chaCtrl.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.HipL),
					gravity
				);
				Util.UpdateButtGravity(
					___chaCtrl.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.HipR),
					gravity
				);
			}

			return flag;
		}
	}
}
