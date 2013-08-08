﻿using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.GUI
{
    public class Label : Control
    {
        public Color Color = Color.Black;

        public string Text
        {
            get { return TextRender.Text; }
            set
            {
                TextRender.Text = value;

                Body.Bounds = TextRender.Font.MeasureString(TextRender.Text);
            }
        }

        //Components
        public TextRender TextRender;

        public Label(ControlHandler parent, string name)
            : base(parent, name)
        {
            TextRender = new TextRender(this, "TextRender", Assets.Font, name, Body);
            TextRender.Text = name;
            TextRender.Layer = 1f;
            Body.Bounds = TextRender.Font.MeasureString(Text);
            TextRender.Color = Color.Black;
            Selectable = false;
        }
    }
}