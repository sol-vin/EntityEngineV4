﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Data
{
    public class Camera
    {
        public Vector2 LastPosition { get; private set; }
        public Vector2 Delta { get { return Position - LastPosition; } }
        public Vector2 Position = new Vector2();
        public float Zoom = 1f;
        public Matrix Transform
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(EntityGame.Viewport.Width * 0.5f, EntityGame.Viewport.Height * 0.5f, 0)); ;
            }
        }

        public bool IsActive
        {
            get { return EntityGame.CurrentCamera.Equals(this); }
        }

        public Rectangle ScreenSpace
        {
            get
            {
                var r = new Rectangle
                    {
                        X = (int)(Position.X - EntityGame.Viewport.Width/2f),
                        Y = (int)(Position.Y - EntityGame.Viewport.Height/2f),
                        Width = (int)(EntityGame.Viewport.Width*Zoom),
                        Height = (int)(EntityGame.Viewport.Height*Zoom)
                    };
                return r;
            }
        }

        public Camera()
        {
            Position = new Vector2(EntityGame.Viewport.Width/2f, EntityGame.Viewport.Height/2f);
        }

        public void Update()
        {
            LastPosition = Position;
        }

        public void View()
        {
            EntityGame.CurrentCamera = this;
        }
    }
}