﻿using GameEngineTK.Engine.Prototypes;
using GameEngineTK.Engine.Prototypes.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngineTK.Engine
{
	public class Params
	{
		public bool isVisible = true;
		public bool isDraggable = false;
		public bool MouseDown = false;
		public bool onHover = false;
	}
	public class GameObject
	{
		private int width;
		private int height;
		private Texture2D texture;
		private TextureHandler vtexture;

		public Params objectParams = new Params();

		/// <summary>
		/// Creates new GameObject instance
		/// </summary>
		public GameObject(Texture2D texture, int width, int height)
		{
			Components.Add(new Transform());
			Components.Add(new Physics());
			Components.Add(new Animation(texture, width, height));


			this.texture = texture;
			this.width = width;
			this.height = height;
		}
		public GameObject(TextureHandler vtexture, int width, int height)
		{
			Components.Add(new Transform());
			Components.Add(new Physics());
			Components.Add(new Animation(vtexture.ToTexture2D(), width, height));

			this.vtexture = vtexture;
			this.width = width;
			this.height = height;
		}
		/// <summary>
		/// Texture origin position
		/// </summary>
		public int Width
		{
			get { return width; }
			set {
				this.GetComponent<Animation>().size.X = value;
				width = value;
			}
		}
		public int Height
		{
			get { return height; }
			set {
				this.GetComponent<Animation>().size.Y = value;
				height = value;
			}
		}

		// REWRITE TO TRANSFORM FIELD
		public void RotateTowardPosition(Vector2 pos)
		{
			this.GetComponent<Transform>().Rotation = (float)Math.Atan2(pos.Y - this.GetComponent<Transform>().ScreenPosition().Y, pos.X - this.GetComponent<Transform>().ScreenPosition().X);
		}
		public void RotateTowardObject(GameObject obj)
		{
			this.RotateTowardPosition(obj.GetComponent<Transform>().Position);
		}
		public void RotateClockwise(float angle)
		{
			this.GetComponent<Transform>().Rotation += angle;
		}
		public void RotateCounterClockwise(float angle)
		{
			this.GetComponent<Transform>().Rotation -= angle;
		}
		// REWRITE TO TRANSFORM FIELD

		public SpriteEffects Flip = default;

		public Texture2D Texture {
			get { return texture; }
			set {
				this.GetComponent<Animation>().SpriteSheet = value;
				texture = value;
			}
		}

		public TextureHandler VTexture {
			get { return vtexture; }
			set
			{
				this.GetComponent<Animation>().SpriteSheet = value.ToTexture2D();
				vtexture = value;
			}
		}

		private readonly List<IComponentManager> Components = new List<IComponentManager>();

		public bool AddComponent(IComponentManager c)
		{
			if (this.HasComponent(c))
				return false;
			Components.Add(c);
			return true;
		}
		public bool RemoveComponent(IComponentManager c)
		{
			if (!this.HasComponent(c))
				return false;
			Components.Remove(c);
			return true;
		}

		public T GetComponent<T>()
		{
			for (int i = 0; i < Components.Count; i++)
				if (Components[i] is T) return (T)Convert.ChangeType(Components[i], typeof(T));
			throw new ArgumentException($"{this} does not contain {nameof(T)} component");
		}
		[Obsolete("This method deprecated; missing components throw an exception")]
		public bool HasComponent<T>()
		{
			for (int i = 0; i < Components.Count; i++)
				if (Components[i] is T) return true;
			return false;
		}
		[Obsolete("This method deprecated; missing components throw an exception")]
		public bool HasComponent(IComponentManager obj)
		{
			for (int i = 0; i < Components.Count; i++)
				if (Components[i].Equals(obj)) return true;
			return false;
		}

		// MAKE FUNCTIONS COMPONENT
		public bool OnObjectClicked()
		{
			bool MouseDown = objectParams.MouseDown;
			if (Mouse.GetState().LeftButton == ButtonState.Pressed && MouseDown)
				return false;
			else
			{
				if (!MouseDown && this.IsHover() && Mouse.GetState().LeftButton == ButtonState.Pressed)
					MouseDown = true;
			}
			return MouseDown;
		} // crap
		public Vector2 OriginPosition = new Vector2();
		
		// REWRITE TO TRANSFORM FIELD
		public bool OnObjectDragging()
		{
			if (this.IsHover() && Mouse.GetState().LeftButton == ButtonState.Released)
				objectParams.onHover = true;
			else if (Mouse.GetState().LeftButton == ButtonState.Released)
				objectParams.onHover = false;
			return objectParams.onHover && Mouse.GetState().LeftButton == ButtonState.Pressed && objectParams.isVisible;
		}
		public bool IsHover()
		{
			Vector2 pos = this.GetComponent<Transform>().Position;
			return Mouse.GetState().X > pos.X &&
				Mouse.GetState().X < pos.X + width &&
				Mouse.GetState().Y > pos.Y &&
				Mouse.GetState().Y < pos.Y + height && objectParams.isVisible;
		}

		// MAKE FUNCTIONS COMPONENT

		// TEST THIS OPTION FOR OPTIMISATION ISSUES
		public float Distance(GameObject obj)
		{
			Vector2 opos = obj.GetComponent<Transform>().Position - obj.OriginPosition;
			Vector2 pos = this.GetComponent<Transform>().Position - OriginPosition;
			return (float) Math.Sqrt((pos.X - opos.X) * (pos.X - opos.X) + (pos.Y - opos.Y) * (pos.Y - opos.Y));
		}
		public float VDistance(GameObject obj)
		{
			Vector2 opos = obj.GetComponent<Transform>().Position;
			Vector2 pos = this.GetComponent<Transform>().Position;
			return Vector2.Distance(opos, pos);
		}
		// TEST THIS OPTION FOR OPTIMISATION ISSUES

		public void Draw()
		{

			Transform _t = this.GetComponent<Transform>();
			if (_t.ScreenPosition().X > -Width && _t.ScreenPosition().X < 1920 &&
				_t.ScreenPosition().Y > -Height && _t.ScreenPosition().Y < 1080)
			{
				if (Components.Count > 0)
					Components.ForEach(v =>
					{
						v.Update();
						v.Width = width;
						v.Height = height;
						v.Position = this.GetComponent<Transform>().Position;
					});
				if (this.HasComponent<Animation>())
				{
					Animation an = this.GetComponent<Animation>();

					ScriptManager.ctx.Draw(an.SpriteSheet, new Rectangle(_t.ScreenPosition().ToPoint(), an.size),
						new Rectangle(an.src, an.FrameSize), Color.White, this.GetComponent<Transform>().Rotation, OriginPosition, Flip, 0);
				}
				else if (objectParams.isVisible)
				{
					ScriptManager.ctx.Draw(texture, new Rectangle(_t.ScreenPosition().ToPoint(), new Point(Width, Height)),
						new Rectangle(0, 0, texture.Width, texture.Height), Color.White, this.GetComponent<Transform>().Rotation, OriginPosition, Flip, 0);
				}
				if (this.HasComponent<BoxCollider>())
				{
					BoxCollider bc = this.GetComponent<BoxCollider>();
					bc.velocity = this.GetComponent<Transform>().Velocity;
					bc.Position -= OriginPosition * 2;
					if (BoxCollider.RenderColisionMask)
						ScriptManager.ctx.Draw(BoxCollider.ColliderRenderTexture, new Rectangle(World.ScreenPosition(bc.Position).ToPoint(), new Point(bc.Width, bc.Height)), Color.White);
				}
			}
		}

	}
}