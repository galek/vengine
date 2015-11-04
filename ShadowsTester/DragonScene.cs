﻿using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using VEngine;
using VEngine.FileFormats;
using VEngine.Generators;

namespace ShadowsTester
{
    public class DragonScene
    {
        public DragonScene()
        {
            var scene = World.Root.RootScene;
            var whiteboxInfo = Object3dInfo.LoadFromObjSingle(Media.Get("whiteroom.obj"));
            var whitebox = new Mesh3d(whiteboxInfo, new GenericMaterial(new Vector4(1000, 1000, 1000, 1000)));
            whitebox.Scale(3000);
            whitebox.Translate(0, -1500, 0);
            scene.Add(whitebox);
            /*  Func<uint, uint, float> terrainGen = (x, y) =>
              {
                  return
                      (SimplexNoise.Noise.Generate((float)x , (float)y) * 12) +
                      (SimplexNoise.Noise.Generate((float)x / 11, (float)y / 22) * 70) +
                      (SimplexNoise.Noise.Generate((float)x / 210, (float)y / 228) * 118) +
                      (SimplexNoise.Noise.Generate((float)x / 634, (float)y / 532) * 555) +
                      (SimplexNoise.Noise.Generate((float)x / 1696, (float)y / 1793) * 870);
              };
              Object3dGenerator.UseCache = false;
              Object3dInfo groundInfo = Object3dGenerator.CreateTerrain(new Vector2(-3000, -3000), new Vector2(3000, 3000), new Vector2(1120, 1120), Vector3.UnitY, 800, terrainGen);
              */
            Object3dInfo groundInfo = Object3dGenerator.CreateTerrain(new Vector2(-12, -12), new Vector2(12, 12), new Vector2(600, 600), Vector3.UnitY, 3, (x, y) => 0);
            var color3 = GenericMaterial.FromMedia("kafel2_a.png", "kafel2_n.png");
            //var color2 = new GenericMaterial(Color.WhiteSmoke);
            // color3.SetBumpMapFromMedia("DisplaceIT_Ground_Pebble1_Displace.bmp");
            // color3.SetNormalMapFromMedia("DisplaceIT_Ground_Pebble1_NormalBump2.bmp");
            // color3.Type = GenericMaterial.MaterialType.Parallax;

            // RainSystem rs = new RainSystem(1.0f, 300, 100.0f, 1.0f);

            color3.Metalness = 0;
            color3.Roughness = 0.2f;
            //color3.Type = GenericMaterial.MaterialType.Parallax;
            //color3.SetRainSystem(rs);
            Mesh3d water3 = new Mesh3d(groundInfo, color3);
            water3.SetMass(0);
            water3.Scale(30);
            water3.Translate(-100, 0, 0);
            scene.Add(water3);
            Random rand = new Random();
            /* GLThread.CreateTimer(() =>
             {
                 for(int i = 0; i < 10; i++)
                 {
                     Vector3 v = new Vector3((float)(rand.NextDouble() * 2.0 - 1.0) * 10.0f, 0.0f, (float)(rand.NextDouble() * 2.0 - 1.0) * 10.0f);
                     rs.AddDrop(v);
                 }
             }, 60).Start();*/

            var ss = new GameScene("ferrari.scene");
            ss.Load();
            var transfs = new List<Vector4>();
            for(int i = 0; i < 64; i++)
            {
                transfs.Add(new Vector4(
                    ((float)rand.NextDouble()) * 20,
                    0,
                    ((float)rand.NextDouble()) * 20,
                    ((float)rand.NextDouble()) * 0.6f + 0.5f
                    ));
            }
            ss.Meshes.ForEach((o) =>
            {
                scene.Add(o);
            });
            //var groundInfo = Object3dInfo.LoadFromObjSingle(Media.Get("ldspc.obj"));
            //var sph1 = Object3dInfo.LoadFromObjSingle(Media.Get("sph1.obj"));
            //ldspc.obj
            /* Object3dInfo waterInfo = Object3dInfo.LoadFromRaw(Media.Get("Realistic tree.vbo.raw"), Media.Get("Realistic tree.indices.raw"));
             Object3dInfo waterInfo2 = Object3dInfo.LoadFromObjSingle(Media.Get("terrain_simplified_normalized.obj"));
             Object3dInfo[] domks = Object3dInfo.LoadFromObj(Media.Get("test_ball.obj"));

             var roof = new Mesh3d(domks[0], new GenericMaterial(Color.White));
             var walls = new Mesh3d(domks[1], new GenericMaterial(Color.White));
             // scene.Add(roof); scene.Add(walls);

             var color = new GenericMaterial(Color.White);
             //color.SetNormalMapFromMedia("151_norm.JPG");
             color.Roughness = 1.0f;
             //color.SetBumpMapFromMedia("terrain_grassmap.png");
             // color.Type = GenericMaterial.MaterialType.Grass;
             waterInfo.Normalize();
             Mesh3d water = new Mesh3d(waterInfo, color);
             water.SetMass(0);
             water.Scale(2.2f);            
             
           //  scene.Add(water);*/
            Object3dInfo wall = Object3dGenerator.CreateCube(new Vector3(10.0f, 10.0f, 1.0f), new Vector2(1, 1));
            InstancedMesh3d wallsInst = new InstancedMesh3d(wall, new GenericMaterial(Color.Red));
            wallsInst.Transformations.Add(new TransformationManager(new Vector3(0, 5, 10), Quaternion.Identity, 1));
            wallsInst.Transformations.Add(new TransformationManager(new Vector3(0, 5, -10), Quaternion.Identity, 1));
            wallsInst.Transformations.Add(new TransformationManager(new Vector3(10, 5, 0), Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.PiOver2), 1));
            wallsInst.Transformations.Add(new TransformationManager(new Vector3(-10, 5, 0), Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.PiOver2), 1));
            wallsInst.Instances = 4;
            wallsInst.UpdateMatrix();
           // World.Root.CreateRigidBody(0, wallsInst.Transformations[0].GetWorldTransform(), new BulletSharp.BoxShape(wall.GetAxisAlignedBox() / 2), null);
        //    World.Root.CreateRigidBody(0, wallsInst.Transformations[1].GetWorldTransform(), new BulletSharp.BoxShape(wall.GetAxisAlignedBox() / 2), null);
        //    World.Root.CreateRigidBody(0, wallsInst.Transformations[2].GetWorldTransform(), new BulletSharp.BoxShape(wall.GetAxisAlignedBox() / 2), null);
        //    World.Root.CreateRigidBody(0, wallsInst.Transformations[3].GetWorldTransform(), new BulletSharp.BoxShape(wall.GetAxisAlignedBox() / 2), null);
          //  scene.Add(wallsInst);

