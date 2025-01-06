using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Here a have a list of predefined usernames and when a new username is needed, we fetch one random item from the list.
/// </summary>

namespace SnakeWarzIO
{
    public class NicknameGenerator : MonoBehaviour
    {
        public static NicknameGenerator instance;
        public GameObject stickyNamePrefab;
        public Transform nicknameParent;

        private void Awake()
        {
            instance = this;
        }

        public string GenerateNickname()
        {
            string names = "Lorder,Golum,Svego,Scratch,Minior,Scooter,TrailBoom,Bomber,Crot,Polos,CretoGard,Creamor,Scremer,Dread!!,Tormant,Grotar,trender,Porter,Potter,Mikel,Dragos,Crang,Creos,Lopas,Kayle,Toodas,Gababa,PilionLorak,Tommy,Dreamer,Josef,Joque,Kinderos,Solan,Sonar,Talos,Tanos,Ketozzzz,Dodo,Foley,Fooooty,Mikilos,Positron,Torman,Vivi,Jios,Pisko,Gurad,Jonny,Xeros,Zooomer,Zombie,velos,pooner,Spil,queqweqwe,asdas,qwee,BloodyKnight,xAngeLx,vlom,Maels,oskar61,wanderer_from,amaze,Z1KkY,Crysler,heletch,shipilov,Chacha,usist,zingo,excurs,capitan_beans,Cashish,Finalboss,LUNTIK,gour,Theknyazzz,American_SnIper,NIGHTMARE,007up,Dr.Dizel,RaNDoM,sportik,Roger,glx506,volandband,pas,Necron,edik_lukoyanov,Synchromesh,SolomA,Dron128,DeatHSoul,DangErXeTER,Psy,Forcas,Morgot,Persiana,Aspect,Kraken,Bender,Lynch,BigPapa,MadDog,Bowser,ODoyle,Bruise,Psycho,Cannon,Ranger,Clink,Ratchet,Cobra,Reaper,Colt,Rigs,Crank,Ripley,Creep,Roadkill,Daemon,Ronin,Decay,Rubble,Diablo,Sasquatch,Doom,Scar,Dracula,Shiver,Dragon,Skinner,Fender,SkullCrusher,Fester,Slasher,Fisheye,Steelshot,Flack,Surge,Gargoyle,Sythe,Grave,Trip,Gunner,Trooper,Hash,Tweek,Hashtag,Vein,Indominus,Void,Ironclad,Wardon,Killer,Wraith,Knuckles,Zero,Steel,Kevlar,Iranika,Lightning,Tito,BulletProof,FireBred,Titanium,Hurricane,Ironsides,IronCut,Tempest,IronHeart,SteelForge,Pursuit,SteelFoil,Upsurge,Uprising,Overthrow,Breaker,Sabotage,Dissent,Subversion,Rebellion,Insurgent,Loch,Golem,Wendigo,Rex,Hydra,Behemoth,Balrog,Manticore,Gorgon,Basilisk,Minotaur,Leviathan,Cerberus,Mothman,Sylla,Charybdis,Orthros,Baal,Cyclops,Satyr,Azrael,Mariy_Kis,KATUSHA,KinDer,Eva,BoSoranY,AlfabetkA,ANGEL";

            string[] selectedName = names.Split(char.Parse(","));
            string nickname = selectedName[Random.Range(0, selectedName.Length)];

            return nickname;
        }

        /// <summary>
        /// Create a sticky UI object that sitcks to the target snake and displays its nickname above its head.
        /// </summary>
        /// <param name="actor"></param>
        public void CreateStickyNickname(GameObject actor)
        {
            GameObject snn = Instantiate(stickyNamePrefab, actor.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
            snn.name = actor.name + "_Nickname";            
            snn.GetComponent<NicknameController>().UpdateNicknameData(actor, actor.GetComponent<Snake>().GetNickname());
            snn.transform.parent = nicknameParent;
        }
    }
}