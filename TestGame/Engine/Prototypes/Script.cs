﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngineTK.Engine
{
	abstract public class Script
	{
		public static GameServiceContainer Services;
		public static ContentManager Content;
		public static SpriteBatch ctx;
		public static GraphicsDevice graphicsDevice;
		abstract public void Update();
		abstract public void Start();
	}
}