            //World.Root.CreateRigidBody(0, Matrix4.Identity, new BulletSharp.StaticPlaneShape(Vector3.UnitY, 0), null);
            /*
            var roadtile = Object3dGenerator.CreateGround(new Vector2(-0.47f, -0.5f) * 20, new Vector2(0.47f, 0.5f) * 20, new Vector2(1, 1), Vector3.UnitY);
            var roadsInstances = new InstancedMesh3d(roadtile, GenericMaterial.FromMedia("roadtex.png", "roadnormaltex.png"));
            var advancer = new Vector3(0, 0, (roadtile.GetAxisAlignedBox() * 2).Z);
            roadsInstances.Transformations.Add(new TransformationManager(new Vector3(0, 0.1f, 0)));
            for(int i = 0; i < 120; i++)
            {
                roadsInstances.Transformations.Add(new TransformationManager(new Vector3(0, 0.1f, 0) + advancer * i));
            }
            roadsInstances.UpdateMatrix();
            Add(roadsInstances);*/

            //Object3dInfo cube = Object3dGenerator.CreateCube(new Vector3(1, 1, 1), new Vector2(1, 1));
            /*var cube = Object3dInfo.LoadFromObjSingle(Media.Get("modernchair.obj"));
            cube.MakeDoubleFaced();
            GenericMaterial material = new GenericMaterial(new Vector4(1, 1, 1, 1.000f));
            int allCount = 0;
            var meshes = new List<Mesh3d>();
            Mesh3d lastmesh = null;
            //var rand = new Random();
            for(int y = 0; y < 100; y++)
            {
                Mesh3d mesh = new Mesh3d(cube, material);
                mesh.DisableDepthWrite = true;
                mesh.Transformation.SetPosition(new Vector3(0, (y + 10.0f) * 12.0f, 0));
                Vector3 scaleRand = new Vector3((float)rand.NextDouble() * 6.0f + 5.0f, (float)rand.NextDouble() * 6.0f + 5.0f, (float)rand.NextDouble() * 6.0f + 5.0f);
                mesh.SetMass(11.0f);
                mesh.Transformation.Scale(1);
                mesh.SetCollisionShape(new BulletSharp.BoxShape(cube.GetAxisAlignedBox() / 2));
                meshes.Add(mesh);
                World.Root.PhysicalWorld.AddCollisionObject(mesh.CreateRigidBody());
                if(lastmesh != null)
                {
                    //var offset = (mesh.Transformation.GetPosition() - lastmesh.Transformation.GetPosition()) / 2;
                    //var cst = new BulletSharp.FixedConstraint(mesh.PhysicalBody, lastmesh.PhysicalBody, (-offset).ToMatrix(), offset.ToMatrix());
                    //World.Root.PhysicalWorld.AddConstraint(cst, true);
                }

                lastmesh = mesh;
            }

            var inst = InstancedMesh3d.FromSimilarMesh3dList(meshes);
            GLThread.OnUpdate += (o, e) =>
            {
                inst.UpdateMatrix();
                //wallsInst.UpdateMatrix();
            };
            scene.Add(inst);
            Console.WriteLine("allCount " + allCount);*/

