using AIChara;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using System;
using System.Collections.Generic;
using UniRx;

namespace HS2_BoobSettings
{
	public partial class HS2_BoobSettings
	{
		public static Dictionary<string, MakerCategory> categories = new Dictionary<string, MakerCategory>()
		{
			{ string.Empty, MakerConstants.Body.Breast },
			{ BoobController.BUTT, MakerConstants.Body.Lower }
		};

		public static Dictionary<string, MakerToggle> toggles = new Dictionary<string, MakerToggle>();
		public static Dictionary<string, MakerSlider> sliders = new Dictionary<string, MakerSlider>();

		public static ChaControl MakerChaCtrl => MakerAPI.GetCharacterControl();
		public static BoobController MakerController =>
			MakerChaCtrl?.GetComponent<BoobController>();

		public void MakerAPI_RegisterCustomSubCategories(object sender, RegisterSubCategoriesEvent e)
		{
			float min = SliderMin.Value / 100f;
			float max = SliderMax.Value / 100f;


			// Boob

			foreach (string prefix in BoobController.prefixKeys)
			{
				foreach (string boolKey in BoobController.boolKeys)
				{
					string key = prefix + boolKey;
					MakerToggle toggle = toggles[key] = e.AddControl(new MakerToggle(
						categories[prefix],
						BoobController.makerAPILabels[key],
						BoobController.boolDefaults[key],
						this
					));
					toggle.ValueChanged.Subscribe(Observer.Create<bool>(v =>
						MakerAPI_ToggleValueChanged(key, v)
					));
				}

				foreach (string floatKey in BoobController.floatKeys)
				{
					string key = prefix + floatKey;
					float defaultValue = BoobController.floatDefaults[key];

					if (BoobController.floatMults.TryGetValue(floatKey, out float mult))
						defaultValue *= mult;

					MakerSlider slider = sliders[key] = e.AddControl(new MakerSlider(
						categories[prefix],
						BoobController.makerAPILabels[key],
						min,
						max,
						defaultValue,
						this
					));
					slider.ValueChanged.Subscribe(Observer.Create<float>(v =>
						MakerAPI_SliderValueChanged(prefix, floatKey, v)
					));
				}
			}
		}

		public void MakerAPI_MakerFinishedLoading(object sender, EventArgs e)
		{
			MakerAPI_Update(MakerController);
		}

		public static void MakerAPI_ToggleValueChanged(string key, bool value)
		{
			ChaControl chaCtrl = MakerChaCtrl;
			MakerController.boolData[key] = value;

			switch (key)
			{
				case BoobController.OVERRIDE_PHYSICS:
					sliders[BoobController.DAMPING].Visible.OnNext(value);
					sliders[BoobController.ELASTICITY].Visible.OnNext(value);
					sliders[BoobController.STIFFNESS].Visible.OnNext(value);
					sliders[BoobController.INERT].Visible.OnNext(value);

					chaCtrl.UpdateBustSoftness();
					chaCtrl.ChangeBustInert(false);
					break;

				case BoobController.OVERRIDE_GRAVITY:
					sliders[BoobController.GRAVITY_X].Visible.OnNext(value);
					sliders[BoobController.GRAVITY_Y].Visible.OnNext(value);
					sliders[BoobController.GRAVITY_Z].Visible.OnNext(value);

					chaCtrl.UpdateBustGravity();
					break;

				case BoobController.BUTT + BoobController.OVERRIDE_PHYSICS:
					sliders[BoobController.BUTT + BoobController.DAMPING].Visible.OnNext(value);
					sliders[BoobController.BUTT + BoobController.ELASTICITY].Visible.OnNext(value);
					sliders[BoobController.BUTT + BoobController.STIFFNESS].Visible.OnNext(value);
					sliders[BoobController.BUTT + BoobController.INERT].Visible.OnNext(value);

					if (value)
						chaCtrl.UpdateBustSoftness();
					else
					{
						Util.ResetButtPhysics(chaCtrl.GetDynamicBoneBustAndHip(
							ChaControlDefine.DynamicBoneKind.HipL
						));
						Util.ResetButtPhysics(chaCtrl.GetDynamicBoneBustAndHip(
							ChaControlDefine.DynamicBoneKind.HipR
						));
					}

					break;

				case BoobController.BUTT + BoobController.OVERRIDE_GRAVITY:
					sliders[BoobController.BUTT + BoobController.GRAVITY_X].Visible.OnNext(value);
					sliders[BoobController.BUTT + BoobController.GRAVITY_Y].Visible.OnNext(value);
					sliders[BoobController.BUTT + BoobController.GRAVITY_Z].Visible.OnNext(value);

					if (value)
						chaCtrl.UpdateBustGravity();
					else
					{
						Util.ResetButtGravity(chaCtrl.GetDynamicBoneBustAndHip(
							ChaControlDefine.DynamicBoneKind.HipL
						)); Util.ResetButtGravity(chaCtrl.GetDynamicBoneBustAndHip(
							 ChaControlDefine.DynamicBoneKind.HipR
						 ));
					}

					break;
			}
		}

		public static void MakerAPI_SliderValueChanged(string prefix, string floatKey, float value)
		{
			if (BoobController.floatMults.TryGetValue(floatKey, out float mult))
				value /= mult;

			string key = prefix + floatKey;
			MakerController.floatData[key] = value;

			switch (key)
			{
				case BoobController.DAMPING:
				case BoobController.ELASTICITY:
				case BoobController.STIFFNESS:
					MakerChaCtrl.UpdateBustSoftness();
					break;

				case BoobController.INERT:
					MakerChaCtrl.ChangeBustInert(false);
					break;

				case BoobController.GRAVITY_X:
				case BoobController.GRAVITY_Y:
				case BoobController.GRAVITY_Z:
					MakerChaCtrl.UpdateBustGravity();
					break;

				case BoobController.BUTT + BoobController.DAMPING:
				case BoobController.BUTT + BoobController.ELASTICITY:
				case BoobController.BUTT + BoobController.STIFFNESS:
				case BoobController.BUTT + BoobController.INERT:
					MakerChaCtrl.UpdateBustSoftness();
					break;

				case BoobController.BUTT + BoobController.GRAVITY_X:
				case BoobController.BUTT + BoobController.GRAVITY_Y:
				case BoobController.BUTT + BoobController.GRAVITY_Z:
					MakerChaCtrl.UpdateBustGravity();
					break;
			}
		}

		public static void MakerAPI_Update(BoobController controller)
		{
			if (controller == null)
				return;

			foreach (string prefix in BoobController.prefixKeys)
			{
				foreach (string boolKey in BoobController.boolKeys)
				{
					string key = prefix + boolKey;

					if (toggles.TryGetValue(key, out MakerToggle toggle))
						toggle.SetValue(controller.boolData[key]);
				}

				foreach (string floatKey in BoobController.floatKeys)
				{
					string key = prefix + floatKey;
					float mult = 1f;

					if (BoobController.floatMults.TryGetValue(floatKey, out float _mult))
						mult = _mult;

					if (sliders.TryGetValue(key, out MakerSlider slider))
						slider.SetValue(controller.floatData[key] * mult);
				}
			}
		}
	}
}
