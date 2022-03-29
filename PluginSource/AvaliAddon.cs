using System;
using System.Linq;
using UnityEngine;

namespace AvaliSpaceProgram
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class AvaliAddon : MonoBehaviour
    {
        private bool isInitialized = false;
        private Game game;

        public void Start()
        {
            AvaliConfigs.instance = new AvaliConfigs();
            GameEvents.onKerbalAddComplete.Add(onKerbalAdd);
            GameEvents.onGameStateCreated.Add(setGame);
            DontDestroyOnLoad(this);
        }

        public void LateUpdate()
        {
            if (!isInitialized && PartLoader.Instance.IsReady())
            {
                isInitialized = true;
                InitPrefabs();

                AvaliConfigs.instance.Load();
            }
        }

        private void InitIvaModule(Component c)
        {
            c.gameObject.AddComponent<IvaModule>();
        }


        private void InitEvaModule(Component c)
        {
            c.gameObject.AddComponent<EvaModule>();
        }
        private void InitPrefabs()
        {
            foreach (Kerbal kerbal in Resources.FindObjectsOfTypeAll<Kerbal>())
            {
                InitIvaModule(kerbal);
            }
            InitEvaModule(PartLoader.getPartInfoByName("kerbalEVAfemale").partPrefab);
            InitEvaModule(PartLoader.getPartInfoByName("kerbalEVA").partPrefab);

            // Folowing code is borrowed from TextureReplacer mod

            // Future Kerbals don't have prefab models loaded. We need to load them from assets.
            AssetBundle serenityBundle = AssetBundle.GetAllLoadedAssetBundles()
                .FirstOrDefault(b => b.name == "serenity_assets");

            if (serenityBundle != null)
            {
                const string maleIvaFuturePrefab = "assets/expansions/serenity/kerbals/iva/kerbalmalefuture.prefab";
                const string femaleIvaFuturePrefab = "assets/expansions/serenity/kerbals/iva/kerbalfemalefuture.prefab";

                var MaleIvaFuture = serenityBundle.LoadAsset(maleIvaFuturePrefab) as GameObject;
                var FemaleIvaFuture = serenityBundle.LoadAsset(femaleIvaFuturePrefab) as GameObject;
                var MaleEvaFuture = PartLoader.getPartInfoByName("kerbalEVAFuture").partPrefab;
                var FemaleEvaFuture = PartLoader.getPartInfoByName("kerbalEVAfemaleFuture").partPrefab;
                InitIvaModule(MaleIvaFuture.transform);
                InitIvaModule(FemaleIvaFuture.transform);
                InitEvaModule(MaleEvaFuture);
                InitEvaModule(FemaleEvaFuture);
            }

            // Vintage Kerbals don't have prefab models loaded. We need to load them from assets.
            AssetBundle missionsBundle = AssetBundle.GetAllLoadedAssetBundles()
                .FirstOrDefault(b => b.name == "makinghistory_assets");

            if (missionsBundle != null)
            {
                const string maleIvaVintagePrefab = "assets/expansions/missions/kerbals/iva/kerbalmalevintage.prefab";
                const string femaleIvaVintagePrefab = "assets/expansions/missions/kerbals/iva/kerbalfemalevintage.prefab";

                var MaleIvaVintage = missionsBundle.LoadAsset(maleIvaVintagePrefab) as GameObject;
                var FemaleIvaVintage = missionsBundle.LoadAsset(femaleIvaVintagePrefab) as GameObject;
                var MaleEvaVintage = PartLoader.getPartInfoByName("kerbalEVAVintage").partPrefab;
                var FemaleEvaVintage = PartLoader.getPartInfoByName("kerbalEVAfemaleVintage").partPrefab;
                InitIvaModule(MaleIvaVintage.transform);
                InitIvaModule(FemaleIvaVintage.transform);
                InitEvaModule(MaleEvaVintage);
                InitEvaModule(FemaleEvaVintage);
            }

            foreach (InternalModel model in Resources.FindObjectsOfTypeAll<InternalModel>())
            {
                if (model.GetComponent<ShipInteriorModule>() == null)
                {
                    model.gameObject.AddComponent<ShipInteriorModule>();
                }
            }
        }

        private static int applicantsCounter = 0;

        private void onKerbalAdd(ProtoCrewMember crew)
        {
            if (crew.type != ProtoCrewMember.KerbalType.Applicant) return;
            applicantsCounter++;
            if (applicantsCounter % 5 != 0) return;

            foreach (var name in AvaliConfigs.instance.AvaliNames)
            {
                if (!game.CrewRoster.Exists(name))
                {
                    Debug.Log(String.Format("[ASP] Avalifying {0} to {1}", crew.name, name));
                    crew.ChangeName(name);
                    AvalifyCrewMember(crew);
                    break;
                }
            }
        }

        private void AvalifyCrewMember(ProtoCrewMember crew)
        {
            var avali = AvaliConfigs.instance.GetAvali(crew.name);
            if (avali == null) return;

            crew.gender = avali.gender;
            crew.courage = avali.courage;
            crew.stupidity = avali.stupidity;
            crew.isBadass = avali.badass;

            KerbalRoster.SetExperienceTrait(crew, avali.trait);
        }

        private void setGame(Game g)
        {
            game = g;
            foreach (var crew in game.CrewRoster.Crew)
            {
                AvalifyCrewMember(crew);
            }
        }

    }
}

