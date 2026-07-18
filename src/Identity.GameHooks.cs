/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.GameHooks;

namespace Identity;

public partial class Identity
{
    public void OnClientProcessUsercmds(ref ProcessUsercmdsPreContext ctx)
    {
        var player = ctx.Params.Player;
        if (player.IsValid && !player.IsFakeClient)
            player.TrySendRankReveal();
    }
}
