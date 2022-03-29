using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AvaliSpaceProgram
{
    public class Avali
    {
        public string name;
        public Mesh mesh;
        public Material main, feathers2, eyes, extra;
        public string trait = "Pilot";
        public float courage = 0.5f, stupidity = 0.5f, scale = 1.0f;
        public bool badass = false;
        public ProtoCrewMember.Gender gender = ProtoCrewMember.Gender.Female;
    }

    public class AvaliConfigs
    {
        public static AvaliConfigs instance = null;
        private Dictionary<string, Avali> avalis = new Dictionary<string, Avali>();
        private static Matrix4x4 bindpose;

        public IEnumerable<string> AvaliNames { get { return avalis.Keys; } }

        public static string aspAssetBundlesPath = Path.Combine(KSPUtil.ApplicationRootPath, "GameData", "AvaliSpaceProgram", "Assets", "asp.ksp");
        public AvaliConfigs()
        {
        }

        private Avali CreateAvali(string name)
        {
            var avali = new Avali();
            avali.name = name;
            return avalis[name] = avali;
        }

        /*
         * Avali Assets (from bundle)
         */
        private static bool loaded = false;
        private static Dictionary<string, Mesh> avaliMeshes = new Dictionary<string, Mesh>();
        private static Dictionary<string, Material> avaliMaterials = new Dictionary<string, Material>();
        private static Dictionary<string, AnimationClip> avaliAnimations = new Dictionary<string, AnimationClip>();
        private static UrlDir.UrlConfig[] configFiles;

        public void Load()
        {
            if (loaded) return;
            Avali avali;

            bindpose = GetBindPose();
            Matrix4x4 mat = new Matrix4x4();
            mat.m10 = -bindpose.m00;
            mat.m11 = -bindpose.m01;
            mat.m12 = -bindpose.m02;
            mat.m13 = -bindpose.m03;
            mat.m00 = bindpose.m10;
            mat.m01 = bindpose.m11;
            mat.m02 = bindpose.m12;
            mat.m03 = bindpose.m13;
            mat.m20 = bindpose.m20;
            mat.m21 = bindpose.m21;
            mat.m22 = bindpose.m22;
            mat.m23 = bindpose.m23;
            mat.m30 = bindpose.m30;
            mat.m31 = bindpose.m31;
            mat.m32 = bindpose.m32;
            mat.m33 = bindpose.m33;
            bindpose = mat;

            LoadAssetBundle(aspAssetBundlesPath);

            Mesh defaultHead = null;
            avaliMeshes.TryGetValue("avali_head", out defaultHead);
            if (defaultHead == null)
            {
                defaultHead = new Mesh();
                Debug.Log("[ASP] ERROR: Default Avali head mesh is missing from asset bundle!");
            }

            Mesh withCollarMesh = null;
            avaliMeshes.TryGetValue("avali_with_collar", out withCollarMesh);
            if (withCollarMesh == null) withCollarMesh = defaultHead;

            configFiles = GameDatabase.Instance.GetConfigs("AvaliSpaceProgram");
            foreach (UrlDir.UrlConfig config in configFiles)
            {
                foreach(ConfigNode node in config.config.GetNodes("Avali"))
                {
                    avali = CreateAvali(node.GetValue("name"));
                    Debug.Log("[ASP] Loading custom Avali \"" + avali.name + "\"");
                    ConfigNode tnode = node.GetNode("Textures");
                    avali.mesh = defaultHead;
                    avali.main = LoadConfigMaterial(tnode, "body");
                    avali.feathers2 = LoadConfigMaterial(tnode, "feathers");
                    avali.eyes = LoadConfigMaterial(tnode, "eyes");
                    if (node.HasNode("Collar"))
                    {
                        avali.mesh = withCollarMesh;
                        avali.extra = LoadConfigMaterial(node.GetNode("Collar"), "collar");
                    }
                    string trait = node.GetValue("trait");
                    if (trait.Equals("Scientist") || trait.Equals("Engineer") || trait.Equals("Pilot"))
                    {
                        avali.trait = trait;
                    }
                    else
                    {
                        Debug.Log("[ASP] ERROR: Unknown trait " + trait);
                        avali.trait = "Pilot";
                    }
                    avali.gender = node.GetValue("gender").ToLower().Equals("female") ? ProtoCrewMember.Gender.Female : ProtoCrewMember.Gender.Male;
                    avali.courage = float.Parse(node.GetValue("courage"));
                    avali.stupidity = float.Parse(node.GetValue("stupidity"));
                }
            }

            {
                avali = CreateAvali("Talali");
                avali.mesh = defaultHead;
                avali.main = GetMaterial("TalaliMain");
                avali.eyes = GetMaterial("TalaliEyes");
                avali.feathers2 = GetMaterial("TalaliFeathers2");
                avali.trait = "Scientist";
                avali.gender = ProtoCrewMember.Gender.Male;
                avali.courage = 0.3f;
                avali.stupidity = 0.0f;
            }

            {
                avali = CreateAvali("Einali");
                avali.mesh = defaultHead;
                avali.main = GetMaterial("EinaliMain");
                avali.eyes = GetMaterial("EinaliEyes");
                avali.feathers2 = GetMaterial("EinaliFeathers2");
                avali.trait = "Engineer";
                avali.gender = ProtoCrewMember.Gender.Female;
                avali.courage = 0.0f;
                avali.stupidity = 0.5f;
            }

            {
                avali = CreateAvali("Jasii");
                avali.mesh = defaultHead;
                avali.main = GetMaterial("JasiiMain");
                avali.eyes = GetMaterial("JasiiEyes");
                avali.feathers2 = GetMaterial("JasiiFeathers2");
                avali.trait = "Pilot";
                avali.gender = ProtoCrewMember.Gender.Female;
                avali.courage = 0.6f;
                avali.stupidity = 0.3f;
            }

            {
                avali = CreateAvali("Jasumi");
                avali.mesh = defaultHead;
                avali.main = GetMaterial("JasumiMain");
                avali.eyes = GetMaterial("JasumiEyes");
                avali.feathers2 = GetMaterial("JasumiFeathers2");
                avali.trait = "Scientist";
                avali.gender = ProtoCrewMember.Gender.Male;
                avali.courage = 0.1f;
                avali.stupidity = 0.0f;
            }

            {
                avali = CreateAvali("Kala");
                avali.mesh = defaultHead;
                avali.main = GetMaterial("KalaMain");
                avali.eyes = GetMaterial("KalaEyes");
                avali.feathers2 = GetMaterial("KalaFeathers2");
                avali.trait = "Engineer";
                avali.gender = ProtoCrewMember.Gender.Male;
                avali.courage = 0.0f;
                avali.stupidity = 0.6f;
            }

            {
                avali = CreateAvali("Khaun");
                avali.mesh = defaultHead;
                avali.main = GetMaterial("KhaunMain");
                avali.eyes = GetMaterial("KhaunEyes");
                avali.feathers2 = GetMaterial("KhaunFeathers2");
                avali.trait = "Pilot";
                avali.gender = ProtoCrewMember.Gender.Male;
                avali.courage = 1.0f;
                avali.stupidity = 0.4f;
                avali.badass = true;
            }

            {
                avali = CreateAvali("Naii");
                avali.mesh = defaultHead;
                avali.main = GetMaterial("NaiiMain");
                avali.eyes = GetMaterial("NaiiEyes");
                avali.feathers2 = GetMaterial("NaiiFeathers2");
                avali.trait = "Scientist";
                avali.gender = ProtoCrewMember.Gender.Female;
                avali.courage = 0.3f;
                avali.stupidity = 0.3f;
            }

            {
                avali = CreateAvali("Nesum");
                avali.mesh = defaultHead;
                avali.main = GetMaterial("NesumMain");
                avali.eyes = GetMaterial("NesumEyes");
                avali.feathers2 = GetMaterial("NesumFeathers2");
                avali.trait = "Engineer";
                avali.gender = ProtoCrewMember.Gender.Female;
                avali.courage = 0.4f;
                avali.stupidity = 0.1f;
            }

            {
                avali = CreateAvali("Ranyo");
                avali.mesh = defaultHead;
                avali.main = GetMaterial("RanyoMain");
                avali.eyes = GetMaterial("RanyoEyes");
                avali.feathers2 = GetMaterial("RanyoFeathers2");
                avali.trait = "Pilot";
                avali.gender = ProtoCrewMember.Gender.Male;
                avali.courage = 0.4f;
                avali.stupidity = 0.5f;
            }

            {
                avali = CreateAvali("Rhaii");
                avali.mesh = defaultHead;
                avali.main = GetMaterial("RhaiiMain");
                avali.eyes = GetMaterial("RhaiiEyes");
                avali.feathers2 = GetMaterial("RhaiiFeathers2");
                avali.trait = "Scientist";
                avali.gender = ProtoCrewMember.Gender.Female;
                avali.courage = 0.6f;
                avali.stupidity = 0.1f;
            }

            loaded = true;
        }

        private static Matrix4x4 GetBindPose()
        {
            var eva = PartLoader.getPartInfoByName("kerbalEVAfemale").partPrefab.gameObject;
            foreach (SkinnedMeshRenderer smr in eva.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                switch (smr.name)
                {
                    case "headMesh01":
                    case "mesh_female_kerbalAstronaut01_kerbalGirl_mesh_polySurface51":
                    case "headMesh":
                        int i = 0;
                        foreach (var bone in smr.bones)
                        {
                            if (bone.name == "bn_upperJaw01")
                            {
                                return smr.sharedMesh.bindposes[i];
                            }
                            i++;
                        }
                        break;
                }
            }
            System.Diagnostics.Debug.Assert(false, "GetBindPose failed");
            return new Matrix4x4();
        }

        private static void LoadAssetBundle(string path)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(path);
            Debug.Log("[ASP] Dump asset names");
            foreach (String s in bundle.GetAllAssetNames()) Debug.Log(s);

            Debug.Log("[ASP] Loading Avali meshes");
            Mesh[] meshes = bundle.LoadAllAssets<Mesh>();
            foreach (Mesh m in meshes)
            {
                Debug.Log(m.name);
                m.bindposes = new Matrix4x4[] { bindpose };
                BoneWeight[] weights = new BoneWeight[m.vertices.Length];
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i].boneIndex0 = 0;
                    weights[i].weight0 = 1;
                }
                m.boneWeights = weights;
                m.MarkModified();
                avaliMeshes.Add(m.name, m);
            }
            
            Debug.Log("[ASP] Loading Avali materials");
            Material[] materials = bundle.LoadAllAssets<Material>();
            foreach (Material m in materials)
            {
                Debug.Log(m.name + " - " + m.shader.name);
                avaliMaterials.Add(m.name, m);
            }

            Debug.Log("[ASP] Loading Avali animations");
            AnimationClip[] anim = bundle.LoadAllAssets<AnimationClip>();
            foreach(AnimationClip a in anim){
                Debug.Log(a.name);
                avaliAnimations.Add(a.name, a);
            }
        }

        private static Material GetMaterial(string name)
        {
            Material material = null;
            avaliMaterials.TryGetValue(name, out material);
            if(material == null)
            {
                material = new Material(Shader.Find("Diffuse"));
                material.color = new Color(1, 0, 1);
            }
            return material;
        }

        public static AnimationClip GetAnimation(string name)
        {
            AnimationClip anim = null;
            avaliAnimations.TryGetValue(name, out anim);
            return anim;
        }

        public Avali GetAvali(string name)
        {
            Avali avali = null;
            avalis.TryGetValue(name, out avali);
            return avali;
        }

        private Material CreateMaterial(Texture2D tex, Color col, Texture2D emissive, Color emissiveColor)
        {
            var mat = new Material(Shader.Find(emissive ? "KSP/Emissive/Diffuse" : "KSP/Diffuse"));
            mat.mainTexture = tex;
            mat.color = col;
            if (emissive)
            {
                mat.SetTexture("_Emissive", emissive);
                mat.SetColor("_EmissiveColor", emissiveColor);
            }
            return mat;
        }

        private Material LoadConfigMaterial(ConfigNode tnode, string name)
        {
            Texture2D mainTex = GameDatabase.Instance.GetTexture(tnode.GetValue(name + "Texture"), false);
            Color mainColor = Color.white;
            if (tnode.HasValue(name + "Color")) mainColor = ConfigNode.ParseColor(tnode.GetValue(name + "Color"));
            Texture2D emissivesTex = null;
            Color emissiveColor = Color.white;
            if (tnode.HasValue(name + "EmissiveMap"))
            {
                emissivesTex = GameDatabase.Instance.GetTexture(tnode.GetValue(name + "EmissiveMap"), false);
                if (tnode.HasValue(name + "EmissiveColor")) emissiveColor = ConfigNode.ParseColor(tnode.GetValue(name + "EmissiveColor"));
            }
            return CreateMaterial(mainTex, mainColor, emissivesTex, emissiveColor);
        }
    }
}