            /*
            Object3dInfo cathobj = Object3dInfo.LoadFromObjSingle(Media.Get("cathedral2.obj"));
            var cath = new Mesh3d(cathobj, new GenericMaterial(Color.LightSteelBlue));
            //cath.Scale(0.8f);
           // scene.Add(cath);
            
            Object3dInfo cathobja = Object3dInfo.LoadFromObjSingle(Media.Get("ryj.obj"));
            var catha = new Mesh3d(cathobja, GenericMaterial.FromMedia("ryj_albedo.jpg"));
            catha.MainMaterial.SetRoughnessMapFromMedia("ryj_roughness.jpg");
            //catha.MainMaterial.SetMetalnessMapFromMedia("girl_metalness.png");
            catha.Translate(70, 8.1f, 0);
            catha.Rotate(Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(-23)));
            catha.Scale(0.8f);
            scene.Add(catha);
            Object3dInfo chairobj = Object3dInfo.LoadFromObjSingle(Media.Get("modernchair.obj"));
            var chair = new Mesh3d(chairobj, new GenericMaterial(Color.LightBlue));
            chair.Translate(70, 8.1f, 0);
            //scene.Add(chair);
            Object3dInfo roomobj = Object3dInfo.LoadFromObjSingle(Media.Get("roomclean.obj"));
            var room = new Mesh3d(roomobj, new GenericMaterial(Color.WhiteSmoke));
            room.Translate(70, 8.1f, 0);*/
           // scene.Add(room);
           
