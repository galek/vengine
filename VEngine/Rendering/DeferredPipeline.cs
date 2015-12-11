﻿using System;
using OpenTK.Graphics.OpenGL4;

namespace VEngine
{
    public class DeferredPipeline
    {
        public PostProcessing PostProcessor;

        public DeferredPipeline()
        {
            PostProcessor = new PostProcessing(Game.Resolution.Width, Game.Resolution.Height);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.DepthClamp);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);
            GL.ClearColor(0, 0, 0, 0);
            GL.ClearDepth(1);

            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);

            GL.Disable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }
    }
}