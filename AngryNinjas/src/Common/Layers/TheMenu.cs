using System;
using Microsoft.Xna.Framework;
using CocosSharp;

namespace AngryNinjas
{
	public class TheMenu : CCLayer
	{
		CCMenu VoiceFXMenu;
		CCMenu SoundFXMenu;
		CCMenu AmbientFXMenu;

		CCPoint VoiceFXMenuLocation;
		CCPoint SoundFXMenuLocation;
		CCPoint AmbientFXMenuLocation;

		string menuBackgroundName;

		string lvlButtonName1;
		string lvlLockedButtonName1;

		string lvlButtonName2;
		string lvlLockedButtonName2;

		string lvlButtonName3;
		string lvlLockedButtonName3;

		string lvlButtonName4;
		string lvlLockedButtonName4;

		string lvlButtonName5;
		string lvlLockedButtonName5;

		string lvlButtonName6;
		string lvlLockedButtonName6;

		string lvlButtonName7;
		string lvlLockedButtonName7;

		string lvlButtonName8;
		string lvlLockedButtonName8;

		string lvlButtonName9;
		string lvlLockedButtonName9;

		string lvlButtonName10;
		string lvlLockedButtonName10;

		string voiceButtonName;
		string voiceButtonNameDim;

		string soundButtonName;
		string soundButtonNameDim;

		string ambientButtonName;
		string ambientButtonNameDim;

		CCSize screenSize;

		public TheMenu(CCSize size)
			: base(size)
		{

		}

