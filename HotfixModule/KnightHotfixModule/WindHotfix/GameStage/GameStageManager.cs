﻿//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Core;
using WindHotfix.Core;

namespace WindHotfix.GameStage
{
    /// <summary>
    /// 游戏的关卡管理，管理一个游戏关卡的加载，卸载，关卡中的内容承载等功能。
    /// </summary>
    public class GameStageManager : THotfixSingleton<GameStageManager>
    {
        /// <summary>
        /// 关卡中的阶段, 相同Index的GameStage谁先谁后都没有关系。
        /// </summary>
        public Dict<int, GameStage>         gameStages;

        /// <summary>
        /// 当前的游戏模式
        /// </summary>
        public GameMode                     gameMode;

        private GameStageManager()
        {
        }

        /// <summary>
        /// 初始化游戏数据
        /// </summary>
        public void InitGame()
        {
            // 获得当前的游戏模式
            if (GameMode.GetCurrentMode == null) return;
            this.gameMode = GameMode.GetCurrentMode();

            // 开始初始化游戏数据，在这里面构建GameStage的信息等
            this.gameMode.InitData();

            //对GameStages进行排序
            this.gameStages = gameStages.Sort((item1, item2) => { return item1.Key.CompareTo(item2.Key); });
        }

        /// <summary>
        /// GameStage开始初始化
        /// </summary>
        public void StageIntialize()
        {
            if (this.gameStages == null) return;

            foreach (var rStageItem in this.gameStages)
            {
                rStageItem.Value.Init();
            }
        }

        /// <summary>
        /// GameStage开始执行
        /// </summary>
        public void StageRunning()
        {
            CoroutineManager.Instance.Start(StageRunning_Async());
        }

        /// <summary>
        /// 异步执行GameStage
        /// </summary>
        private IEnumerator StageRunning_Async()
        {
            if (this.gameStages == null) yield break;

            // @Tips: 很蛋疼的痛点，直接使用foreach在其中yield return Coroutine会卡死在里面，不知道是怎么回事。
            //        只能临时把Dict转成List再来等待。
            var rGameStageList = new List<KeyValuePair<int, GameStage>>(this.gameStages);
            for (int i = 0; i < rGameStageList.Count; i++)
            {
                yield return rGameStageList[i].Value.Run_Async();
            }
        }
    }
}