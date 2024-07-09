using System;
using UnityEngine;

namespace CoppraGames
{
    public class Main : MonoBehaviour
    {
        public static Main instance;

        public GameObject MainMenu;
        public DailyRewardsWindow DailyRewardsWindow;
        public QuestWindow QuestWindow;
        public SpinWheelController SpinWheelWindow;

        private const string LastDailyRewardTimeKey = "last_daily_reward_time";

        void Awake()
        {
            instance = this;

            ShowDailyRewardsWindow(false);
            ShowQuestWindow(false);
            ShowSpinWheelWindow(false);
            ShowMainMenu(true);

            CheckAndHandleDailyReward();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void CheckAndHandleDailyReward()
        {
            // Check if the last daily reward time exists
            if (!PlayerPrefs.HasKey(LastDailyRewardTimeKey))
            {
                // First time user opens the app
                ShowDailyRewardsWindow(true);
                UpdateLastDailyRewardTime();
            }
            else
            {
                // Retrieve the last daily reward time
                string lastDailyRewardTimeString = PlayerPrefs.GetString(LastDailyRewardTimeKey, string.Empty);
                if (DateTime.TryParse(lastDailyRewardTimeString, out DateTime lastDailyRewardTime))
                {
                    // Check if 24 hours have passed since the last reward
                    if ((DateTime.Now - lastDailyRewardTime).TotalHours >= 24)
                    {
                        ShowDailyRewardsWindow(true);
                        UpdateLastDailyRewardTime();
                    }
                }
                else
                {
                    // Handle parsing failure (shouldn't normally happen)
                    UpdateLastDailyRewardTime();
                }
            }
        }

        private void UpdateLastDailyRewardTime()
        {
            PlayerPrefs.SetString(LastDailyRewardTimeKey, DateTime.Now.ToString());
        }

        public void OnClickDailyRewardsButton()
        {
            ShowDailyRewardsWindow(true);
            ShowQuestWindow(false);
            ShowSpinWheelWindow(false);
            ShowMainMenu(false);
        }

        public void OnClickQuestButton()
        {
            ShowQuestWindow(true);
            ShowDailyRewardsWindow(false);
            ShowSpinWheelWindow(false);
            ShowMainMenu(false);
        }

        public void OnClickSpinwheelButton()
        {
            ShowQuestWindow(false);
            ShowDailyRewardsWindow(false);
            ShowSpinWheelWindow(true);
            ShowMainMenu(false);
        }

        public void ShowMainMenu(bool isTrue)
        {
            if (MainMenu)
            {
                MainMenu.SetActive(isTrue);
            }
        }

        // DAILY REWARDS OPTIONS
        public void ShowDailyRewardsWindow(bool isTrue)
        {
            if (DailyRewardsWindow)
            {
                DailyRewardsWindow.gameObject.SetActive(isTrue);

                if (isTrue)
                {
                    DailyRewardsWindow.Init();
                }
                else
                {
                    ShowMainMenu(true);
                }
            }
        }

        public void OnClickResetDailyRewardsButton()
        {
            PlayerPrefs.DeleteAll();
            DailyRewardsWindow.Init();
        }

        // QUESTS OPTIONS
        public void ShowQuestWindow(bool isTrue)
        {
            if (QuestWindow)
            {
                QuestWindow.gameObject.SetActive(isTrue);

                if (isTrue)
                {
                    QuestWindow.Init();
                }
                else
                {
                    ShowMainMenu(true);
                }
            }
        }

        public void OnClickFinishMission()
        {
            QuestManager.instance.OnAchieveQuestGoal(QuestManager.QuestGoals.COMPLETE_MISSION);
        }

        public void OnClickUpgradeHero()
        {
            QuestManager.instance.OnAchieveQuestGoal(QuestManager.QuestGoals.UPGRADE_HERO);
        }

        public void OnClickKillEnemy()
        {
            QuestManager.instance.OnAchieveQuestGoal(QuestManager.QuestGoals.DESTROY_ENEMY);
        }

        public void OnClickCollectDailyRewards()
        {
            // When the daily reward is collected, update the last reward time
            UpdateLastDailyRewardTime();
            ShowDailyRewardsWindow(false); // Hide the daily rewards window after collecting the reward
        }

        public void OnClickResetQuest()
        {
            QuestManager.instance.ResetAllDailyQuests();
        }

        // SPINWHEEL OPTIONS
        public void ShowSpinWheelWindow(bool isTrue)
        {
            if (SpinWheelWindow)
            {
                SpinWheelWindow.gameObject.SetActive(isTrue);

                if (isTrue)
                {
                    SpinWheelWindow.Init();
                }
                else
                {
                    ShowMainMenu(true);
                }
            }
        }
    }
}