		protected override void AddedToScene()
		{
			base.AddedToScene();

			screenSize = Window.WindowSizeInPixels;

			CCPoint menu1Position;
			CCPoint menu2Position;

			menuBackgroundName = "menu_background";
			//will use "menu_background.png" for non-Retina Phones 
			//will use   "menu_background-hd.png"; for retina phones
			//will use "menu_background-ipad.png";

			//same goes for images below..

			lvlButtonName1 = "levelButton1";
			lvlLockedButtonName1 = "levelButton1_locked";

			lvlButtonName2 = "levelButton2";
			lvlLockedButtonName2 = "levelButton2_locked";

			lvlButtonName3 = "levelButton3";
			lvlLockedButtonName3 = "levelButton3_locked";

			lvlButtonName4 = "levelButton4";
			lvlLockedButtonName4 = "levelButton4_locked";

			lvlButtonName5 = "levelButton5";
			lvlLockedButtonName5 = "levelButton5_locked";

			lvlButtonName6 = "levelButton6";
			lvlLockedButtonName6 = "levelButton6_locked";

			lvlButtonName7 = "levelButton7";
			lvlLockedButtonName7 = "levelButton7_locked";

			lvlButtonName8 = "levelButton8";
			lvlLockedButtonName8 = "levelButton8_locked";

			lvlButtonName9 = "levelButton9";
			lvlLockedButtonName9 = "levelButton9_locked";

			lvlButtonName10 = "levelButton10";
			lvlLockedButtonName10 = "levelButton10_locked";

			voiceButtonName = "voiceFX";
			voiceButtonNameDim = "voiceFX_dim";

			soundButtonName = "soundFX";
			soundButtonNameDim = "soundFX_dim";

			ambientButtonName = "ambientFX";
			ambientButtonNameDim = "ambientFX_dim";

			if (TheLevel.SharedLevel.IS_IPAD)
			{ 
                //iPADs..
				menu1Position = new CCPoint(screenSize.Width / 2, 430);
				menu2Position = new CCPoint(screenSize.Width / 2, 290);

				SoundFXMenuLocation = new CCPoint(240, 170);
				VoiceFXMenuLocation = new CCPoint(480, 170);
				AmbientFXMenuLocation = new CCPoint(750, 170);
				//if( ! CCDirector.SharedDirector.enableRetinaDisplay ) {
				CCLog.Log("must be iPad 1 or 2");
				//change nothing 
				//} else {
				CCLog.Log("retina display is on-must be iPAd 3");
				//change files names for iPad 3 

				menuBackgroundName = "menu_background-ipad";  //will use @"menu_background-ipad-hd"; 

				lvlButtonName1 = "levelButton1-ipad";
				lvlLockedButtonName1 = "levelButton1_locked-ipad";

				lvlButtonName2 = "levelButton2-ipad";
				lvlLockedButtonName2 = "levelButton2_locked-ipad";

				lvlButtonName3 = "levelButton3-ipad";
				lvlLockedButtonName3 = "levelButton3_locked-ipad";

				lvlButtonName4 = "levelButton4-ipad";
				lvlLockedButtonName4 = "levelButton4_locked-ipad";

				lvlButtonName5 = "levelButton5-ipad";
				lvlLockedButtonName5 = "levelButton5_locked-ipad";

				lvlButtonName6 = "levelButton6-ipad";
				lvlLockedButtonName6 = "levelButton6_locked-ipad";

				lvlButtonName7 = "levelButton7-ipad";
				lvlLockedButtonName7 = "levelButton7_locked-ipad";

				lvlButtonName8 = "levelButton8-ipad";
				lvlLockedButtonName8 = "levelButton8_locked-ipad";

				lvlButtonName9 = "levelButton9-ipad";
				lvlLockedButtonName9 = "levelButton9_locked-ipad";

				lvlButtonName10 = "levelButton10-ipad";
				lvlLockedButtonName10 = "levelButton10_locked-ipad";

				voiceButtonName = "voiceFX-ipad";
				voiceButtonNameDim = "voiceFX_dim-ipad";

				soundButtonName = "soundFX-ipad";
				soundButtonNameDim = "soundFX_dim-ipad";

				ambientButtonName = "ambientFX-ipad";
				ambientButtonNameDim = "ambientFX_dim-ipad";
			}
			else
			{  
                //IPHONES..
				menu1Position = new CCPoint(screenSize.Width / 2, 185);
				menu2Position = new CCPoint(screenSize.Width / 2, 115);

				SoundFXMenuLocation = new CCPoint(110, 55);
				VoiceFXMenuLocation = new CCPoint(230, 55);
				AmbientFXMenuLocation = new CCPoint(355, 55);
			}

			var theBackground = new CCSprite(menuBackgroundName);
			theBackground.Position = new CCPoint(screenSize.Width / 2, screenSize.Height / 2);
			AddChild(theBackground, 0);

			//TouchEnabled = true;

			CCMenuItem button1;
			CCMenuItem button2;
			CCMenuItem button3;
			CCMenuItem button4;
			CCMenuItem button5;
			CCMenuItem button6;
			CCMenuItem button7;
			CCMenuItem button8;
			CCMenuItem button9;
			CCMenuItem button10;

			button1 = new CCMenuItemImage(lvlButtonName1, lvlButtonName1, GoToFirstLevelSection1);

			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(2) == false)
				button2 = new CCMenuItemImage(lvlLockedButtonName2, lvlLockedButtonName2, PlayNegativeSound);
			else
				button2 = new CCMenuItemImage(lvlButtonName2, lvlButtonName2, GoToFirstLevelSection2);

			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(3) == false)
				button3 = new CCMenuItemImage(lvlLockedButtonName3, lvlLockedButtonName3, PlayNegativeSound);
			else
				button3 = new CCMenuItemImage(lvlButtonName3, lvlButtonName3, GoToFirstLevelSection3);

			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(4) == false)
				button4 = new CCMenuItemImage(lvlLockedButtonName4, lvlLockedButtonName4, PlayNegativeSound);
			else
				button4 = new CCMenuItemImage(lvlButtonName4, lvlButtonName4, GoToFirstLevelSection4);

			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(5) == false)
				button5 = new CCMenuItemImage(lvlLockedButtonName5, lvlLockedButtonName5, PlayNegativeSound);
			else
				button5 = new CCMenuItemImage(lvlButtonName5, lvlButtonName5, GoToFirstLevelSection5);

			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(6) == false)
				button6 = new CCMenuItemImage(lvlLockedButtonName6, lvlLockedButtonName6, PlayNegativeSound);
			else
				button6 = new CCMenuItemImage(lvlButtonName6, lvlButtonName6, GoToFirstLevelSection6);

			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(7) == false)
				button7 = new CCMenuItemImage(lvlLockedButtonName7, lvlLockedButtonName7, PlayNegativeSound);
			else
				button7 = new CCMenuItemImage(lvlButtonName7, lvlButtonName7, GoToFirstLevelSection7);

			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(8) == false)
				button8 = new CCMenuItemImage(lvlLockedButtonName8, lvlLockedButtonName8, PlayNegativeSound);
			else
				button8 = new CCMenuItemImage(lvlButtonName8, lvlButtonName8, GoToFirstLevelSection8);

			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(9) == false)
				button9 = new CCMenuItemImage(lvlLockedButtonName9, lvlLockedButtonName9, PlayNegativeSound);
			else
				button9 = new CCMenuItemImage(lvlButtonName9, lvlButtonName9, GoToFirstLevelSection9);
		
            if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(10) == false)
				button10 = new CCMenuItemImage(lvlLockedButtonName10, lvlLockedButtonName10, PlayNegativeSound);
			else
				button10 = new CCMenuItemImage(lvlButtonName10, lvlButtonName10, GoToFirstLevelSection10);

			CCMenu Menu = new CCMenu(button1, button2, button3, button4, button5);
			Menu.Position = menu1Position;

			Menu.AlignItemsHorizontally(10);
			AddChild(Menu, 1);

			CCMenu Menu2 = new CCMenu(button6, button7, button8, button9, button10);
			Menu2.Position = menu2Position;
			Menu2.AlignItemsHorizontally(10);
			AddChild(Menu2, 1);

			IsSoundFXMenuItemActive = !GameData.SharedData.AreSoundFXMuted;
			IsVoiceFXMenuActive = !GameData.SharedData.AreVoiceFXMuted;
			IsAmbientFXMenuActive = !GameData.SharedData.AreAmbientFXMuted;
		}

		void PlayNegativeSound(object sender)
		{
			//play a sound indicating this level isn't available
			GameSounds.SharedGameSounds.PlaySoundFX("bloop");
		}

		public static CCScene GetScene(CCWindow mainWindow)
		{
			CCScene scene = new CCScene(mainWindow);
			CCLayer layer = new TheMenu(mainWindow.WindowSizeInPixels);

			// add layer as a child to scene
			scene.AddChild(layer);

			// return the scene
			return scene;
		}

		#region SECTION BUTTONS

		void GoToFirstLevelSection1(object sender)
		{
			GameData.SharedData.ChangeLevelToFirstInThisSection(1);
			PopAndTransition();
		}

		void GoToFirstLevelSection2(object sender)
		{
			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(2))
			{
				GameData.SharedData.ChangeLevelToFirstInThisSection(2);
				PopAndTransition();
			}
		}

		void GoToFirstLevelSection3(object sender)
		{
			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(3))
			{
				GameData.SharedData.ChangeLevelToFirstInThisSection(3);
				PopAndTransition();
			}
		}

		void GoToFirstLevelSection4(object sender)
		{
			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(4))
			{
				GameData.SharedData.ChangeLevelToFirstInThisSection(4);
				PopAndTransition();
			}
		}

		void GoToFirstLevelSection5(object sender)
		{
			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(5))
			{
				GameData.SharedData.ChangeLevelToFirstInThisSection(5);
				PopAndTransition();

			}
		}
		void GoToFirstLevelSection6(object sender)
		{
			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(6))
			{
				GameData.SharedData.ChangeLevelToFirstInThisSection(6);
				PopAndTransition();
			}
		}

		void GoToFirstLevelSection7(object sender)
		{
			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(7))
			{
				GameData.SharedData.ChangeLevelToFirstInThisSection(7);
				PopAndTransition();

			}
		}

		void GoToFirstLevelSection8(object sender)
		{
			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(8))
			{
				GameData.SharedData.ChangeLevelToFirstInThisSection(8);
				PopAndTransition();
			}
		}

		void GoToFirstLevelSection9(object sender)
		{
			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(9))
			{
				GameData.SharedData.ChangeLevelToFirstInThisSection(9);
				PopAndTransition();
			}
		}

		void GoToFirstLevelSection10(object sender)
		{
			if (GameData.SharedData.CanYouGoToTheFirstLevelOfThisSection(10))
			{
				GameData.SharedData.ChangeLevelToFirstInThisSection(10);
				PopAndTransition();
			}
		}

		#endregion

		#region POP (remove) SCENE and Transition to new level

		void PopAndTransition()
		{
			Director.PopScene();
			//when TheLevel scene reloads it will start with a new level
			TheLevel.SharedLevel.TransitionAfterMenuPop();
		}

		#endregion

		#region  POP (remove) SCENE and continue playing current level

		public void TouchesBegan(System.Collections.Generic.List<CCTouch> touches)
		{
			Director.PopScene();
		}

		#endregion

		#region VOICE FX
		
        bool IsVoiceFXMenuActive
		{
			set
			{
				RemoveChild(VoiceFXMenu, true);
				CCMenuItem button1;

				if (!value)
					button1 = new CCMenuItemImage(voiceButtonNameDim, voiceButtonNameDim, TurnVoiceFXOn);
				else
					button1 = new CCMenuItemImage(voiceButtonName, voiceButtonName, TurnVoiceFXOff);

				VoiceFXMenu = new CCMenu(button1);
				VoiceFXMenu.Position = VoiceFXMenuLocation;

				AddChild(VoiceFXMenu, 10);
			}
		}

		void TurnVoiceFXOn(object sender)
		{
			GameData.SharedData.AreVoiceFXMuted = false;
			GameSounds.SharedGameSounds.AreVoiceFXMuted = false;
			IsVoiceFXMenuActive = true;
		}
		void TurnVoiceFXOff(object sender)
		{
			GameData.SharedData.AreVoiceFXMuted = true;
			GameSounds.SharedGameSounds.AreVoiceFXMuted = true;
			IsVoiceFXMenuActive = false;
		}

		#endregion

		#region Sound FX

		bool IsSoundFXMenuItemActive
		{
			set
			{
				RemoveChild(SoundFXMenu, true);
				CCMenuItem button1;

				if (!value)
					button1 = new CCMenuItemImage(soundButtonNameDim, soundButtonNameDim, TurnSoundFXOn);
				else
					button1 = new CCMenuItemImage(soundButtonName, soundButtonName, TurnSoundFXOff);

				SoundFXMenu = new CCMenu(button1);
				SoundFXMenu.Position = SoundFXMenuLocation;
				AddChild(SoundFXMenu, 10);
			}
		}

		void TurnSoundFXOn(object sender)
		{
			GameData.SharedData.AreSoundFXMuted = false;
			GameSounds.SharedGameSounds.AreSoundFXMuted = false;
			IsSoundFXMenuItemActive = true;
		}

		void TurnSoundFXOff(object sender)
		{
			GameData.SharedData.AreSoundFXMuted = true;
			GameSounds.SharedGameSounds.AreSoundFXMuted = true;
			IsSoundFXMenuItemActive = false;
		}

		#endregion

		#region Ambient FX
		
        bool IsAmbientFXMenuActive
		{
			set
			{
				RemoveChild(AmbientFXMenu, true);
				CCMenuItem button1;

				if (!value)
					button1 = new CCMenuItemImage(ambientButtonNameDim, ambientButtonNameDim, TurnAmbientFXOn);
				else
					button1 = new CCMenuItemImage(ambientButtonName, ambientButtonName, TurnAmbientFXOff);

				AmbientFXMenu = new CCMenu(button1);
				AmbientFXMenu.Position = AmbientFXMenuLocation;
				AddChild(AmbientFXMenu, 10);
			}

		}

		void TurnAmbientFXOn(object sender)
		{
			GameData.SharedData.AreAmbientFXMuted = true;
			GameSounds.SharedGameSounds.RestartBackgroundMusic();
			IsAmbientFXMenuActive = true;
		}

		void TurnAmbientFXOff(object sender)
		{
			GameData.SharedData.AreAmbientFXMuted = false;
			GameSounds.SharedGameSounds.StopBackgroundMusic();
			IsAmbientFXMenuActive = false;
		}

		#endregion
	}
}

