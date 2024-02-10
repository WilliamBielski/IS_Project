using IS_Project.AI;
using IS_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Project.Managers
{
    public class GameManager
    {
        private GameBoard _gameBoard;
        private TurnBasedAI _turnBasedAI;

        public GameManager()
        {
            _gameBoard = new GameBoard();
            _turnBasedAI = new TurnBasedAI()
            {
                currentGameBoard = _gameBoard
            };
        }

        public void Update()
        {
            if (!(_gameBoard.redVictory || _gameBoard.blueVictory))
            {
                if (!_gameBoard.isRedTurn)
                {
                    _turnBasedAI.Update();
                }
                else
                {
                    _gameBoard.Update();
                }
            }
        }
        public void Draw()
        {
            _gameBoard.Draw();
        }
    }
}
