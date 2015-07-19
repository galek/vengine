﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace VEngine.PathTracing
{
    public class Vertex
    {
        public Vector3 Position, Normal, Albedo;
        public void Tranform(Matrix4 matrix)
        {
            Position = Vector4.Transform(new Vector4(Position, 1.0f), matrix).Xyz;
            Normal = Vector3.Transform(Normal, matrix.ExtractRotation(true));
        }
    }

    public class Triangle
    {
        public Triangle()
        {
            Vertices = new List<Vertex>();
        }
        public List<Vertex> Vertices;
    }

    public class PointLight
    {
        public Vector3 Position;
        public Vector3 Color;
        public float Radius;
        public int Samples;
        public PointLight(Vector3 position, Vector3 color, float radius, int samples)
        {
            Position = position;
            Color = color;
            Radius = radius;
            Samples = samples;
        }
    }

    public class PathTracer
    {
        ShaderStorageBuffer MeshDataSSBO;
        ShaderStorageBuffer RandomsSSBO;
        ShaderStorageBuffer TrianglesStream;
        ShaderStorageBuffer OctreeBoxes;
        ShaderStorageBuffer LightsSSBO;
        int TriangleCount = 0;
        int BoxesCount = 0;
        int LightsCount = 0;
        ComputeShader TracerShader;
        public PathTracer()
        {
            TracerShader = new ComputeShader("PathTracer.compute.glsl");
            LightsSSBO = new ShaderStorageBuffer();
        }

        public void SetLights(List<PointLight> lights)
        {
            LightsCount = lights.Count;
            List<byte> bytes = new List<byte>();
            foreach(var l in lights)
            {
                bytes.AddRange(BitConverter.GetBytes(l.Position.X));
                bytes.AddRange(BitConverter.GetBytes(l.Position.Y));
                bytes.AddRange(BitConverter.GetBytes(l.Position.Z));
                bytes.AddRange(BitConverter.GetBytes(l.Radius));
                bytes.AddRange(BitConverter.GetBytes(l.Color.X));
                bytes.AddRange(BitConverter.GetBytes(l.Color.Y));
                bytes.AddRange(BitConverter.GetBytes(l.Color.Z));
                bytes.AddRange(BitConverter.GetBytes((float)l.Samples));
            }
            GLThread.Invoke(() => LightsSSBO.MapData(bytes.ToArray()));
        }
        
        private Random Rand = new Random();
        public void PathTraceToImage(MRTFramebuffer MRT, int imageHandle, int lastBuffer, int Width, int Height)
        {
            TracerShader.Use();
            MeshDataSSBO.Use(0);
            RandomsSSBO.MapData(JitterRandomSequenceGenerator.Generate(32, 32 * 32 * 32, true).ToArray());
            TracerShader.SetUniform("RandomsCount", 32 * 32 * 32);
            RandomsSSBO.Use(6);
            LightsSSBO.Use(7);
            TrianglesStream.Use(1);
            OctreeBoxes.Use(2);
            GL.BindImageTexture(0, imageHandle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);
            GL.BindImageTexture(1, lastBuffer, 0, false, 0, TextureAccess.ReadOnly, SizedInternalFormat.Rgba16f);
            GL.BindImageTexture(2, MRT.TexDiffuse, 0, false, 0, TextureAccess.ReadOnly, SizedInternalFormat.Rgba8);
            GL.BindImageTexture(3, MRT.TexNormals, 0, false, 0, TextureAccess.ReadOnly, SizedInternalFormat.Rgba16f);
            GL.BindImageTexture(4, MRT.TexWorldPos, 0, false, 0, TextureAccess.ReadOnly, SizedInternalFormat.Rgba16f);
            
            TracerShader.SetUniform("CameraPosition", Camera.Current.Transformation.GetPosition());
            TracerShader.SetUniform("ViewMatrix", Camera.MainDisplayCamera.ViewMatrix);
            TracerShader.SetUniform("ProjectionMatrix", Camera.MainDisplayCamera.ProjectionMatrix);
            TracerShader.SetUniform("Rand", (float)Rand.NextDouble());
            TracerShader.SetUniform("TrianglesCount", TriangleCount);
            TracerShader.SetUniform("TotalBoxesCount", BoxesCount);
            TracerShader.SetUniform("LightsCount", LightsCount);
            TracerShader.Dispatch(Width / 32 + 1, Height / 32 + 1);
        }

        public void PrepareTrianglesData(List<Mesh3d> meshes)
        {
            List<Triangle> triangles = new List<Triangle>();
            foreach(var mesh in meshes)
            {
                var Triangle = new Triangle();
                var vertices = mesh.MainObjectInfo.GetOrderedVertices();
                var normals = mesh.MainObjectInfo.GetOrderedNormals();
                for(int i = 0; i < vertices.Count; i++)
                {
                    var vertex = new Vertex()
                    {
                        Position = vertices[i],
                        Normal = normals[i],
                        Albedo = mesh.MainMaterial.Color.Xyz
                    };
                    vertex.Tranform(mesh.Matrix);
                    Triangle.Vertices.Add(vertex);
                    if(Triangle.Vertices.Count == 3)
                    {
                        triangles.Add(Triangle);
                        Triangle = new Triangle();
                    }
                }
            }
            SceneOctalTree tree = new SceneOctalTree();
            Triangle[] trcopy = new Triangle[triangles.Count];
            triangles.CopyTo(trcopy);
            tree.CreateFromTriangleList(trcopy.ToList());
            TriangleCount = triangles.Count;
            // lets prepare byte array
            // layout
            // posx, posy, poz, norx, nory, norz, albr, albg, albz
            List<byte> bytes = new List<byte>();
            foreach(var triangle in triangles)
            {
                foreach(var vertex in triangle.Vertices)
                {
                    bytes.AddRange(BitConverter.GetBytes(vertex.Position.X));
                    bytes.AddRange(BitConverter.GetBytes(vertex.Position.Y));
                    bytes.AddRange(BitConverter.GetBytes(vertex.Position.Z));
                    bytes.AddRange(BitConverter.GetBytes(0.0f));

                    bytes.AddRange(BitConverter.GetBytes(vertex.Normal.X));
                    bytes.AddRange(BitConverter.GetBytes(vertex.Normal.Y));
                    bytes.AddRange(BitConverter.GetBytes(vertex.Normal.Z));
                    bytes.AddRange(BitConverter.GetBytes(0.0f));

                    bytes.AddRange(BitConverter.GetBytes(vertex.Albedo.X));
                    bytes.AddRange(BitConverter.GetBytes(vertex.Albedo.Y));
                    bytes.AddRange(BitConverter.GetBytes(vertex.Albedo.Z));
                    bytes.AddRange(BitConverter.GetBytes(0.0f));
                }
            }
            MeshDataSSBO = new ShaderStorageBuffer();
            TrianglesStream = new ShaderStorageBuffer();
            OctreeBoxes = new ShaderStorageBuffer();
            GLThread.Invoke(() =>
            {
                MeshDataSSBO.MapData(bytes.ToArray());
                tree.PopulateSSBOs(TrianglesStream, OctreeBoxes);
                BoxesCount = tree.TotalBoxesCount;
            });
            RandomsSSBO = new ShaderStorageBuffer();
        }
    }
}