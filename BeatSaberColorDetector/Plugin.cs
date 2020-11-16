using System.Linq;
using IPA;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace BeatSaberColorDetector
{
	[Plugin(RuntimeOptions.SingleStartInit)]
	public class Plugin
	{
		internal static Plugin Instance { get; private set; }
		internal static IPALogger Log { get; private set; }

		private Tcp tcp;
		private ColorManager cm;

		[Init]
		public void Init(IPALogger logger)
		{
			Instance = this;
			Log = logger;

			Log.Info("BeatSaberColorDetector initialized.");
		}

		[OnStart]
		public void OnApplicationStart()
		{
			BS_Utils.Utilities.BSEvents.noteWasCut += NoteWasCut;
			BS_Utils.Utilities.BSEvents.lateMenuSceneLoadedFresh += MenuScreenLoaded;
			BS_Utils.Utilities.BSEvents.gameSceneActive += GameSceneActive;
		}

		[OnExit]
		public void OnApplicationQuit()
		{
			tcp.Disconnect();
		}

		private void MenuScreenLoaded(ScenesTransitionSetupDataSO data)
		{
			tcp = new Tcp("192.168.1.222", 32750);
			tcp.Init();
		}

		private void GameSceneActive()
		{
			cm = Resources.FindObjectsOfTypeAll<ColorManager>().LastOrDefault();
		}

		private void NoteWasCut(NoteData data, NoteCutInfo info, int multiplier)
		{
			Color32 color = cm.ColorForType(data.colorType);
			tcp.SendData(new RgbObject()
			{
				type = "PULSE",
				r = color.r,
				g = color.g,
				b = color.b
			});
		}
	}
}
