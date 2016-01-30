using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PerfectlyParanormal.AchievementSystem
{
    /// <summary>
    /// Achievements for Manual Samuel, contains a list of achievements, as well as methods for accessing them based on an enum
    /// </summary>
    [System.Serializable]
    public class SamAchievements
    {
        public enum AchievementNames
        {
            Blink_2000_Times_For_Yes,
            Stairmaster_3000,
            Stairway_To_The_Livingroom,
            Piss_Perfect,
        }


        private List<AchievementBase> m_achievements;


        public SamAchievements()
        {
            m_achievements = new List<AchievementBase>();

            m_achievements.Add(new CountingAchievement(AchievementNames.Blink_2000_Times_For_Yes.ToString(), 20));
            m_achievements.Add(new BooleanAchievement(AchievementNames.Stairmaster_3000.ToString()));
            m_achievements.Add(new BooleanAchievement(AchievementNames.Stairway_To_The_Livingroom.ToString()));
            m_achievements.Add(new BooleanAchievement(AchievementNames.Piss_Perfect.ToString()));
        }

        /// <summary>
        /// Use this when starting a new game to reset certain playthrough-specific achievements
        /// </summary>
        public void OnStartNewGame()
        {
            //reset playthrough-specific achievements
        }


        public AchievementBase GetAchievement(AchievementNames name)
        {
            string n = name.ToString();

            for (int i = 0; i < m_achievements.Count; i++)
            {
                if (m_achievements[i].Name == n) return m_achievements[i];
            }
            return null;
        }

    }

    /// <summary>
    /// Achievement that counts up to a certain value before being unlocked
    /// </summary>
    [System.Serializable]
    public class CountingAchievement : AchievementBase {

        private int m_unlockCount;
        private int m_count=0;

        public override bool IsUnlocked
        {
            get
            {
                return m_count>=m_unlockCount;
            }
        }

        public CountingAchievement(string name, int unlockCount) : base(name)
        {
            m_unlockCount=unlockCount;
        }

        public override bool AddValue(int v)
        {
            if (IsUnlocked) return false;

            m_count += v;
            return IsUnlocked;
        }

        public override bool SetValue(int v)
        {
            if (IsUnlocked) return false;

            m_count = v;
            return IsUnlocked;
        }

    }

    /// <summary>
    /// Achievement that is simply unlocked at a certain point
    /// </summary>
    [System.Serializable]
    public class BooleanAchievement : AchievementBase
    {
        private bool m_unlocked = false;

        public override bool IsUnlocked
        {
            get
            {
                return m_unlocked;
            }
        }

        public BooleanAchievement(string name) : base(name) { }

        public override bool Unlock()
        {
            if (IsUnlocked) return false;

            m_unlocked = true;
            return true;
        }
    }

}