using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace AvaliSpaceProgram
{
    public class Avalifier
    {
        Component component;
        ProtoCrewMember kerbal;
        Transform bone = null;
        int layer = 0;

        public Avalifier(Component component, ProtoCrewMember kerbal)
        {
            this.component = component;
            this.kerbal = kerbal;
        }

        public void Avalify()
        {
            var avali = AvaliConfigs.instance.GetAvali(kerbal.name);
            if (avali == null) return;

            foreach (var t in component.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == "bn_upperJaw01")
                {
                    bone = t;
                    break;
                }
            }
            System.Diagnostics.Debug.Assert(bone != null, "[ASP] cannot find bn_upperJaw01");

            var nullMesh = new Mesh();
            foreach (var smr in component.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                switch (smr.name)
                {
                    case "headMesh01":
                    case "headMesh02":
                    case "mesh_female_kerbalAstronaut01_kerbalGirl_mesh_polySurface51":
                    case "headMesh":

                    case "eyeballLeft":
                    case "eyeballRight":
                    case "pupilLeft":
                    case "pupilRight":
                    case "mesh_female_kerbalAstronaut01_kerbalGirl_mesh_eyeballLeft":
                    case "mesh_female_kerbalAstronaut01_kerbalGirl_mesh_eyeballRight":
                    case "mesh_female_kerbalAstronaut01_kerbalGirl_mesh_pupilLeft":
                    case "mesh_female_kerbalAstronaut01_kerbalGirl_mesh_pupilRight":

                    case "mesh_female_kerbalAstronaut01_kerbalGirl_mesh_pCube1": // ponytail
                    case "ponytail":
                    case "tongue":
                    case "upTeeth01":
                    case "upTeeth02":
                    case "mesh_female_kerbalAstronaut01_kerbalGirl_mesh_upTeeth01":
                    case "mesh_female_kerbalAstronaut01_kerbalGirl_mesh_downTeeth01":
                    case "downTeeth01":
                        smr.sharedMesh = nullMesh;
                        layer = smr.gameObject.layer;
                        break;
                }
            }

            AddModel("avali_head", avali.mesh, avali.extra != null ? new Material[] { avali.extra, avali.main, avali.eyes, avali.feathers2 } : new Material[] { avali.main, avali.eyes, avali.feathers2 });

            if (component.GetComponent<EvaModule>() != null)
            {
                component.transform.localScale *= avali.scale;
            }

            RuntimeAnimatorController anim = component.GetComponent<Animator>().runtimeAnimatorController;
            AnimatorOverrideController aoc = new AnimatorOverrideController(anim);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            for (int i = 0; i < anim.animationClips.Length; i++)
            {
                AnimationClip clip = anim.animationClips[i];
                Debug.Log(clip.name);
                if (clip.name.Contains("Head")) continue;
                if ((clip.name.Contains("facialExp_happy") || clip.name.Contains("faceExp_happy")) && !(clip.name.Contains("waa") && !clip.name.Contains("smirk")))
                {
                    AnimationClip newAnim = AvaliConfigs.GetAnimation(clip.name.Contains("happy_a") ? "avali_nwn" : "avali_happy");

                    if (newAnim != null) anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, newAnim));
                }else if (clip.name.Contains("faceExp_panic"))
                {
                    AnimationClip newAnim = AvaliConfigs.GetAnimation(clip.name.Contains("halfOpen") ? "avali_panic_half_open" : "avali_panic");
                    if(newAnim != null) anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, newAnim));
                }else if (clip.name.Contains("faceExp_sad"))
                {
                    AnimationClip newAnim = AvaliConfigs.GetAnimation("avali_sad");
                    if (newAnim != null) anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, newAnim));
                }
                else if (clip.name.Contains("faceExp_fun_ohAh"))
                {
                    AnimationClip newAnim = AvaliConfigs.GetAnimation("avali_ohAh");
                    if (newAnim != null) anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, newAnim));
                }
                else if (clip.name.Contains("faceExp_amazed"))
                {
                    AnimationClip newAnim = AvaliConfigs.GetAnimation("avali_amazed");
                    if (newAnim != null) anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, newAnim));
                }
                else if (clip.name.Contains("facialExp_excited"))
                {
                    AnimationClip newAnim = AvaliConfigs.GetAnimation("avali_excited");
                    if (newAnim != null) anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, newAnim));
                }
            }
            aoc.ApplyOverrides(anims);
            component.GetComponent<Animator>().runtimeAnimatorController = aoc;
        }

        public GameObject AddModel(string name, Mesh mesh, Material[] mats)
        {
            var obj = new GameObject(name);
            var smr = obj.AddComponent<SkinnedMeshRenderer>();
            smr.sharedMesh = mesh;
            smr.materials = mats;
            smr.bones = new Transform[] { bone };
            obj.transform.parent = component.transform;
            obj.layer = layer;
            return obj;
        }
    }
}

