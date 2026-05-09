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
    private static readonly ConcurrentDictionary<ulong, IPlayerState> _playerStateManager = [];

    extension(IPlayer self)
    {
        public IPlayerState GetState()
        {
            return _playerStateManager.GetOrAdd(self.SteamID, _ => new());
        }

        public void RemoveState()
        {
            _playerStateManager.TryRemove(self.SteamID, out var _);
        }

        public async void HandleSteamAuthorize()
        {
            var steamId = self.SteamID;
            var name = self.Controller.PlayerName;
            var playerState = self.GetState();
            if (self.IsFakeClient || !Api.IsActive() || playerState.IsFetching)
                return;
            Swiftly.Core.Logger.LogInformation(
                "Player {Name} (id: {Id}) is authenticating...",
                name,
                steamId
            );
            playerState.IsFetching = true;
            var user = await Api.FetchUser(steamId);
            playerState.Data = user;
            playerState.IsFetching = false;
            Swiftly.Core.Scheduler.NextWorldUpdate(() =>
            {
                if (!self.Controller.IsValid)
                {
                    Swiftly.Core.Logger.LogInformation(
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
                if (ConVars.IsForceNickname.Value)
                    self.Controller.SetPlayerName(user.Nickname);
                if (
                    ConVars.IsForceRating.Value
                    && Swiftly.Core.EntitySystem.GetGameRules()?.TeamIntroPeriod != true
                )
                    self.Controller.SetCompetitiveRanking(user.Rating);
                if (user.Flags.Length > 0)
                    foreach (var flag in user.Flags)
                        Swiftly.Core.Permission.AddPermission(steamId, flag);
                Swiftly.Core.Logger.LogInformation(
                    "Player {Name} (id: {Id}) is authenticated (rating={Rating}, flags={Flags}).",
                    name,
                    steamId,
                    user.Rating,
                    user.Flags
                );
            });
        }

        public void HandleProcessUsercmds()
        {
            if (!ConVars.IsForceRating.Value)
                return;
            var playerState = self.GetState();
            var pressedButtons = self.PressedButtons;
            var isSendNetMessage = (
                (pressedButtons & GameButtonFlags.Tab) != 0
                && (playerState.LastPressedButtons & GameButtonFlags.Tab) == 0
            );
            playerState.LastPressedButtons = pressedButtons;
            if (isSendNetMessage)
                Swiftly.Core.NetMessage.Send<CCSUsrMsg_ServerRankRevealAll>(msg =>
                    msg.Recipients.AddRecipient(self.PlayerID)
                );
        }

        public void HandleDisconnect()
        {
            self.RemoveState();
        }
    }
}
