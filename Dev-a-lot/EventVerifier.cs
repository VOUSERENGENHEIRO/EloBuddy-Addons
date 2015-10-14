﻿using System;
using System.IO;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace TestAddon
{
    public static class EventVerifier
    {
        public static Menu Menu { get; set; }

        #region Menu Values

        private static bool BasicAttack
        {
            get { return Menu["basicAttack"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool SpellCast
        {
            get { return Menu["spellCast"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool ProcessSpell
        {
            get { return Menu["processSpell"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool NewPath
        {
            get { return Menu["newPath"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool StopCast
        {
            get { return Menu["stopCast"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool Create
        {
            get { return Menu["create"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool Delete
        {
            get { return Menu["delete"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool Animation
        {
            get { return Menu["animation"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool BuffGain
        {
            get { return Menu["buffGain"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool BuffLose
        {
            get { return Menu["buffLose"].Cast<CheckBox>().CurrentValue; }
        }

        #endregion

        static EventVerifier()
        {
            #region Menu Creation

            // Create the menu
            Menu = Program.Menu.AddSubMenu("Event verifier");

            Menu.AddGroupLabel("Core Event Verifier");
            Menu.AddLabel("Note: This might cause your game to crash! Only use this if you know what you are doing!");
            Menu.Add("basicAttack", new CheckBox("Obj_AI_Base.OnBasicAttack", false)).CurrentValue = false;
            Menu.Add("spellCast", new CheckBox("Obj_AI_Base.OnSpellCast", false)).CurrentValue = false;
            Menu.Add("processSpell", new CheckBox("Obj_AI_Base.OnProcessSpellCast", false)).CurrentValue = false;
            Menu.Add("stopCast", new CheckBox("Spellbook.OnStopCast", false)).CurrentValue = false;
            Menu.Add("newPath", new CheckBox("Obj_AI_Base.OnNewPath", false)).CurrentValue = false;
            Menu.Add("animation", new CheckBox("Obj_AI_Base.OnPlayAnimation (laggy)", false)).CurrentValue = false;
            Menu.Add("create", new CheckBox("GameObject.OnCreate", false)).CurrentValue = false;
            Menu.Add("delete", new CheckBox("GameObject.OnDelete", false)).CurrentValue = false;
            Menu.Add("buffGain", new CheckBox("Obj_AI_Base.OnBuffGain", false)).CurrentValue = false;
            Menu.Add("buffLose", new CheckBox("Obj_AI_Base.OnBuffLose", false)).CurrentValue = false;

            Menu.AddSeparator();
            Menu.AddLabel(string.Format("Note: All of those tests will create a folder on your Desktop called '{0}'!", Path.GetFileName(Program.ResultPath)));

            #endregion

            #region Event Handling

            // Listen to required events
            Obj_AI_Base.OnBasicAttack += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (BasicAttack)
                {
                    Verify(sender, args, "OnBasicAttack");
                }
            };
            Obj_AI_Base.OnSpellCast += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (SpellCast)
                {
                    Verify(sender, args, "OnSpellCast");
                }
            };
            Obj_AI_Base.OnProcessSpellCast += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (ProcessSpell)
                {
                    Verify(sender, args, "OnProcessSpellCast");
                }
            };
            Spellbook.OnStopCast += delegate(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
            {
                if (StopCast)
                {
                    Verify(sender, args, "OnStopCast");
                }
            };
            Obj_AI_Base.OnBuffGain += delegate(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
            {
                if (BuffGain)
                {
                    Verify(sender, args, "OnProcessSpellCast");
                }
            };
            Obj_AI_Base.OnBuffLose += delegate(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
            {
                if (BuffLose)
                {
                    Verify(sender, args, "OnBuffLose");
                }
            };
            Obj_AI_Base.OnNewPath += delegate(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
            {
                if (NewPath)
                {
                    Verify(sender, args, "OnNewPath");
                }
            };
            Obj_AI_Base.OnPlayAnimation += delegate(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
            {
                if (Animation)
                {
                    Verify(sender, args, "OnPlayAnimation");
                }
            };
            GameObject.OnCreate += delegate(GameObject sender, EventArgs args)
            {
                if (Create)
                {
                    Verify(sender, args, "OnCreate");
                }
            };
            GameObject.OnDelete += delegate(GameObject sender, EventArgs args)
            {
                if (Delete)
                {
                    Verify(sender, args, "OnDelete");
                }
            };

            #endregion
        }

        public static void Initialize()
        {
        }

        private static void Verify(GameObject sender, EventArgs args, string eventName)
        {
            if (!Directory.Exists(Program.ResultPath))
            {
                Directory.CreateDirectory(Program.ResultPath);
            }

            using (var writer = File.CreateText(Path.Combine(Program.ResultPath, eventName + ".txt")))
            {
                using (var analyzer = new GameObjectDiagnosis(sender, writer))
                {
                    analyzer.Analyze(sender, true, false);
                    analyzer.Analyze(args, false, false);
                }
            }
        }
    }
}
