/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.ProtobufDefinitions;

namespace Identity;

public static class IPlayerExtensions
{
    private static readonly ConcurrentDictionary<ulong, PlayerState> _states = [];

    extension(IPlayer self)
    {
        public PlayerState GetState()
        {
            return _states.GetOrAdd(self.SteamID, _ => new());
        }

        public void ClearState()
        {
            _states.TryRemove(self.SteamID, out var _);
        }

        public async void AuthenticateAsync()
        {
            var steamId = self.SteamID;
            var name = self.Controller.PlayerName;
            var playerState = self.GetState();
            if (self.IsFakeClient || !Api.IsConfigured || playerState.IsFetching)
                return;
            Runtime.Core.Logger.LogInformation(
                "Player {Name} (id: {Id}) is authenticating...",
                name,
                steamId
            );
            playerState.IsFetching = true;
            var user = await Api.FetchUserAsync(steamId);
            playerState.Data = user;
            playerState.IsFetching = false;
            Runtime.Core.Scheduler.NextWorldUpdate(() =>
            {
                if (!self.Controller.IsValid)
                {
                    Runtime.Core.Logger.LogInformation(
                        "Player {Name} (id: {Id}) is no longer valid.",
                        name,
                        steamId
                    );
                    return;
                }
                if (user == null)
                {
                    if (ConVars.IsStrict.Value)
                        self.Kick(
                            "Failed to fetch player data.",
                            ENetworkDisconnectionReason.NETWORK_DISCONNECT_CLIENT_CONSISTENCY_FAIL
                        );
                    return;
                }
                if (ConVars.ForceNickname.Value)
                    self.Controller.SetPlayerName(user.Nickname);
                if (
                    ConVars.ForceRating.Value
                    && Runtime.Core.EntitySystem.GetGameRules()?.TeamIntroPeriod != true
                )
                    self.Controller.SetCompetitiveRanking(user.Rating);
                if (user.Flags.Length > 0)
                    foreach (var flag in user.Flags)
                        Runtime.Core.Permission.AddPermission(steamId, flag);
                Runtime.Core.Logger.LogInformation(
                    "Player {Name} (id: {Id}) is authenticated (rating={Rating}, flags={Flags}).",
                    name,
                    steamId,
                    user.Rating,
                    user.Flags
                );
            });
        }

        public void TrySendRankReveal()
        {
            if (!ConVars.ForceRating.Value)
                return;
            var playerState = self.GetState();
            var pressedButtons = self.PressedButtons;
            var isSendNetMessage = (
                (pressedButtons & GameButtonFlags.Tab) != 0
                && (playerState.LastPressedButtons & GameButtonFlags.Tab) == 0
            );
            playerState.LastPressedButtons = pressedButtons;
            if (isSendNetMessage)
                Runtime.Core.NetMessage.Send<CCSUsrMsg_ServerRankRevealAll>(msg =>
                    msg.Recipients.AddRecipient(self.PlayerID)
                );
        }

        public void OnDisconnect()
        {
            self.ClearState();
        }
    }
}
