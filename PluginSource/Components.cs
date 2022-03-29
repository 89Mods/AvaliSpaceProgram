using System;
using UnityEngine;

namespace AvaliSpaceProgram
{
    public class ShipInteriorModule : MonoBehaviour
    {
        public void Start()
        {
            // Avalifies all kerbals inside the ship's internal space after it was loaded.
            // See comments for TRIvaModelModule in TextureReplacer's code
            // in TRIvaModelModule.cs and Personaliser.cs files for more details
            foreach (Kerbal kerbal in GetComponentsInChildren<Kerbal>())
            {
                if (kerbal.GetComponent<IvaModule>() == null)
                {
                    kerbal.gameObject.AddComponent<IvaModule>();
                }
            }

            Destroy(this);
        }
    }

    public class IvaModule : MonoBehaviour
    {
        public void Start()
        {
            var kerbal = GetComponent<Kerbal>();
            new Avalifier(kerbal, kerbal.protoCrewMember).Avalify();
            gameObject.AddComponent<VisibilityChecker>();
            Destroy(this);
        }
    }

    public class EvaModule : PartModule
    {
        private bool isInitialised = false;

        public override void OnStart(StartState state)
        {
            if (!isInitialised)
            {
                isInitialised = true;
                new Avalifier(part, part.protoModuleCrew[0]).Avalify();
            }
        }
    }

    public class VisibilityChecker : MonoBehaviour
    {
        private Renderer head, eyes, feathers, extra, kerbalHead;

        public void Start()
        {
            foreach (var smr in GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                switch (smr.name)
                {
                    case "headMesh01":
                    case "mesh_female_kerbalAstronaut01_kerbalGirl_mesh_polySurface51":
                    case "headMesh":
                        kerbalHead = smr;
                        break;
                    case "avaliHead":
                        head = smr;
                        break;
                    case "avaliEyes":
                        eyes = smr;
                        break;
                    case "headFeathers":
                        feathers = smr;
                        break;
                    case "extra":
                        extra = smr;
                        break;
                }
            }
        }

        public void Update()
        {
            if (!head) return;

            // Hide all head meshes when in IVA first-person view
            bool visible = kerbalHead.enabled;
            if (head) head.enabled = visible;
            if (eyes) eyes.enabled = visible;
            if (feathers) feathers.enabled = visible;
            if (extra) extra.enabled = visible;
        }
    }
}
