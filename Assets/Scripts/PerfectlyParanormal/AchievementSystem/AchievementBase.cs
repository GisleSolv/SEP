using UnityEngine;
using System.Collections;


namespace PerfectlyParanormal.AchievementSystem
{
    /// <summary>
    /// Base class for achievements, contains virtual methods for adding and setting values, as well as unlocking
    /// </summary>
    [System.Serializable]
    public class AchievementBase
    {

        private string m_name;

        public string Name { get { return m_name; } }

        public virtual bool IsUnlocked { get { return false; } }

        public AchievementBase(string name)
        {
            m_name = name;
            
        }

        /// <summary>
        /// Returns true when achievement is unlocked
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public virtual bool SetValue(int v)
        {
           
            return IsUnlocked;
        }

        /// <summary>
        /// Returns true when achievement is unlocked
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public virtual bool AddValue(int v)
        {

            return IsUnlocked;
        }

        /// <summary>
        /// Returns true when achievement is unlocked
        /// </summary>
        /// <returns></returns>
        public virtual bool Unlock()
        {
            return false;
        }
    }

}