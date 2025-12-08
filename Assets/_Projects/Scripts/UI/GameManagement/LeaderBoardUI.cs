using System;
using System.Collections.Generic;
using System.Linq;
using _Projects.Player;
using _Projects.Scripts.Serializables;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace _Projects.Scripts.UI.GameUIManagement
{
    public class LeaderBoardUI : NetworkBehaviour
    {
        [Header("References")] [SerializeField]
        private Transform _rankingParent;

        [SerializeField] private LeaderBoardRanking _LeaderBoardRankingPrefab;
        [SerializeField] private TMP_Text _rankText;

        [Header("Settings")] [SerializeField] private int _entitiesToDisplay = 4;

        private NetworkList<LeaderBoardEntitesSerializabes> _leaderboardEntityList;
        private List<LeaderBoardRanking> _LeaderBoardRankingList = new List<LeaderBoardRanking>();

        private void Awake()
        {
            _leaderboardEntityList = new NetworkList<LeaderBoardEntitesSerializabes>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                _leaderboardEntityList.OnListChanged += HandleLeaderboardEntitiesChanged;

                foreach (LeaderBoardEntitesSerializabes entity in _leaderboardEntityList)
                {
                    HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderBoardEntitesSerializabes>
                    {
                        Type = NetworkListEvent<LeaderBoardEntitesSerializabes>.EventType.Add,
                        Value = entity
                    });
                }
            }

            if (IsServer)
            {
                PlayerNetworkController[]
                    players = FindObjectsByType<PlayerNetworkController>(FindObjectsSortMode.None);
                foreach (PlayerNetworkController player in players)
                {
                    HandlePlayerSpawned(player);
                }

                PlayerNetworkController.OnPlayerSpawned += HandlePlayerSpawned;
                PlayerNetworkController.OnPlayerDespawned += HandlePlayerDespawned;
            }
        }
 
        private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderBoardEntitesSerializabes> changeEvent)
        {
            switch (changeEvent.Type)
            {
                case NetworkListEvent<LeaderBoardEntitesSerializabes>.EventType.Add:
                    if (!_LeaderBoardRankingList.Any(x => x.ClientId == changeEvent.Value.ClientId))
                    {
                        LeaderBoardRanking LeaderBoardRankingInstance
                            = Instantiate(_LeaderBoardRankingPrefab, _rankingParent);

                        LeaderBoardRankingInstance.SetData(
                            changeEvent.Value.ClientId,
                            changeEvent.Value.PlayerName,
                            changeEvent.Value.Score);

                        _LeaderBoardRankingList.Add(LeaderBoardRankingInstance);
                    }

                    UpdatePlayerRankText();
                    break;
                case NetworkListEvent<LeaderBoardEntitesSerializabes>.EventType.Remove:
                    LeaderBoardRanking LeaderBoardRankingToRemove
                        = _LeaderBoardRankingList.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);

                    if (LeaderBoardRankingToRemove != null)
                    {
                        LeaderBoardRankingToRemove.transform.SetParent(null);
                        Destroy(LeaderBoardRankingToRemove.gameObject);
                        _LeaderBoardRankingList.Remove(LeaderBoardRankingToRemove);
                        UpdatePlayerRankText();
                    }

                    break;
                case NetworkListEvent<LeaderBoardEntitesSerializabes>.EventType.Value:
                    LeaderBoardRanking LeaderBoardRankingToUpdate
                        = _LeaderBoardRankingList.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);

                    if (LeaderBoardRankingToUpdate != null)
                    {
                        LeaderBoardRankingToUpdate.UpdateScore(changeEvent.Value.Score);
                    }

                    break;
            }

            _LeaderBoardRankingList.Sort((x, y) => y.Score.CompareTo(x.Score));

            for (int i = 0; i < _LeaderBoardRankingList.Count; ++i)
            {
                _LeaderBoardRankingList[i].transform.SetSiblingIndex(i);
                _LeaderBoardRankingList[i].UpdateOrder();

                bool shouldShow = i <= _entitiesToDisplay - 1;
                _LeaderBoardRankingList[i].gameObject.SetActive(shouldShow);
            }

            LeaderBoardRanking myRanking
                = _LeaderBoardRankingList.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);

            if (myRanking != null)
            {
                if (myRanking.transform.GetSiblingIndex() >= _entitiesToDisplay)
                {
                    _rankingParent.GetChild(_entitiesToDisplay - 1).gameObject.SetActive(false);
                    myRanking.gameObject.SetActive(true);
                }

                UpdatePlayerRankText();
            }
        }

        private void HandlePlayerSpawned(PlayerNetworkController playerNetworkController)
        {
            LeaderBoardEntitesSerializabes leaderBoardEntitesSerializabes = new LeaderBoardEntitesSerializabes(
                playerNetworkController.OwnerClientId,
                playerNetworkController.PlayerName.Value.ToString(),
                0
            );
            _leaderboardEntityList.Add(leaderBoardEntitesSerializabes);

            playerNetworkController.PlayerScoreController.PlayerScore.OnValueChanged
                += (oldScore, newScore) =>
                    HandleScoreChanged(playerNetworkController.OwnerClientId, newScore);
        }

        private void HandlePlayerDespawned(PlayerNetworkController playerNetworkController)
        {
            if (_leaderboardEntityList == null)
            {
                return;
            }

            foreach (LeaderBoardEntitesSerializabes entity in _leaderboardEntityList)
            {
                if (entity.ClientId != playerNetworkController.OwnerClientId)
                {
                    continue;
                }

                _leaderboardEntityList.Remove(entity);
                break;
            }

            playerNetworkController.PlayerScoreController.PlayerScore.OnValueChanged
                -= (oldScore, newScore) =>
                    HandleScoreChanged(playerNetworkController.OwnerClientId, newScore);
        }

        private void HandleScoreChanged(ulong clientId, int newScore)
        {
            for (int i = 0; i < _leaderboardEntityList.Count; ++i)
            {
                if (_leaderboardEntityList[i].ClientId != clientId) continue;

                LeaderBoardEntitesSerializabes leaderBoardEntitesSerializabes = new LeaderBoardEntitesSerializabes(
                    _leaderboardEntityList[i].ClientId,
                    _leaderboardEntityList[i].PlayerName.Value,
                    newScore
                );

                UpdatePlayerRankText();

                return;
            }
        }

        private void UpdatePlayerRankText()
        {
            LeaderBoardRanking myRanking
                = _LeaderBoardRankingList.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);

            if (myRanking == null)
            {
                return;
            }

            int rank = myRanking.transform.GetSiblingIndex() + 1;
            string rankSuffix = GetRankSuffix(rank);

            _rankText.text = $"{rank}<sup>{rankSuffix}</sup>/{_LeaderBoardRankingList.Count}";
        }

        private string GetRankSuffix(int rank)
        {
            return rank switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            };
        }

        public List<LeaderBoardEntitesSerializabes> GetLeaderboardData()
        {
            List<LeaderBoardEntitesSerializabes> leaderboardData = new List<LeaderBoardEntitesSerializabes>();

            foreach (var entity in _leaderboardEntityList)
            {
                leaderboardData.Add(entity);
            }

            return leaderboardData;
        }

        public string GetWinnersName()
        {
            if (_LeaderBoardRankingList.Count > 0)
            {
                return _LeaderBoardRankingList[0].GetPlayerName();
            }

            return "No Winner";
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient)
            {
                _leaderboardEntityList.OnListChanged -= HandleLeaderboardEntitiesChanged;
            }

            if (IsServer)
            {
                PlayerNetworkController.OnPlayerSpawned -= HandlePlayerSpawned;
                PlayerNetworkController.OnPlayerDespawned -= HandlePlayerDespawned;
            }
        }
    }
}