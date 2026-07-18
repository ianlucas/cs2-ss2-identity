/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Events;

namespace Identity;

public partial class Identity
{
    public void OnTick()
    {
        if (!ConVars.ForceRating.Value)
            return;
        var gameRules = Core.EntitySystem.GetGameRules();
        if (gameRules == null)
            return;
        var teamIntroPeriod = gameRules.TeamIntroPeriod;
        var isUpdateRating = gameRules.LastTeamIntroPeriod != teamIntroPeriod;
        gameRules.LastTeamIntroPeriod = teamIntroPeriod;
        if (!isUpdateRating)
            return;
        var players = Core.PlayerManager.GetAllPlayers();
        foreach (var player in players)
            if (!player.IsFakeClient)
            {
                var rating = player.GetState().Data?.Rating;
                if (teamIntroPeriod)
                    player.Controller.HideCompetitiveRanking();
                else if (rating != null)
                    player.Controller.SetCompetitiveRanking(rating.Value);
            }
    }

    public void OnClientSteamAuthorize(IOnClientSteamAuthorizeEvent @event)
    {
        var player = Core.PlayerManager.GetPlayer(@event.PlayerId);
        if (player != null && !player.IsFakeClient)
            player.AuthenticateAsync();
    }
}
