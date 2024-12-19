using UnityEngine;

	public static class GameSetings
	{
		//Defaults
		private const float DefaultAnimMoveDuration = 0.15f;
		private const bool DefaultAnimate = true;
		private const string DefaultChessChannel = "bullet";
		private const int DefaultViewScene = 0;
		public static float AnimationMovementDuration
		{
			get;
			private set;
		}
		public static bool Animate { get; private set; }
		public static string ChessChannel;
		public static int ViewScene;

		public static void LoadSetings()
		{
			//todo: move these values to consts somewhere.
			AnimationMovementDuration = PlayerPrefs.GetFloat("AnimDuration", DefaultAnimMoveDuration);
			Animate = PlayerPrefs.GetInt("Animate", DefaultAnimate ? 1 : 0) == 1;
			ChessChannel = PlayerPrefs.GetString("ChessChannel", DefaultChessChannel);
			ViewScene = PlayerPrefs.GetInt("ViewScene", DefaultViewScene); 
		}

		public static void SetViewScene(int viewScene)
		{
			ViewScene = viewScene;
			PlayerPrefs.SetInt("ViewScene", ViewScene); 

		}
		public static void SetChannel(string channel)
		{
			ChessChannel = channel;
			PlayerPrefs.SetString("ChessChannel", ChessChannel);
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

		public static void ResetToDefault()
		{
			AnimationMovementDuration = DefaultAnimMoveDuration;
			Animate = DefaultAnimate;
		}
	}
