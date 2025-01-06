using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ListItem is a simple gameobject that is used inside leaderboard UI to display the rank data.
/// All ListItems are created by the main LeaderboardManager class.
/// </summary>

namespace SnakeWarzIO
{
    public class ListItem : MonoBehaviour
    {
        public Text rankUI;
        public Text playerNameUI;
        public Text scoreUI;

        internal int rank;
        internal string playerName;
        internal int score;

        public void BuildListItemInstance(int _rank, string _playerName, int _score, Color _color)
        {
            rank = _rank;
            playerName = _playerName;
            score = _score;

            rankUI.text = "" + _rank;
            playerNameUI.text = "" + _playerName;
            scoreUI.text = "" + _score;

            //Apply Color
            rankUI.color = _color;
            playerNameUI.color = _color;
            scoreUI.color = _color;
        }
    }
}