            /*
            Object3dInfo fenceobj = Object3dInfo.LoadFromObjSingle(Media.Get("fenceplane.obj"));
            var fence = new Mesh3d(fenceobj, new GenericMaterial(Color.WhiteSmoke));
            fence.Translate(10, 11.1f, 11);
            fence.MainMaterial.Type = GenericMaterial.MaterialType.Parallax;
            fence.MainMaterial.SetBumpMapFromMedia("fendeBump.jpg");
            fence.MainMaterial.SetAlphaMaskFromMedia("fenceAlpha.jpg");
            fence.MainMaterial.SetNormalMapFromMedia("fence_normal.jpg");
            scene.Add(fence);
            // cath.MainMaterial.Type = GenericMaterial.MaterialType.Parallax;
            //  cath.MainMaterial.ParallaxHeightMultiplier = 0.06f;
            
            var color2 = GenericMaterial.FromMedia("riftenplazabrick01.jpg");
            //var color2 = new GenericMaterial(Color.WhiteSmoke);
            color2.SetBumpMapFromMedia("riftenplazabrick01_b.jpg");
            color2.SetNormalMapFromMedia("riftenplazabrick01_n.jpg");
            color2.Type = GenericMaterial.MaterialType.Parallax;
            color2.Metalness = 0.1f;
            color2.Roughness = 0.9f;
            Mesh3d water2 = new Mesh3d(groundInfo, color2);
            water2.SetMass(0);
            water2.Scale(30);
            scene.Add(water2);*/
            /*
            var color3 = GenericMaterial.FromMedia("DisplaceIT_Ground_Pebble1_Color.bmp");
            //var color2 = new GenericMaterial(Color.WhiteSmoke);
            color3.SetBumpMapFromMedia("DisplaceIT_Ground_Pebble1_Displace.bmp");
            color3.SetNormalMapFromMedia("DisplaceIT_Ground_Pebble1_NormalBump2.bmp");
            color3.Type = GenericMaterial.MaterialType.Parallax;
            color3.Metalness = 0;
            color3.Roughness = 0.2f;
            Mesh3d water3 = new Mesh3d(groundInfo, color3);
            water3.SetMass(0);
            water3.Scale(30);
            water3.Translate(-100, 0, 0);
            scene.Add(water3);*/
            /*
            var color4 = GenericMaterial.FromMedia("rock_02_dif.jpg");
            //var color2 = new GenericMaterial(Color.WhiteSmoke);
            color4.SetBumpMapFromMedia("rock_02_hm.jpg");
            color4.SetNormalMapFromMedia("rock_02_nm.jpg");
            // color4.SetRoughnessMapFromMedia("1r.jpg");
            color4.Type = GenericMaterial.MaterialType.Parallax;
            color4.Metalness = 0.0f;
            // color4.ParallaxHeightMultiplier = 3.5f;
            Mesh3d water4 = new Mesh3d(groundInfo, color4);
            water4.SetMass(0);
            water4.Scale(30);
            water4.Translate(100, 0, 0);
            scene.Add(water4);

            var color4a = GenericMaterial.FromMedia("mosaic_a.jpg");
            //var color2 = new GenericMaterial(Color.WhiteSmoke);
            color4a.SetBumpMapFromMedia("mosaic.jpg");
            color4a.SetNormalMapFromMedia("mosaic_n.jpg");
            // color4.SetRoughnessMapFromMedia("1r.jpg");
            color4a.Type = GenericMaterial.MaterialType.Parallax;
            color4a.Metalness = 0.0f;
            // color4.ParallaxHeightMultiplier = 3.5f;
            Mesh3d water4a = new Mesh3d(groundInfo, color4a);
            water4a.SetMass(0);
            water4a.Scale(30);
            water4a.Translate(100, 0, -100);
            scene.Add(water4a);*/
            /*
            var color5 = GenericMaterial.FromMedia("peas_albedo.jpg");
           // var color5 = new GenericMaterial(Color.White);
            color5.SetBumpMapFromMedia("peas_bump.jpg");
            color5.SetNormalMapFromMedia("peas_normal.png");
            //color5.SetSpecularMapFromMedia("Specullar.bmp");
            color5.Type = GenericMaterial.MaterialType.Parallax;
            color5.Metalness = 0.0f;
            var color6 = new GenericMaterial(Color.Green);
            color6.Type = GenericMaterial.MaterialType.Grass;
            color6.Metalness = 0.0f;
            color6.Roughness = 1.0f;
            // color4.ParallaxHeightMultiplier = 3.5f;
            Mesh3d water5 = new Mesh3d(groundInfo, color5);
            water5.SetMass(0);
            water5.Scale(30);
            water5.Translate(100, 0, 100);
            scene.Add(water5);
            Mesh3d sph2 = new Mesh3d(sph1, color5);
            sph2.SetMass(0);
            sph2.Scale(35);
            sph2.Translate(70, 120, 0);
            scene.Add(sph2);
            Mesh3d water6 = new Mesh3d(groundInfo, color6);
            water6.SetMass(0);
            water6.Scale(30);
            water6.Translate(100, 0, 100);
            *//*
            var oflow = Object3dInfo.LoadFromObj(Media.Get("flower.obj"));
            foreach(var of in oflow)
            {
                var me = new Mesh3d(of, GenericMaterial.FromMedia("PRIM1P.png"));
                scene.Add(me);
            }*/
              //scene.Add(water6);

            var tsc = Object3dInfo.LoadFromObjSingle(Media.Get("lightsphere.obj"));
            /*tsc[2].MainMaterial.Color = new Vector4(0.8f, 0.181f, 0.309f, 1);
            tsc[0].MainMaterial.Color = new Vector4(0.8f, 0.792f, 0.591f, 1);
            tsc[1].MainMaterial.Color = new Vector4(0.8f, 0.553f, 0.032f, 1);
            tsc[3].MainMaterial.Color = new Vector4(0.8f, 0.553f, 0.032f, 1);*/
            //tsc.ForEach((a) => scene.Add(a));

