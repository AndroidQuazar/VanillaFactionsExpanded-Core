﻿using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace VanillaStorytellersExpanded
{
    public class QuestGiverManager : IExposable
    {
        public QuestGiverDef def;
        private List<QuestInfo> availableQuests = new List<QuestInfo>();
        public List<QuestInfo> AvailableQuests => availableQuests;
        private int lastResetTick;
        public QuestGiverManager()
        {

        }

        public QuestGiverManager(QuestGiverDef def)
        {
            this.def = def;
            this.availableQuests = new List<QuestInfo>();
        }
        public void Tick()
        {
            if (def.resetEveryTick != -1 && Find.TickManager.TicksAbs > lastResetTick + def.resetEveryTick)
            {
                Reset();
            }
        }

        public void Init()
        {
            GenerateQuests();
        }
        public void Reset()
        {
            availableQuests.Clear();
            GenerateQuests();
            lastResetTick = Find.TickManager.TicksAbs;
        }

        public void GenerateQuests()
        {
            availableQuests.AddRange(def.Worker.GenerateQuests(this));
        }

        public void ActivateQuest(Pawn accepter, QuestInfo questInfo)
        {
            Log.Message("1 questInfo.quest: " + questInfo.quest.State + " - " + questInfo.quest.initiallyAccepted);
            Find.QuestManager.Add(questInfo.quest);
            questInfo.quest.Accept(accepter);
            QuestUtility.SendLetterQuestAvailable(questInfo.quest);
            questInfo.currencyInfo?.Buy(questInfo);
            availableQuests.Remove(questInfo);
            Log.Message("2 questInfo.quest: " + questInfo.quest.State + " - " + questInfo.quest.initiallyAccepted);
        }

        public void CallWindow()
        {
            var window = (Window)Activator.CreateInstance(this.def.windowClass, this);
            Find.WindowStack.Add(window);
        }
        public void ExposeData()
        {
            Scribe_Collections.Look(ref availableQuests, "availableQuests", LookMode.Deep);
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref lastResetTick, "lastResetTick");
        }
    }
}
