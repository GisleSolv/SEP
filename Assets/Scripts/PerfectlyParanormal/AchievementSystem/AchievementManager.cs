using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace PerfectlyParanormal.AchievementSystem
{
    /// <summary>
    /// Manager for achievements, loads achievements file or creates a new one if it doesnt exist
    /// </summary>
    public static class AchievementManager
    {

        private static SamAchievements m_achievements;

        public static SamAchievements Achievements { get { if (m_achievements == null) LoadAchievements(); return m_achievements; } }

        public static void LoadAchievements()
        {
            if (!File.Exists(Application.persistentDataPath + "/samach.ach"))
            {
                m_achievements = new SamAchievements();
                return;
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + "/samach.ach", FileMode.Open);

            m_achievements = (SamAchievements)bf.Deserialize(fs);
            fs.Close();
        }

        public static void SaveAchievements()
        {
            if (m_achievements == null) return;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + "/samach.ach", FileMode.Create);
            bf.Serialize(fs, m_achievements);
            fs.Close();
        }

        public static void OnNewGameStarted()
        {
            Achievements.OnStartNewGame();
        }

        public static void Unlock(SamAchievements.AchievementNames name)
        {
            if(Achievements.GetAchievement(name).Unlock())  //returns true if achievements is unlocked
            {
                Debug.Log("Unlocked Achievement: " + name.ToString());
                SaveAchievements();
            }
        }

        public static void SetValue(SamAchievements.AchievementNames name, int v)
        {
            if(Achievements.GetAchievement(name).SetValue(v)) //returns true if achievements is unlocked
            {
                Debug.Log("Unlocked Achievement: " + name.ToString());
            }
        }

        public static void AddValue(SamAchievements.AchievementNames name, int v)
        {
            if(Achievements.GetAchievement(name).AddValue(v)) //returns true if achievements is unlocked
            {
                Debug.Log("Unlocked Achievement: " + name.ToString());
            }
        }
    }


}