﻿using System;
using System.Collections.Generic;
using System.Text;
using GameEngineTK.Engine;
using Microsoft.Xna.Framework;

namespace GameEngineTK.Scripts
{
	public class CameraScript : Script
	{
		public override void Start()
		{

		}

		public override void Update()
		{
			Vector2 pos = PlayerScript.Player.GetComponent<Transform>().Position;
			Camera.Position = Vector2.Lerp(Camera.Position,
				pos - (new Vector2(1920 / 2 - PlayerScript.Player.Width / 4, 1080 / 2 - PlayerScript.Player.Height / 4)),
				.005f * Time.deltaTime);
		}
	}
}
