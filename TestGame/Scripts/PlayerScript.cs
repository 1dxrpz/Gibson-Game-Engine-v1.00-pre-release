﻿using System;
using System.Collections.Generic;
using System.Text;
using GameEngineTK.Engine;
using GameEngineTK.Engine.Prototypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameEngineTK.Scripts
{
	public class PlayerScript : Script
	{
		public static GameObject Player;
		TextureHandler texture = new TextureHandler(@"C:\Users\HP\source\repos\TestGame\TestGame\Content\player.png");
		TextureHandler light = new TextureHandler(@"C:\Users\HP\source\repos\TestGame\TestGame\Content\light_right_input.png");
		Color[] data;
		Texture2D tex;
		public override void Start()
		{
			//Player = new GameObject(Content.Load<Texture2D>("player"), 32, 32);

			data = new Color[texture.Width * texture.Height];
			tex = new Texture2D(Script.graphicsDevice, texture.Width, texture.Height);
			Player = new GameObject(texture, 32, 32);
			
			

			Player.AddComponent(new BoxCollider());
		}

		public override void Update()
		{
			Player.Width = 64;
			Player.Height = 64;

			Services.GetService<Debug>().text = Player.VTexture.GetPixel(0, 0).ToString();

			

			//Player.RotateTowardObject(CursorScript.Cursor);

			Transform pt = Player.GetComponent<Transform>();

			if (Keyboard.GetState().IsKeyDown(Keys.D))
			{
				Player.Flip = SpriteEffects.None;
				pt.Velocity.X = .25f * Time.deltaTime;
			}
			else
			if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				Player.Flip = SpriteEffects.FlipHorizontally;
				pt.Velocity.X = -.25f * Time.deltaTime;
			}
			else
				pt.Velocity.X = 0;

			if (Keyboard.GetState().IsKeyDown(Keys.S))
				pt.Velocity.Y = .25f * Time.deltaTime;
			else
			if (Keyboard.GetState().IsKeyDown(Keys.W))
				pt.Velocity.Y = -.25f * Time.deltaTime;
			else
				pt.Velocity.Y = 0;

			Player.Draw();
		}
	}
}