using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using Game.Interfaces;
using Microsoft.ServiceFabric.Data;
using System.Runtime.Serialization;

namespace Game
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class Game : Actor, IGame
    {

        [DataContract]
        internal sealed class ActorState

        {
            [DataMember]
            public int[] Board;
            [DataMember]
            public string Winner;
            [DataMember]
            public List<Tuple<long, string>> Players;
            [DataMember]
            public int NextPlayerIndex;
            [DataMember]
            public int NumberOfMoves;
        }
        /// <summary>
        /// Initializes a new instance of Game
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public Game(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>


        protected override Task OnActivateAsync()
        {
            return this.StateManager.TryAddStateAsync("mystate", new ActorState {
                Board = new int[9],
                Winner = "",
                Players = new List<Tuple<long, string>>(),
                NextPlayerIndex = 0,
                NumberOfMoves = 0

            });

        }
        public async Task<int[]> GetGameBoardAsync()
        {
            ActorState state = await this.StateManager.GetStateAsync<ActorState>("mystate");
            return state.Board;
            
        }
        public async Task<string> GetWinnerAsync()
        {
            ActorState state = await this.StateManager.GetStateAsync<ActorState>("mystate");
            return state.Winner;
        }
        public async Task<bool> JoinGameAsync(long playerId, string playerName)
        {
            ActorState state = await this.StateManager.GetStateAsync<ActorState>("mystate");

            if (state.Players.Count >= 2 || state.Players.FirstOrDefault(p => p.Item2 == playerName) != null)
            {
                return false;
            }
            state.Players.Add(new Tuple<long, string>(playerId, playerName));
            await this.StateManager.SetStateAsync("mystate", state);
            return true;
        }

        public async Task<bool> MakeMoveAsync(long playerId, int x, int y)
        {
            ActorState state = await this.StateManager.GetStateAsync<ActorState>("mystate");
            if (x < 0 || x > 2 || y < 0 || y > 2
                || state.Players.Count != 2
                || state.NumberOfMoves >= 9
                ||state.Winner != "")
                return false;

            int index = state.Players.FindIndex(p => p.Item1 == playerId);
            if (index == state.NextPlayerIndex)
            {
                if (state.Board[y * 3 + x] == 0)
                {
                    int piece = index * 2 - 1;
                    state.Board[y * 3 + x] = piece;
                    state.NumberOfMoves++;
                    if (HasWon(piece * 3, state))
                        state.Winner = state.Players[index].Item2 + " (" +
                                           (piece == -1 ? "X" : "O") + ")";
                    else if (state.Winner == "" && state.NumberOfMoves >= 9)

                        state.Winner = "TIE";

                    state.NextPlayerIndex = (state.NextPlayerIndex + 1) % 2;

                    await this.StateManager.SetStateAsync("mystate", state);
                    return true;
                }
                else
                {
                    await this.StateManager.SetStateAsync("mystate", state);
                    return false;
                }
            }
            else
            {
                await this.StateManager.SetStateAsync("mystate", state);
                return false;
            }
        }

        private bool HasWon(int sum, ActorState state)
        {
            
            return state.Board[0] + state.Board[1] + state.Board[2] == sum
                || state.Board[3] + state.Board[4] + state.Board[5] == sum
                || state.Board[6] + state.Board[7] + state.Board[8] == sum
                || state.Board[0] + state.Board[3] + state.Board[6] == sum
                || state.Board[1] + state.Board[4] + state.Board[7] == sum
                || state.Board[2] + state.Board[5] + state.Board[8] == sum
                || state.Board[0] + state.Board[4] + state.Board[8] == sum
                || state.Board[2] + state.Board[4] + state.Board[6] == sum;

        }

    }
}