            var metal = new Mesh3d(tsc, new GenericMaterial(Color.WhiteSmoke));
            metal.MainMaterial.Metalness = 1;
            metal.MainMaterial.Roughness = 0.0f;
            metal.MainMaterial.SpecularComponent = 0.1f;
            metal.Translate(10, 0, -10);
            scene.Add(metal);
            /*
            var plastic = new Mesh3d(tsc, new GenericMaterial(Color.Green));
            plastic.MainMaterial.Metalness = 0;
            plastic.MainMaterial.Roughness = 0;
            plastic.MainMaterial.SpecularComponent = 1f;
            plastic.Translate(10, 0, -7);
            scene.Add(plastic);

            var diffuse = new Mesh3d(tsc, new GenericMaterial(Color.Green));
            diffuse.MainMaterial.Metalness = 0;
            diffuse.MainMaterial.Roughness = 1;
            diffuse.MainMaterial.SpecularComponent = 0.1f;
            diffuse.Translate(10, 0, -4);
            scene.Add(diffuse);

            var diffuse2 = new Mesh3d(tsc, new GenericMaterial(Color.Red));
            diffuse2.MainMaterial.Metalness = 0.3f;
            diffuse2.MainMaterial.Roughness = 0.5f;
            diffuse2.MainMaterial.SpecularComponent = 0.1f;
            diffuse2.Translate(10, 0, 0);
            scene.Add(diffuse2);

            var diffuse3 = new Mesh3d(tsc, new GenericMaterial(Color.Violet));
            diffuse3.MainMaterial.Metalness = 0.1f;
            diffuse3.MainMaterial.Roughness = 0.7f;
            diffuse3.MainMaterial.SpecularComponent = 1f;
            diffuse3.Translate(10, 0, 4);
            scene.Add(diffuse3);*/

            // var stukaobj = 

            for(int i = 0; i < 1; i++)
            {
                // Object3dInfo gridx = Object3dInfo.LoadFromRaw(Media.Get("lucy.vbo.raw"), Media.Get("lucy.indices.raw"));
                Object3dInfo gridx = Object3dInfo.LoadFromObjSingle(Media.Get("flagplane.obj"));
                gridx.Normalize();
                
                var gcx = new GenericMaterial(Color.White);
                gcx.Type = GenericMaterial.MaterialType.Flag;
                gcx.Roughness = 1.0f;
                gcx.SetAlphaMaskFromMedia("ornament.png");
                // gcx.IgnoreLighting = true;
                Mesh3d gex = new Mesh3d(gridx, gcx);
                gex.SetMass(0);
                gex.Scale(14f, 12, 4);
                gex.Translate(-12-i * 4, 0, 0);
                scene.Add(gex);
            }

            /* for(int z = 0; z < 20; z++)
             {
                 Object3dInfo grid = Object3dGenerator.CreateTerrain(new Vector2(-20, -20), new Vector2(20, 20), new Vector2(-4, 4), Vector3.UnitY, 2, (x2, y2) => 0);
                 var gc = new GenericMaterial(Color.White);
                 gc.SetAlphaMaskFromMedia("gridalphamask.png");
                 gc.Roughness = 1.0f;
                 gc.IgnoreLighting = true;
                 Mesh3d ge = new Mesh3d(grid, gc);
                 ge.SetMass(0);
                 ge.Translate(z, 0, 0);
                 ge.Rotate(Quaternion.FromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(90)));
                 Add(ge);
             }*//*
             var lod1 = Object3dInfo.LoadSceneFromObj(Media.Get("shipment.obj"), Media.Get("shipment.mtl"));
              lod1.ForEach((a) => Add(a));

             var chair = new Mesh3d(lod1, GenericMaterial.FromMedia("Cerberus_A.png"));
             chair.MainMaterial.SetNormalMapFromMedia("Cerberus_N.png");
             chair.MainMaterial.SetSpecularMapFromMedia("Cerberus_M.png");
             chair.MainMaterial.SetMetalnessMapFromMedia("Cerberus_M.png");
             chair.MainMaterial.SetRoughnessMapFromMedia("Cerberus_R.png");
             chair.Scale(10);
             Add(chair);*/

            /* var scene = Object3dInfo.LoadSceneFromObj(Media.Get("gold.obj"), Media.Get("gold.mtl"), 1.0f);
             foreach(var ob in scene)
             {
                 ob.SetMass(0);
                 ob.MainMaterial.Roughness = 0.05f;
                 ob.MainMaterial.Metalness = 0.7f;
                 ob.MainMaterial.Color = new Vector4(((float)229 / (float)0xFF), ((float)179 / (float)0xFF), ((float)44 / (float)0xFF), 1);
                 ob.MainMaterial.Mode = GenericMaterial.DrawMode.ColorOnly;
                 this.Add(ob);
             }*/
        }
    }
}