using UnityEngine;

	public static class GameSetings
	{
		public static float AnimationMovementDuration
		{
			get;
			private set;
		}
		public static bool Animate { get; private set; }

		public static void LoadSetings()
		{
			//todo: move these values to consts somewhere.
			AnimationMovementDuration = PlayerPrefs.GetFloat("AnimDuration", 0.15f);
			Animate = PlayerPrefs.GetInt("Animate", 1) == 1 ? true : false;
		}

		public static void SetAnimationMovementDuration(float animationSpeed)
		{
			AnimationMovementDuration = animationSpeed;
			PlayerPrefs.SetFloat("AnimDuration", animationSpeed);
		}

		public static void SetAnimate(bool animate)
		{
			Animate = animate;
			PlayerPrefs.SetInt("Animate", Animate ? 1 : 0);
		}

		public static void ResetToDefaul()
		{
			AnimationMovementDuration = 0.15f;
			Animate = true;
		}
	}
