﻿using OpenTK.Graphics;
using System.Drawing;

namespace VDGTech
{
    public class SolidColorMaterial : IMaterial
    {
        protected static ShaderProgram Program;
        private Color4 Colour;

        public SolidColorMaterial(Color color)
        {
            Program = new ShaderProgram(Media.ReadAllText("SolidColorMaterial.vertex.glsl"), Media.ReadAllText("SolidColorMaterial.fragment.glsl"));
            Colour = color;
        }

        public ShaderProgram GetShaderProgram()
        {
            return Program;
        }

        public void Use()
        {
            Program.Use();
            Program.SetUniform("input_Color", Colour);
        }
    }
}