using AIChara;
using KKAPI.Maker;
using KKAPI.Maker.UI;
using System;
using UniRx;

namespace HS2_BoobSettings
{
	public partial class HS2_BoobSettings
	{
		public static MakerToggle overridePhysics;
		public static MakerSlider damping;
		public static MakerSlider elasticity;
		public static MakerSlider stiffness;
		public static MakerSlider inert;

		public static MakerToggle overrideGravity;
		public static MakerSlider gravityX;
		public static MakerSlider gravityY;
		public static MakerSlider gravityZ;

		public static ChaControl MakerChaCtrl => MakerAPI.GetCharacterControl();
		public static BoobController MakerController =>
			MakerChaCtrl?.GetComponent<BoobController>();

		public void MakerAPI_RegisterCustomSubCategories(object sender, RegisterSubCategoriesEvent e)
		{
			float min = SliderMin.Value / 100f;
			float max = SliderMax.Value / 100f;
			MakerCategory category = MakerConstants.Body.Breast;

			overridePhysics = e.AddControl(new MakerToggle(category, "Override Breast Physics", this));
			overridePhysics.ValueChanged.Subscribe(Observer.Create<bool>(MakerAPI_TogglePhysics));

			damping = e.AddControl(new MakerSlider(category, "Breast Damping", min, max, 0.14f, this));
			damping.ValueChanged.Subscribe(Observer.Create<float>(v =>
			{
				MakerController.damping = v;
				MakerChaCtrl.UpdateBustSoftness();
			}));

			elasticity = e.AddControl(new MakerSlider(category, "Breast Elasticity", min, max, 0.17f, this));
			elasticity.ValueChanged.Subscribe(Observer.Create<float>(v =>
			{
				MakerController.elasticity = v;
				MakerChaCtrl.UpdateBustSoftness();
			}));

			stiffness = e.AddControl(new MakerSlider(category, "Breast Stiffness", min, max, 0.5f, this));
			stiffness.ValueChanged.Subscribe(Observer.Create<float>(v =>
			{
				MakerController.stiffness = v;
				MakerChaCtrl.UpdateBustSoftness();
			}));

			inert = e.AddControl(new MakerSlider(category, "Breast Inert", min, max, 0.8f, this));
			inert.ValueChanged.Subscribe(Observer.Create<float>(v =>
			{
				MakerController.inert = v;
				MakerChaCtrl.ChangeBustInert(false);
			}));

			overrideGravity = e.AddControl(new MakerToggle(category, "Override Breast Gravity", this));
			overrideGravity.ValueChanged.Subscribe(Observer.Create<bool>(MakerAPI_ToggleGravity));

			gravityX = e.AddControl(new MakerSlider(category, "Breast Gravity X", min, max, 0f, this));
			gravityX.ValueChanged.Subscribe(Observer.Create<float>(v =>
			{
				MakerController.gravityX = v / 100f;
				MakerChaCtrl.UpdateBustGravity();
			}));

			gravityY = e.AddControl(new MakerSlider(category, "Breast Gravity Y", min, max, -0.03f, this));
			gravityY.ValueChanged.Subscribe(Observer.Create<float>(v =>
			{
				MakerController.gravityY = v / 100f;
				MakerChaCtrl.UpdateBustGravity();
			}));

			gravityZ = e.AddControl(new MakerSlider(category, "Breast Gravity Z", min, max, 0f, this));
			gravityZ.ValueChanged.Subscribe(Observer.Create<float>(v =>
			{
				MakerController.gravityZ = v / 100f;
				MakerChaCtrl.UpdateBustGravity();
			}));
		}

		public void MakerAPI_MakerFinishedLoading(object sender, EventArgs e)
		{
			MakerAPI_Update(MakerAPI.GetCharacterControl()?.GetComponent<BoobController>());
		}

		public static void MakerAPI_TogglePhysics(bool flag)
		{
			MakerController.overridePhysics = flag;

			damping.Visible.OnNext(flag);
			elasticity.Visible.OnNext(flag);
			stiffness.Visible.OnNext(flag);
			inert.Visible.OnNext(flag);

			ChaControl chaCtrl = MakerChaCtrl;

			chaCtrl.UpdateBustSoftness();
			chaCtrl.ChangeBustInert(false);
		}

		public static void MakerAPI_ToggleGravity(bool flag)
		{
			MakerController.overrideGravity = flag;

			gravityX.Visible.OnNext(flag);
			gravityY.Visible.OnNext(flag);
			gravityZ.Visible.OnNext(flag);

			MakerChaCtrl.UpdateBustGravity();
		}

		public static void MakerAPI_Update(BoobController controller)
		{
			if (controller == null)
				return;

			overridePhysics?.SetValue(controller.overridePhysics);
			damping?.SetValue(controller.damping);
			elasticity?.SetValue(controller.elasticity);
			stiffness?.SetValue(controller.stiffness);
			inert?.SetValue(controller.inert);

			overrideGravity?.SetValue(controller.overrideGravity);
			gravityX?.SetValue(controller.gravityX * 100f);
			gravityY?.SetValue(controller.gravityY * 100f);
			gravityZ?.SetValue(controller.gravityZ * 100f);
		}
	}
}
